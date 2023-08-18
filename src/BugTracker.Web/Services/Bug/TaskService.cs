using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services.Bug
{
    public class TaskService
    {
        public DataSet GetTasksByBugId(int bugId, int permissionLevel, User user, string session)
        {
            string sql = "select tsk_id [id],";
            if (permissionLevel == Security.PERMISSION_ALL && !user.is_guest && (user.is_admin || user.can_edit_tasks))
            {
                sql += @"
					'<a   href=edit_task.aspx?bugid=@bugId&id=' + convert(varchar,tsk_id) + '>edit</a>'   [$no_sort_edit],
					'<a href=delete_task.aspx?ses=$ses&bugid=@bugId&id=' + convert(varchar,tsk_id) + '>delete</a>' [$no_sort_delete],";
            }

            sql += "tsk_description [description]";

            if (Util.get_setting("ShowTaskAssignedTo", "1") == "1")
            {
                sql += ",us_username [assigned to]";
            }

            if (Util.get_setting("ShowTaskPlannedStartDate", "1") == "1")
            {
                sql += ", tsk_planned_start_date [planned start]";
            }
            if (Util.get_setting("ShowTaskActualStartDate", "1") == "1")
            {
                sql += ", tsk_actual_start_date [actual start]";
            }

            if (Util.get_setting("ShowTaskPlannedEndDate", "1") == "1")
            {
                sql += ", tsk_planned_end_date [planned end]";
            }
            if (Util.get_setting("ShowTaskActualEndDate", "1") == "1")
            {
                sql += ", tsk_actual_end_date [actual end]";
            }

            if (Util.get_setting("ShowTaskPlannedDuration", "1") == "1")
            {
                sql += ", tsk_planned_duration [planned<br>duration]";
            }
            if (Util.get_setting("ShowTaskActualDuration", "1") == "1")
            {
                sql += ", tsk_actual_duration  [actual<br>duration]";
            }

            if (Util.get_setting("ShowTaskDurationUnits", "1") == "1")
            {
                sql += ", tsk_duration_units [duration<br>units]";
            }

            if (Util.get_setting("ShowTaskPercentComplete", "1") == "1")
            {
                sql += ", tsk_percent_complete [percent<br>complete]";
            }

            if (Util.get_setting("ShowTaskStatus", "1") == "1")
            {
                sql += ", st_name  [status]";
            }

            if (Util.get_setting("ShowTaskSortSequence", "1") == "1")
            {
                sql += ", tsk_sort_sequence  [seq]";
            }

            sql += @"
				from bug_tasks 
				left outer join statuses on tsk_status = st_id
				left outer join users on tsk_assigned_to_user = us_id
				where tsk_bug = @bugId 
				order by tsk_sort_sequence, tsk_id";

            sql = sql.Replace("$ses", session);

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bugId", bugId);

            return DbUtil.get_dataset(cmd);
        }
    }
}