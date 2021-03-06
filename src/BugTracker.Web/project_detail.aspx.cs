using btnet;
using BugTracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class project_detail : System.Web.UI.Page
    {
        protected List<ProjectBugVM> BugList;

        protected void Page_Load(object sender, EventArgs e)
        {
            BugTracker masterPage = Page.Master as BugTracker;
            masterPage.security = new btnet.Security();
            masterPage.security.check_security(HttpContext.Current, btnet.Security.ANY_USER_OK);
            masterPage.pageLink = "projects";

            lblBugLabel.Text = Util.get_setting("PluralBugLabel", "Bug");

            int projectId = 0;
            BugList = new List<ProjectBugVM>();

            if (Int32.TryParse(Request.QueryString["projectId"], out projectId))
            {
                GetBugList(projectId);
            }
            
        }

        private void GetBugList(int projectID)
        {
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
        }
    }
}