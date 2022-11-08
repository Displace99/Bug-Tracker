using btnet;
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

        public DataSet GetAllProjectList()
        {
            string sql =
                @"select
					pj_id [id],
					'<a href=edit_project.aspx?&id=' + convert(varchar,pj_id) + '>edit</a>' [$no_sort_edit],
					'<a href=edit_user_permissions2.aspx?projects=y&id=' + convert(varchar,pj_id) + '>permissions</a>' [$no_sort_per user<br>permissions],
					'<a href=delete_project.aspx?id=' + convert(varchar,pj_id) + '>delete</a>' [$no_sort_delete],
					pj_name [project],
					case when pj_active = 1 then 'Y' else 'N' end [active],
					us_username [default user],
					case when isnull(pj_auto_assign_default_user,0) = 1 then 'Y' else 'N' end [auto assign<br>default user],
					case when isnull(pj_auto_subscribe_default_user,0) = 1 then 'Y' else 'N' end [auto subscribe<br>default user],
					case when isnull(pj_enable_pop3,0) = 1 then 'Y' else 'N' end [receive items<br>via pop3],
					pj_pop3_username [pop3 username],
					pj_pop3_email_from [from email addressl],
					case when pj_default = 1 then 'Y' else 'N' end [default]
					from projects
					left outer join users on us_id = pj_default_user
					order by pj_name";
            
            return DbUtil.get_dataset(sql);
        }
    }
}