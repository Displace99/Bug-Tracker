using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet;
using BugTracker.Web.Services.Bug;

namespace BugTracker.Web
{
    public partial class delete_task : Page
    {
		String sql;

		public Security security;
		private TaskService _taskService = new TaskService();

		public void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

		///////////////////////////////////////////////////////////////////////
		public void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();

			security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

			if (Request.QueryString["ses"] != (string)Session["session_cookie"])
			{
				Response.Write("session in URL doesn't match session cookie");
				Response.End();
			}

			string string_bugid = Util.sanitize_integer(Request["bugid"]);
			int bugid = Convert.ToInt32(string_bugid);

			int permission_level = Bug.get_bug_permission_level(bugid, security);

			if (permission_level != Security.PERMISSION_ALL)
			{
				Response.Write("You are not allowed to edit this item");
				Response.End();
			}

			string string_tsk_id = Util.sanitize_integer(Request["id"]);
			int taskid = Convert.ToInt32(string_tsk_id);

			if (IsPostBack)
			{
				// do delete here
				_taskService.DeleteTask(taskid, bugid);

				Response.Redirect("tasks.aspx?bugid=" + string_bugid);
			}
			else
			{
				titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
					+ "delete task";

				back_href.HRef = "tasks.aspx?bugid=" + string_bugid;

				DataRow dr = _taskService.GetTaskById(taskid, bugid);

				confirm_href.InnerText = "confirm delete of task: " + Convert.ToString(dr["tsk_description"]);
			}
		}
	}
}
