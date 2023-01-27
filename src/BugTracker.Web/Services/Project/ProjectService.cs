using btnet;
using BugTracker.Web.Models;
using BugTracker.Web.Models.Project;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

        public DataView GetProjectListForSelf(int userId, int defaultPermissionLevel)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("select pj_id, pj_name, isnull(pu_auto_subscribe,0) [pu_auto_subscribe]");
			sql.AppendLine("from projects");
			sql.AppendLine("left outer join project_user_xref on pj_id = pu_project and @userId = pu_user");
			sql.AppendLine("where isnull(pu_permission_level,@defaultPermission) <> 0");
            sql.AppendLine("order by pj_name");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@defaultPermission", defaultPermissionLevel);

            return DbUtil.get_dataview(cmd);
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

        public DataRow GetProjectDetails(int projectId)
        {
            string sql = "select pj_name from projects where pj_id = @projectId";

            SqlCommand cmd = new SqlCommand(sql);

            cmd.Parameters.AddWithValue("@projectId", projectId);

            return DbUtil.get_datarow(cmd);
        }

        public DataSet GetProjectSettings(int projectId)
        {
            int defaultPermissionLevel = Convert.ToInt32(Util.get_setting("DefaultPermissionLevel", "2"));
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("Select us_username, us_id, isnull(pu_permission_level,@permissionLevel) [pu_permission_level]");
            sql.AppendLine("from users");
            sql.AppendLine("left outer");
            sql.AppendLine("join project_user_xref on pu_user = us_id");
            sql.AppendLine("and pu_project = @projectId");
            sql.AppendLine("order by us_username;");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            cmd.Parameters.AddWithValue("@permissionLevel", defaultPermissionLevel);
            cmd.Parameters.AddWithValue("@projectId", projectId);

            return DbUtil.get_dataset(cmd);
        }

        #region ProjectUserSettings

        public void AddProjectSettings(List<int> projectIds, int userId, bool autoSubscribe)
        {
            StringBuilder sql = new StringBuilder();

			sql.AppendLine("INSERT INTO project_user_xref (pu_project, pu_user, pu_auto_subscribe)");
			sql.AppendLine("SELECT pj_id, @userId, @autoSubscribe");
			sql.AppendLine("FROM projects");
			sql.AppendLine(string.Format("WHERE pj_id in ({0})", string.Join(",", projectIds.Select(n => "@prm"+n).ToArray())));
            sql.AppendLine("AND pj_id NOT IN (SELECT pu_project FROM project_user_xref WHERE pu_user = @userId);");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            foreach(int n in projectIds)
            {
                cmd.Parameters.AddWithValue("@prm"+n, n);
            }

            cmd.Parameters.AddWithValue("@autoSubscribe", autoSubscribe);
            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }

        public void SetDefaultProjectSettings(int userId, int permissionLevel)
        {
            StringBuilder sql = new StringBuilder();
            
		    sql.AppendLine("update project_user_xref");
		    sql.AppendLine("set pu_auto_subscribe = 0,");
		    sql.AppendLine("pu_admin = 0,");
		    sql.AppendLine("pu_permission_level = @permissionLevel");
		    sql.AppendLine("where pu_user = @userId;");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@permissionLevel", permissionLevel);

            DbUtil.execute_nonquery(cmd);
        }

        public void UpdateAutoSubscribe(List<int> projectIds, int userId)
        {
            StringBuilder sql = new StringBuilder();
            
			sql.AppendLine("UPDATE project_user_xref");
			sql.AppendLine("SET pu_auto_subscribe = 1");
			sql.AppendLine("WHERE pu_user = @userId");
            sql.AppendLine(string.Format("AND pu_project IN ({0});", string.Join(",", projectIds.Select(n => "@prm"+n).ToArray())));

            SqlCommand cmd = new SqlCommand(sql.ToString());

            foreach (int n in projectIds)
            {
                cmd.Parameters.AddWithValue("@prm"+n, n);
            }

            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }

        public void UpdateProjectAdmins(List<int> projectIds, int userId)
        {
            StringBuilder sql = new StringBuilder();
            
			sql.AppendLine("UPDATE project_user_xref");
			sql.AppendLine("SET pu_admin = 1");
			sql.AppendLine("WHERE pu_user = @userId");
            sql.AppendLine(string.Format("AND pu_project IN ({0});", string.Join(",", projectIds.Select(n => "@prm"+n).ToArray())));

            SqlCommand cmd = new SqlCommand(sql.ToString());

            foreach (int n in projectIds)
            {
                cmd.Parameters.AddWithValue("@prm"+n, n);
            }

            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }

        public void UpdateProjectPermissionLevels(List<int> projectIds, int permissionLevel, int userId)
        {
            StringBuilder sql = new StringBuilder();
            
			sql.AppendLine("UPDATE project_user_xref");
			sql.AppendLine("SET pu_permission_level = @permissionLevel");
			sql.AppendLine("WHERE pu_user = @userId");
            sql.AppendLine(string.Format("AND pu_project IN ({0});", string.Join(",", projectIds.Select(n => "@prm"+n).ToArray())));

            SqlCommand cmd = new SqlCommand(sql.ToString());

            foreach (int n in projectIds)
            {
                cmd.Parameters.AddWithValue("@prm"+n, n);
            }

            cmd.Parameters.AddWithValue("@permissionLevel", permissionLevel);
            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Takes a list of users and updates their permissions for a specific project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="projectPermissions"></param>
        public void UpdateProjectPermissionLevelsByProject(int projectId, List<ProjectUserPermissions> projectPermissions)
        {
            foreach (var user in projectPermissions)
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("if exists (select * from project_user_xref where pu_user = @userId and pu_project = @projectId)");
                sql.AppendLine("update project_user_xref set pu_permission_level = @permissionLevel");
                sql.AppendLine("where pu_user = @userId and pu_project = @projectId");
                sql.AppendLine("else");
                sql.AppendLine("insert into project_user_xref (pu_user, pu_project, pu_permission_level)");
                sql.AppendLine("values (@userId, @projectId, @permissionLevel);");

                SqlCommand cmd = new SqlCommand(sql.ToString());

                cmd.Parameters.AddWithValue("@userId", user.UserId);
                cmd.Parameters.AddWithValue("@projectId", projectId);
                cmd.Parameters.AddWithValue("@permissionLevel", user.PermissionLevel);

                DbUtil.execute_nonquery(cmd);
            }
        }

        #endregion
    }
}