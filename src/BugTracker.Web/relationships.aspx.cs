using btnet;
using BugTracker.Web.Services.Bug;
using System;
using System.Data;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class relationships : Page
    {
		public int bugid;
		int previd;
		public DataSet ds;

		public Security security;
		public int permission_level;
		public string ses;

		private BugService _bugService = new BugService();

		protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


		protected void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.ANY_USER_OK);

			titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
					+ "relationships";


			string sql;
			add_err.InnerText = "";

			bugid = Convert.ToInt32(Util.sanitize_integer(Request["bgid"]));

			if (string.IsNullOrEmpty(Request["bugid"]))
			{
				previd = 0;
			}
			else
			{
				previd = Convert.ToInt32(Util.sanitize_integer(Request["prev"]));
			}


			int bugid2 = 0;

			permission_level = Bug.get_bug_permission_level(bugid, security);
			if (permission_level == Security.PERMISSION_NONE)
			{
				Response.Write("You are not allowed to view this item");
				Response.End();
			}

			ses = (string)Session["session_cookie"];
			string action = Request["actn"];

			//We are either adding or removing a relationship for a bug
			if (!string.IsNullOrEmpty(action))
			{
				if (Request["ses"] != ses)
				{
					Response.Write("session in Request doesn't match session cookie");
					Response.End();
				}

				if (permission_level == Security.PERMISSION_READONLY)
				{
					Response.Write("You are not allowed to edit this item");
					Response.End();
				}

				if (action == "remove") // remove
				{
					if (security.user.is_guest)
					{
						Response.Write("You are not allowed to delete a relationship");
						Response.End();
					}

					bugid2 = Convert.ToInt32(Util.sanitize_integer(Request["bugid2"]));

					_bugService.DeleteRelationship(bugid, bugid2, security.user.usid);
				}
				else
				{
					// adding
					if (Request["bugid2"] != null)
					{
						if (!Util.is_int(Request["bugid2"]))
						{
							add_err.InnerText = "Related ID must be an integer.";
						}
						else
						{
							bugid2 = Convert.ToInt32((Request["bugid2"]));

							if (bugid == bugid2)
							{
								add_err.InnerText = "Cannot create a relationship to self.";
							}
							else
							{
								bool bugFound = _bugService.DoesBugExist(bugid2);

								if (!bugFound)
								{
									add_err.InnerText = "Not found.";
								}
								else
								{
									bool hasRelationship = _bugService.DoesRelationshipExist(bugid, bugid2);

									if (hasRelationship)
									{
										add_err.InnerText = "Relationship already exists.";
									}
									else
									{
										// check permission of related bug
										int permission_level2 = Bug.get_bug_permission_level(bugid2, security);
										if (permission_level2 == Security.PERMISSION_NONE)
										{
											add_err.InnerText = "You are not allowed to view the related item.";
										}
										else
										{

											// insert the relationship both ways
											string type = Request["type"].Replace("'", "''");

											_bugService.AddRelationship(bugid, bugid2, security.user.usid, type, siblings.Checked, child_to_parent.Checked);

											add_err.InnerText = "Relationship was added.";
										}
									}
								}
							}
						}
					}
				}

			}

			sql = @"
				select bg_id [id],
					bg_short_desc [desc],
					re_type [comment],
					st_name [status],
					case
						when re_direction = 0 then ''
						when re_direction = 2 then 'child of $bg'
						else                       'parent of $bg' 
					end as [parent or child],
					'<a target=_blank href=edit_bug.aspx?id=' + convert(varchar,bg_id) + '>view</a>' [view]";

			if (!security.user.is_guest && permission_level == Security.PERMISSION_ALL)
			{

				sql += @"
					,'<a href=''javascript:remove(' + convert(varchar,re_bug2) + ')''>detach</a>' [detach]";
			}

			sql += @"
				from bugs
				inner join bug_relationships on bg_id = re_bug2
				left outer join statuses on st_id = bg_status
				where re_bug1 = $bg
				order by bg_id desc";


			sql = sql.Replace("$bg", Convert.ToString(bugid));
			sql = Util.alter_sql_per_project_permissions(sql, security);

			ds = btnet.DbUtil.get_dataset(sql);

			bgid.Value = Convert.ToString(bugid);

		}

		string get_bug_html(DataRow dr)
		{
			string s = @"
	
	
				<td valign=top>
				<div
				style='background: #dddddd; border: 1px solid blue; padding 15px;  width: 140px; height: 50px; overflow: hidden;'
				><a 
				href='relationships.aspx?bgid=$id&prev=$prev'>$id&nbsp;&nbsp;&nbsp;&nbsp;$title</a></div>";


			if (previd == (int)dr["id"])
			{
				s = s.Replace("1px solid blue", "2px solid red");
			}

			s = s.Replace("$id", Convert.ToString(dr["id"]));
			s = s.Replace("$prev", Convert.ToString(bugid));
			s = s.Replace(
				"$title",
				Server.HtmlEncode(
						Convert.ToString(dr["desc"])
					)
				);

			return s;
		}


		public void display_hierarchy()
		{

			string parents = "";
			string siblings = "";
			string children = "";


			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				string level = (string)dr["parent or child"];

				if (level.StartsWith("parent"))
				{
					parents += get_bug_html(dr);
				}
				else if (level.StartsWith("child"))
				{
					children += get_bug_html(dr);
				}
				else
				{
					siblings += get_bug_html(dr);
				}
			}

			Response.Write("Parents:&nbsp;<table border=0 cellspacing=15 cellpadding=0><tr>");
			Response.Write(parents);
			Response.Write("</table><p>");
			Response.Write("Siblings:&nbsp;<table border=0 cellspacing=15 cellpadding=0><tr>");
			Response.Write(siblings);
			Response.Write("</table><p>");
			Response.Write("Children:&nbsp;<table border=0 cellspacing=15 cellpadding=0><tr>");
			Response.Write(children);
			Response.Write("</table>");
		}
	}
}
