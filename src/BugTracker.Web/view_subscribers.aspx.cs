using btnet;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class view_subscribers : Page
    {
		public Security security;
		string sql;
		public int bugid = 0;
		public DataSet ds;

		protected void Page_Load(Object sender, EventArgs e)
		{
			int userId = 0;

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.ANY_USER_OK);

			titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
				+ "view subscribers";

			if (!Int32.TryParse(Request["id"], out bugid))
			{
				Response.Write("Not able to find this item");
				Response.End();
			}

			int permission_level = Bug.get_bug_permission_level(bugid, security);
			if (permission_level == Security.PERMISSION_NONE)
			{
				Response.Write("You are not allowed to view this item");
				Response.End();
			}

			string action = Request["actn"];

			if (!String.IsNullOrEmpty(action))
			{
				if (permission_level == Security.PERMISSION_READONLY)
				{
					Response.Write("You are not allowed to edit this item");
					Response.End();
				}

				if (!Int32.TryParse(Request["userid"], out userId))
				{
					Response.Write("Not able to find the user");
					Response.End();
				}

				AddNewSubscriber(bugid, userId);	
			}

			// clean up bug subscriptions that no longer fit the security restrictions
			Bug.auto_subscribe(bugid);

			//Get list of currently subscribed users
			GetSubscriberList(bugid);

			//Gets the list of available users that could be added as subscribers
			GetAvailableSubscribers(bugid);

		}

		//Adds a user as a new subscriber to a bug
		private void AddNewSubscriber(int bugId, int userId)
        {
			sql = @"delete from bug_subscriptions where bs_bug = @bg and bs_user = @us;
						insert into bug_subscriptions (bs_bug, bs_user) values(@bg, @us)";
			
			SqlCommand cmd = new SqlCommand(sql);
			cmd.Parameters.AddWithValue("@bg", bugId);
			cmd.Parameters.AddWithValue("@us", userId);
			
			DbUtil.execute_nonquery(cmd);

			// send a notification to this user only
			btnet.Bug.send_notifications(btnet.Bug.UPDATE, bugid, security, userId);
		}

		private void GetSubscriberList(int bugId)
        {
			// show who is subscribed
			if (security.user.is_admin)
			{
				sql = @"
					select
					'<a href=delete_subscriber.aspx?ses=$ses&bg_id=$bg&us_id=' + convert(varchar,us_id) + '>unsubscribe</a>'	[$no_sort_unsubscriber],
					us_username [user],
					us_lastname + ', ' + us_firstname [name],
					us_email [email],
					case when us_reported_notifications < 4 or us_assigned_notifications < 4 or us_subscribed_notifications < 4 then 'Y' else 'N' end [user is<br>filtering<br>notifications]
					from bug_subscriptions
					inner join users on bs_user = us_id
					where bs_bug = $bg
					and us_enable_notifications = 1
					and us_active = 1
					order by 1";

				sql = sql.Replace("$ses", Convert.ToString(Session["session_cookie"]));

			}
			else
			{
				sql = @"
					select
					us_username [user],
					us_lastname + ', ' + us_firstname [name],
					us_email [email],
					case when us_reported_notifications < 4 or us_assigned_notifications < 4 or us_subscribed_notifications < 4 then 'Y' else 'N' end [user is<br>filtering<br>notifications]
					from bug_subscriptions
					inner join users on bs_user = us_id
					where bs_bug = $bg
					and us_enable_notifications = 1
					and us_active = 1
					order by 1";
			}

			sql = sql.Replace("$bg", Convert.ToString(bugId));
			ds = btnet.DbUtil.get_dataset(sql);
		}

		//Gets a list of all users that could be added to the bug as a subscriber. 
		//This is used to populate the drop down box
		private void GetAvailableSubscribers(int bugId)
        {
			sql = @"
				declare @project int;
				declare @org int;
				select @project = bg_project, @org = bg_org from bugs where bg_id = $bg;";

			// Only users explicitly allowed will be listed
			if (Util.get_setting("DefaultPermissionLevel", "2") == "0")
			{
				sql += @"select us_id, case when $fullnames then us_lastname + ', ' + us_firstname else us_username end us_username
					from users
					where us_active = 1
					and us_enable_notifications = 1
					and us_id in
						(select pu_user from project_user_xref
						where pu_project = @project
						and pu_permission_level <> 0)
					and us_id not in (
						select us_id
						from bug_subscriptions
						inner join users on bs_user = us_id
						where bs_bug = $bg
						and us_enable_notifications = 1
						and us_active = 1)
					and us_id not in (
						select us_id from users
						inner join orgs on us_org = og_id
						where us_org <> @org
						and og_other_orgs_permission_level = 0)

					order by us_username; ";
			}
			// Only users explictly DISallowed will be omitted
			else
			{
				sql += @"select us_id, case when $fullnames then us_lastname + ', ' + us_firstname else us_username end us_username
					from users
					where us_active = 1
					and us_enable_notifications = 1
					and us_id not in
						(select pu_user from project_user_xref
						where pu_project = @project
						and pu_permission_level = 0)
					and us_id not in (
						select us_id
						from bug_subscriptions
						inner join users on bs_user = us_id
						where bs_bug = $bg
						and us_enable_notifications = 1
						and us_active = 1)
					and us_id not in (
						select us_id from users
						inner join orgs on us_org = og_id
						where us_org <> @org
						and og_other_orgs_permission_level = 0)
					order by us_username; ";
			}

			if (Util.get_setting("UseFullNames", "0") == "0")
			{
				// false condition
				sql = sql.Replace("$fullnames", "0 = 1");
			}
			else
			{
				// true condition
				sql = sql.Replace("$fullnames", "1 = 1");
			}

			sql = sql.Replace("$bg", Convert.ToString(bugid));

			//Populating control
			userid.DataSource = DbUtil.get_dataview(sql);
			userid.DataTextField = "us_username";
			userid.DataValueField = "us_id";
			userid.DataBind();

			if (userid.Items.Count == 0)
			{
				userid.Items.Insert(0, new ListItem("[no users to select]", "0"));
			}
			else
			{
				userid.Items.Insert(0, new ListItem("[select to add]", "0"));
			}
		}
	}
}
