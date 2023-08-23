using System;
using System.Data;
using System.Web;
using System.Web.UI;
using btnet;
using BugTracker.Web.Services.Bug;

namespace BugTracker.Web
{
    public partial class tasks : Page
    {
		public int bugid;
		public DataSet ds;

		public Security security;
		public int permission_level;
		string ses;

		private TaskService _taskService = new TaskService();

		public void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

		public void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.ANY_USER_OK);

			titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
					+ "tasks";

			bugid = Convert.ToInt32(Util.sanitize_integer(Request["bugid"]));

			permission_level = Bug.get_bug_permission_level(bugid, security);
			if (permission_level == Security.PERMISSION_NONE)
			{
				Response.Write("You are not allowed to view tasks for this item");
				Response.End();
			}

			if (security.user.is_admin || security.user.can_view_tasks)
			{
				// allowed
			}
			else
			{
				Response.Write("You are not allowed to view tasks");
				Response.End();
			}


			ses = (string)Session["session_cookie"];

			ds = _taskService.GetTasksByBugId(bugid, permission_level, security.user, ses);
		}
	}
}
