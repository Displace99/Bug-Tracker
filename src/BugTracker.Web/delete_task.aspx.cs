using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet;

namespace BugTracker.Web
{
    public partial class delete_task : Page
    {
		String sql;

		public Security security;

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

			string string_bugid = btnet.Util.sanitize_integer(Request["bugid"]);
			int bugid = Convert.ToInt32(string_bugid);

			int permission_level = Bug.get_bug_permission_level(bugid, security);

			if (permission_level != Security.PERMISSION_ALL)
			{
				Response.Write("You are not allowed to edit this item");
				Response.End();
			}

			string string_tsk_id = btnet.Util.sanitize_integer(Request["id"]);
			int tsk_id = Convert.ToInt32(string_tsk_id);

			if (IsPostBack)
			{
				// do delete here

				sql = @"delete bug_tasks where tsk_id = $tsk_id and tsk_bug = $bugid";
				sql = sql.Replace("$tsk_id", string_tsk_id);
				sql = sql.Replace("$bugid", string_bugid);
				btnet.DbUtil.execute_nonquery(sql);
				Response.Redirect("tasks.aspx?bugid=" + string_bugid);
			}
			else
			{

				titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
					+ "delete task";

				back_href.HRef = "tasks.aspx?bugid=" + string_bugid;

				sql = @"select tsk_description from bug_tasks where tsk_id = $tsk_id and tsk_bug = $bugid";
				sql = sql.Replace("$tsk_id", string_tsk_id);
				sql = sql.Replace("$bugid", string_bugid);

				DataRow dr = btnet.DbUtil.get_datarow(sql);

				confirm_href.InnerText = "confirm delete of task: " + Convert.ToString(dr["tsk_description"]);

			}


		}
	}
}
