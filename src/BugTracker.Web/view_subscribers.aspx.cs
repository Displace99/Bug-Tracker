using btnet;
using BugTracker.Web.Services.Subscription;
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
		public int bugid = 0;
		public DataSet ds;

		private SubscriptionService _subscriptionService = new SubscriptionService();

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

				_subscriptionService.AddNewSubscriber(bugid, userId);
			}

			// clean up bug subscriptions that no longer fit the security restrictions
			Bug.auto_subscribe(bugid);

			string sessionCookie = Convert.ToString(Session["session_cookie"]);
			//Get list of currently subscribed users
			ds = _subscriptionService.GetSubscriberList(bugid, sessionCookie, security.user.is_admin);
            //GetSubscriberList(bugid);

			//Gets the list of available users that could be added as subscribers
			GetAvailableSubscribers(bugid);

		}

		//Gets a list of all users that could be added to the bug as a subscriber. 
		//This is used to populate the drop down box
		private void GetAvailableSubscribers(int bugId)
        {
			DataView dv = _subscriptionService.GetAvailableSubscribers(bugId);

			//Populating control
			userid.DataSource = dv;
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
