using System;
using System.Data;
using System.Web;
using System.Web.UI;
using btnet;

namespace BugTracker.Web
{
    public partial class tasks : Page
    {
		public int bugid;
		public DataSet ds;

		public Security security;
		public int permission_level;
		string ses;

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

			string sql = "select tsk_id [id],";

			if (permission_level == Security.PERMISSION_ALL && !security.user.is_guest && (security.user.is_admin || security.user.can_edit_tasks))
			{
				sql += @"
					'<a   href=edit_task.aspx?bugid=$bugid&id=' + convert(varchar,tsk_id) + '>edit</a>'   [$no_sort_edit],
					'<a href=delete_task.aspx?ses=$ses&bugid=$bugid&id=' + convert(varchar,tsk_id) + '>delete</a>' [$no_sort_delete],";
			}

			sql += "tsk_description [description]";

			if (btnet.Util.get_setting("ShowTaskAssignedTo", "1") == "1")
			{
				sql += ",us_username [assigned to]";
			}

			if (btnet.Util.get_setting("ShowTaskPlannedStartDate", "1") == "1")
			{
				sql += ", tsk_planned_start_date [planned start]";
			}
			if (btnet.Util.get_setting("ShowTaskActualStartDate", "1") == "1")
			{
				sql += ", tsk_actual_start_date [actual start]";
			}

			if (btnet.Util.get_setting("ShowTaskPlannedEndDate", "1") == "1")
			{
				sql += ", tsk_planned_end_date [planned end]";
			}
			if (btnet.Util.get_setting("ShowTaskActualEndDate", "1") == "1")
			{
				sql += ", tsk_actual_end_date [actual end]";
			}

			if (btnet.Util.get_setting("ShowTaskPlannedDuration", "1") == "1")
			{
				sql += ", tsk_planned_duration [planned<br>duration]";
			}
			if (btnet.Util.get_setting("ShowTaskActualDuration", "1") == "1")
			{
				sql += ", tsk_actual_duration  [actual<br>duration]";
			}


			if (btnet.Util.get_setting("ShowTaskDurationUnits", "1") == "1")
			{
				sql += ", tsk_duration_units [duration<br>units]";
			}

			if (btnet.Util.get_setting("ShowTaskPercentComplete", "1") == "1")
			{
				sql += ", tsk_percent_complete [percent<br>complete]";
			}

			if (btnet.Util.get_setting("ShowTaskStatus", "1") == "1")
			{
				sql += ", st_name  [status]";
			}

			if (btnet.Util.get_setting("ShowTaskSortSequence", "1") == "1")
			{
				sql += ", tsk_sort_sequence  [seq]";
			}

			sql += @"
				from bug_tasks 
				left outer join statuses on tsk_status = st_id
				left outer join users on tsk_assigned_to_user = us_id
				where tsk_bug = $bugid 
				order by tsk_sort_sequence, tsk_id";

			sql = sql.Replace("$bugid", Convert.ToString(bugid));
			sql = sql.Replace("$ses", ses);

			ds = DbUtil.get_dataset(sql);

		}
	}
}
