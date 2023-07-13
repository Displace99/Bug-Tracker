﻿using btnet;
using BugTracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Text;
using System.Web;

namespace BugTracker.Web.Services.Bug
{
    public class BugService
    {
        public List<ProjectBugVM> GetBugsByProject(int projectID)
        {
            List<ProjectBugVM> BugList = new List<ProjectBugVM>();
            
            string sql = @"select bg_id [id],
                bg_short_desc [desc],
                isnull(rpt.us_username,'') [reported by],
                bg_reported_date [reported on],
                isnull(lu.us_username,'') [last updated by],
                bg_last_updated_date [last updated on],
                isnull(og_name,'') [organization],
                isnull(ct_name,'') [category],
                isnull(pr_name,'') [priority],
                isnull(asg.us_username,'') [assigned to],
                isnull(st_name,'') [status]
                from bugs
                left outer join users rpt on rpt.us_id = bg_reported_user
                left outer join users lu on lu.us_id = bg_last_updated_user
                left outer join users asg on asg.us_id = bg_assigned_to_user
                left outer join orgs on og_id = bg_org
                left outer join categories on ct_id = bg_category
                left outer join priorities on pr_id = bg_priority
                left outer join statuses on st_id = bg_status
                where bg_project = @projectID
                and bg_status <> 5
                order by bg_id desc";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@projectID", projectID);

            using (SqlDataReader reader = btnet.DbUtil.execute_reader(cmd, CommandBehavior.CloseConnection))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ProjectBugVM bug = new ProjectBugVM();

                        bug.Id = Convert.ToInt32(reader["id"]);
                        bug.Description = reader["desc"].ToString();
                        bug.ReportedBy = reader["reported by"].ToString();
                        bug.ReportedDate = Convert.ToDateTime(reader["reported on"]);
                        bug.LastUpdatedBy = reader["last updated by"].ToString();
                        bug.LastUpdatedDate = Convert.ToDateTime(reader["last updated on"]);
                        bug.Organization = reader["organization"].ToString();
                        bug.Category = reader["category"].ToString();
                        bug.Priority = reader["priority"].ToString();
                        bug.AssignedTo = reader["assigned to"].ToString();
                        bug.Status = reader["status"].ToString();

                        BugList.Add(bug);
                    }
                }
            }

            return BugList;
        }

        public int GetBugCountByUserId(int userId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("declare @cnt int");
            sql.AppendLine("select @cnt = count(1) from bugs where bg_reported_user = @Id or bg_assigned_to_user = @Id");
            sql.AppendLine("if @cnt = 0");
            sql.AppendLine("begin");
            sql.AppendLine("select @cnt = count(1) from bug_posts where bp_user = @Id");
            sql.AppendLine("end");
            sql.AppendLine("select @cnt [cnt] from users where us_id = @Id");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            cmd.Parameters.AddWithValue("@Id", userId);

            DataRow dr = DbUtil.get_datarow(cmd);

            return (int)dr["cnt"];
        }

        /// <summary>
        /// Returns bug details by bug ID
        /// </summary>
        /// <param name="bugId">Id of the bug we are searching for</param>
        /// <returns></returns>
        public DataRow GetBugById(int bugId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT bg_id, bg_short_desc, bg_reported_user, bg_reported_date, bg_status, bg_priority, bg_org, bg_category, bg_project, ");
            sql.Append("bg_assigned_to_user, bg_last_updated_user, bg_last_updated_date, bg_user_defined_attribute, bg_project_custom_dropdown_value1, ");
            sql.Append("bg_project_custom_dropdown_value2, bg_project_custom_dropdown_value3, bg_tags FROM bugs WHERE bg_id = @bugId");

            SqlCommand cmd = new SqlCommand(sql.ToString());
            cmd.Parameters.AddWithValue("@bugId", bugId);

            return DbUtil.get_datarow(cmd);
        }

        public void FlagBug(int bugId, int userId, int flagged)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("if not exists (select bu_bug from bug_user where bu_bug = $bg and bu_user = @userId) ");
            sql.AppendLine("insert into bug_user (bu_bug, bu_user, bu_flag, bu_seen, bu_vote) values(@bugId, @userId, 1, 0, 0) ");
            sql.AppendLine("update bug_user set bu_flag = @flagged, bu_flag_datetime = getdate() where bu_bug = @bugId and bu_user = @userId and bu_flag <> @flagged");

            SqlCommand cmd = new SqlCommand(sql.ToString());
            cmd.Parameters.AddWithValue("@bugId", bugId);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@flagged", flagged);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Performs a mass delete of a list of bugs
        /// </summary>
        /// <param name="bugIds">List of Id's of the bugs you want to delete</param>
        public void MassDeleteBugs(List<int> bugIds) 
        {
            string paramNames = string.Join(",", bugIds.Select(n => "@prm" + n).ToArray());

            StringBuilder sql = new StringBuilder();
            sql.Append(string.Format("delete bug_post_attachments from bug_post_attachments inner join bug_posts on bug_post_attachments.bpa_post = bug_posts.bp_id where bug_posts.bp_bug in ({0});", paramNames));
            sql.AppendLine(string.Format("delete from bug_posts where bp_bug in ({0});", paramNames));
            sql.AppendLine(string.Format("delete from bug_subscriptions where bs_bug in ({0});", paramNames));
            sql.AppendLine(string.Format("delete from bug_relationships where re_bug1 in ({0});", paramNames));
            sql.AppendLine(string.Format("delete from bug_relationships where re_bug2 in ({0});", paramNames));
            sql.AppendLine(string.Format("delete from bug_user where bu_bug in ({0});", paramNames));
            sql.AppendLine(string.Format("delete from bug_tasks where tsk_bug in ({0});", paramNames));
            sql.AppendLine(string.Format("delete from bugs where bg_id in ({0});", paramNames));

            SqlCommand cmd = new SqlCommand(sql.ToString());

            foreach (int n in bugIds)
            {
                cmd.Parameters.AddWithValue("@prm" + n, n);
            }

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Mass updates bugs
        /// </summary>
        /// <param name="bugIds">List of Id's of bugs you want to update</param>
        /// <param name="projectId">Project Id to update to</param>
        /// <param name="orgId">Org Id to update to</param>
        /// <param name="categoryId">Category Id to update to</param>
        /// <param name="priorityId">Priority Id to update to</param>
        /// <param name="assignedTo">Assigned To to update to</param>
        /// <param name="reportedBy">Reported By to update to</param>
        /// <param name="statusId">Status Id to update to</param>
        public void MassUpdateBugs(List<int> bugIds, int projectId, int orgId, int categoryId, int priorityId, int assignedTo, int reportedBy, int statusId)
        {
            SqlCommand cmd = new SqlCommand();
            List<string> updateList = new List<string>();

            //Creates the set statement and parameter for each property
            if (projectId != -1)
            {
                updateList.Add(" bg_project = @projectId");
                cmd.Parameters.AddWithValue("@projectId", projectId);
            }
            if (orgId != -1)
            {
                updateList.Add(" bg_org = @orgId");
                cmd.Parameters.AddWithValue("@orgId", orgId);
            }
            if (categoryId != -1)
            {
                updateList.Add(" bg_category = @categoryId");
                cmd.Parameters.AddWithValue("@categoryId", categoryId);
            }
            if (priorityId != -1)
            {
                updateList.Add(" bg_priority = @priorityId");
                cmd.Parameters.AddWithValue("@priorityId", priorityId);
            }
            if (assignedTo != -1)
            {
                updateList.Add(" bg_assigned_to_user = @assignedToId");
                cmd.Parameters.AddWithValue("@assignedToId", assignedTo);
            }
            if (reportedBy != -1)
            {
                updateList.Add(" bg_reported_user = @reportedById");
                cmd.Parameters.AddWithValue("@reportedById", reportedBy);
            }
            if (statusId != -1)
            {
                updateList.Add(" bg_status = @statusId");
                cmd.Parameters.AddWithValue("@statusId", statusId);
            }

            //Creates a comma separated list from the set statements above
            string setStatement = string.Join(",", updateList);

            //Creates parameters from the bugs in the list
            string paramNames = string.Join(",", bugIds.Select(n => "@prm" + n).ToArray());

            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE bugs SET");
            sql.Append(setStatement);
            sql.Append(string.Format(" WHERE bg_id in ({0})", paramNames));

            cmd.CommandText = sql.ToString();

            foreach (int n in bugIds)
            {
                cmd.Parameters.AddWithValue("@prm" + n, n);
            }

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// This is a massive method that moves a bunch of information from a lot of different tables
        /// over to the new "merged" bug.
        /// </summary>
        /// <param name="fromBugId"></param>
        /// <param name="toBugId"></param>
        public void MergeBugs(int fromBugId, int intoBugId)
        {
            string sql = @"
                insert into bug_subscriptions
                (bs_bug, bs_user)
                select @intoBugId, bs_user
                from bug_subscriptions
                where bs_bug = @fromBugId
                and bs_user not in (select bs_user from bug_subscriptions where bs_bug = $into)

                insert into bug_user
                (bu_bug, bu_user, bu_flag, bu_flag_datetime, bu_seen, bu_seen_datetime, bu_vote, bu_vote_datetime)
                select @intoBugId, bu_user, bu_flag, bu_flag_datetime, bu_seen, bu_seen_datetime, bu_vote, bu_vote_datetime
                from bug_user
                where bu_bug = @fromBugId
                and bu_user not in (select bu_user from bug_user where bu_bug = @intoBugId)

                update bug_posts     set bp_bug     = @intoBugId where bp_bug = @fromBugId
                update bug_tasks     set tsk_bug    = @intoBugId where tsk_bug = @fromBugId
                update svn_revisions set svnrev_bug = @intoBugId where svnrev_bug = @fromBugId
                update hg_revisions  set hgrev_bug  = @intoBugId where hgrev_bug = @fromBugId
                update git_commits   set gitcom_bug = @intoBugId where gitcom_bug = @fromBugId
                ";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@fromBugId", fromBugId);
            cmd.Parameters.AddWithValue("@intoBugId", intoBugId);

            DbUtil.execute_nonquery(cmd);
        }
    }
}