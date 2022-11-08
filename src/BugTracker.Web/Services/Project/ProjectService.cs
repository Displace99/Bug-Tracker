using BugTracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services.Project
{
    public class ProjectService
    {
        public List<ProjectVM> GetUserProjectList(int userId, string permissionLevel)
        {
            List<ProjectVM> projectList = new List<ProjectVM>();

            // only show projects where user has permissions
            string sql = @"select pj_id, pj_name, pj_description, pj_active
		        from projects
		        left outer join project_user_xref on pj_id = pu_project
		        and pu_user = @us
		        where isnull(pu_permission_level,@dpl) not in (0, 1)
		        order by pj_name;";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@us", userId);
            cmd.Parameters.AddWithValue("@dpl", permissionLevel);

            using (SqlDataReader reader = btnet.DbUtil.execute_reader(cmd, CommandBehavior.CloseConnection))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ProjectVM project = new ProjectVM();

                        project.Id = Convert.ToInt32(reader["pj_id"]);
                        project.Name = reader["pj_name"].ToString();
                        project.Description = reader["pj_description"].ToString();
                        bool isActive = Convert.ToBoolean(reader["pj_active"]);

                        project.Status = isActive ? "Active" : "Closed";

                        projectList.Add(project);
                    }
                }
            }

            return projectList;
        }
    }
}