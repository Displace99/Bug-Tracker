using btnet;
using BugTracker.Web.Models.Task;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using static Lucene.Net.Index.CheckIndex;

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
					'<a href=edit_task.aspx?bugid=$bugId&id=' + convert(varchar,tsk_id) + '>edit</a>'   [$no_sort_edit],
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
            sql = sql.Replace("$bugId", bugId.ToString());

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bugId", bugId);

            return DbUtil.get_dataset(cmd);
        }

        public DataSet GetAllTasks(Security security, int bugid)
        {
            string sql = "select ";

            if (bugid == 0)
            {
                sql += @"
                    bg_id as [id], 
                    bg_short_desc as [description], 
                    pj_name as [project], 
                    ct_name as [category], 
                    bug_statuses.st_name as [status],  
                    bug_users.us_username as [assigned to],";
            }

            sql += "tsk_id [task<br>id], tsk_description [task<br>description] ";

            if (Util.get_setting("ShowTaskAssignedTo", "1") == "1")
            {
                sql += ", task_users.us_username [task<br>assigned to]";
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
                sql += ", task_statuses.st_name  [task<br>status]";
            }

            if (Util.get_setting("ShowTaskSortSequence", "1") == "1")
            {
                sql += ", tsk_sort_sequence  [seq]";
            }

            sql += @"
                from bug_tasks 
                inner join bugs on tsk_bug = bg_id
                left outer join projects on bg_project = pj_id
                left outer join categories on bg_category = ct_id
                left outer join statuses bug_statuses on bg_status = bug_statuses.st_id
                left outer join statuses task_statuses on tsk_status = task_statuses.st_id
                left outer join users bug_users on bg_assigned_to_user = bug_users.us_id
                left outer join users task_users on tsk_assigned_to_user = task_users.us_id
                where tsk_bug in 
                (";

            if (bugid == 0)
            {
                sql += Util.alter_sql_per_project_permissions("select bg_id from bugs", security);
            }
            else
            {
                sql += Convert.ToString(bugid);
            }
            sql += @") order by tsk_sort_sequence, tsk_id";

            return DbUtil.get_dataset(sql);
        }

        /// <summary>
        /// Returns details of a specific task
        /// </summary>
        /// <param name="taskId">Task Id to search by</param>
        /// <param name="bugId">Bug Id to search by</param>
        /// <returns>DataRow of Task details</returns>
        public DataRow GetTaskById(int taskId, int bugId)
        {
            string sql = @"select * from bug_tasks where tsk_id = @taskId and tsk_bug = @bugId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@taskId", taskId); 
            cmd.Parameters.AddWithValue("@bugId", bugId);
            
            return DbUtil.get_datarow(cmd);
        }

        public void InsertTask(EditTask taskModel)
        {
            string sql = @"
						insert into bug_tasks (
						tsk_bug,
						tsk_created_user,
						tsk_created_date,
						tsk_last_updated_user,
						tsk_last_updated_date,
						tsk_assigned_to_user,
						tsk_planned_start_date,
						tsk_actual_start_date,
						tsk_planned_end_date,
						tsk_actual_end_date,
						tsk_planned_duration,
						tsk_actual_duration,
						tsk_duration_units,
						tsk_percent_complete,
						tsk_status,
						tsk_sort_sequence,
						tsk_description
						)
						values (
						@bugId,
						@createdUserId,
						getdate(),
						@lastUpdatedUserId,
						getdate(),
						@assignedToUserId,
						@plannedStartDate,
						@actualStartDate,
						@plannedEndDate,
						@actualEndDate,
						@plannedDuration,
						@actualDuration,
						@durationUnits,
						@percentComplete,
						@status,
						@sortSeq,
						@description
						)

						declare @tsk_id int
						select @tsk_id = scope_identity()

						insert into bug_posts
						(bp_bug, bp_user, bp_date, bp_comment, bp_type)
						values(@bugId, @lastUpdatedUserId, getdate(), N'added task ' + convert(varchar, @tsk_id), 'update')";

            SqlCommand cmd = new SqlCommand(sql); 
            cmd.Parameters.AddWithValue("@bugId", taskModel.BugId);
            cmd.Parameters.AddWithValue("@createdUserId", taskModel.CreatedBy);
            cmd.Parameters.AddWithValue("@lastUpdatedUserId", taskModel.LastUpdatedBy);
            cmd.Parameters.AddWithValue("@assignedToUserId", taskModel.AssignedTo.HasValue ? (object)taskModel.AssignedTo.Value : DBNull.Value);

            cmd.Parameters.AddWithValue("@plannedStartDate", taskModel.PlannedStartDate.HasValue ? (object)taskModel.PlannedStartDate.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@actualStartDate", taskModel.ActualStartDate.HasValue ? (object)taskModel.ActualStartDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@plannedEndDate", taskModel.PlannedEndDate.HasValue ? (object)taskModel.PlannedEndDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@actualEndDate", taskModel.ActualEndDate.HasValue ? (object)taskModel.ActualEndDate.Value : DBNull.Value);

            cmd.Parameters.AddWithValue("@plannedDuration", taskModel.PlannedDuration.HasValue ? (object)taskModel.PlannedDuration.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@actualDuration", taskModel.ActualDuration.HasValue ? (object)taskModel.ActualDuration.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@percentComplete", taskModel.PercentComplete.HasValue ? (object)taskModel.PercentComplete.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@status", taskModel.Status);
            cmd.Parameters.AddWithValue("@sortSeq", taskModel.SortSequence.HasValue ? (object)taskModel.SortSequence.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@description", taskModel.Description);
            cmd.Parameters.AddWithValue("@durationUnits", taskModel.DurationUnits);

            DbUtil.execute_nonquery(cmd);
        }
    }
}