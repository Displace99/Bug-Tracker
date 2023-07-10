using btnet;
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
    }
}