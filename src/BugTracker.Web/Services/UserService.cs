using btnet;
using BugTracker.Web.Models;
using BugTracker.Web.Models.Registration;
using BugTracker.Web.Models.User;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using static SupportClass;

namespace BugTracker.Web.Services
{
    public class UserService
    {
        /// <summary>
        /// Returns a list of all Users in the system. Used in dropdown lists
        /// </summary>
        /// <returns></returns>
        public DataSet GetUserList()
        {
            string sql = "SELECT us_id, us_username FROM users ORDER BY us_username";

            return DbUtil.get_dataset(sql);
        }

        /// <summary>
        /// Returns a DataSet of all users in the system. Used by Admin users only
        /// </summary>
        /// <param name="filterText">Text to filter by user name</param>
        /// <param name="showInactiveUsers">Whether to show inactive users or not.</param>
        /// <returns>DataSet of Users</returns>
        public DataSet GetUserListForAdmins(string filterText, bool showInactiveUsers) 
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT pu_user");
            sql.AppendLine("INTO #t FROM project_user_xref");
            sql.AppendLine("WHERE pu_admin = 1;");
            
            sql.AppendLine("SELECT u.us_id [id],");
            sql.AppendLine("'<a href=edit_user.aspx?id=' + convert(varchar,u.us_id) + '>edit</a>' [$no_sort_edit],");
            sql.AppendLine("'<a href=edit_user.aspx?copy=y&id=' + convert(varchar,u.us_id) + '>copy</a>' [$no_sort_add<br>like<br>this],"); 
            sql.AppendLine("'<a href=delete_user.aspx?id=' + convert(varchar,u.us_id) + '>delete</a>' [$no_sort_delete],");
            sql.AppendLine("u.us_username [username],");
            sql.AppendLine("isnull(u.us_firstname,'') + ' ' + isnull(u.us_lastname,'') [name],");
            sql.AppendLine("'<a sort=''' + og_name + ''' href=edit_org.aspx?id=' + convert(varchar,og_id) + '>' + og_name + '</a>' [org],");
            sql.AppendLine("isnull(u.us_email,'') [email],");
            sql.AppendLine("CASE WHEN u.us_admin = 1 THEN 'Y' ELSE 'N' END [admin],");
            sql.AppendLine("CASE WHEN pu_user is null THEN 'N' ELSE 'Y' END [project<br>admin],");
            sql.AppendLine("CASE WHEN u.us_active = 1 THEN 'Y' ELSE 'N' END [active],");
            sql.AppendLine("CASE WHEN og_external_user = 1 THEN 'Y' else 'N' END [external],");
            sql.AppendLine("isnull(pj_name,'') [forced<br>project],");
            sql.AppendLine("isnull(qu_desc,'') [default query],");
            sql.AppendLine("CASE WHEN u.us_enable_notifications = 1 THEN 'Y' ELSE 'N' END [notif-<br>ications],");
            sql.AppendLine("u.us_most_recent_login_datetime [most recent login],");
            sql.AppendLine("u2.us_username [created<br>by]");
            sql.AppendLine("FROM users u");
            sql.AppendLine("INNER JOIN orgs ON u.us_org = og_id");
            sql.AppendLine("LEFT OUTER JOIN queries ON u.us_default_query = qu_id");
            sql.AppendLine("LEFT OUTER JOIN projects ON u.us_forced_project = pj_id");
            sql.AppendLine("LEFT OUTER JOIN users u2 ON u.us_created_user = u2.us_id");
            sql.AppendLine("LEFT OUTER JOIN #t ON u.us_id = pu_user");
            sql.AppendLine("WHERE u.us_active = 1");

            if (showInactiveUsers)
            {
                sql.AppendLine("OR u.us_active = 0");
            }

            if (!string.IsNullOrEmpty(filterText))
            {
                sql.AppendLine("AND u.us_username LIKE @filterText");
            }

            sql.AppendLine("ORDER BY u.us_username;");
            
            sql.AppendLine("DROP TABLE #t");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();

            if (!string.IsNullOrEmpty(filterText))
            {
                cmd.Parameters.AddWithValue("@filterText", filterText + "%");
            }

            return DbUtil.get_dataset(cmd);
        }

        /// <summary>
        /// Returns a DataSet of all users in the system. Used by Non-Admin users only
        /// </summary>
        /// <param name="filterText">Text to filter by user name</param>
        /// <param name="showInactiveUsers">Whether to show inactive users or not.</param>
        /// <param name="userId">The UserId to filter results from</param>
        /// <returns>DataSet of Users</returns>
        public DataSet GetUserListForNonAdmins(string filterText, bool showInactiveUsers, int userId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT pu_user");
            sql.AppendLine("INTO #t FROM project_user_xref");
            sql.AppendLine("WHERE pu_admin = 1;");

            sql.AppendLine("select u.us_id [id],");
            sql.AppendLine("'<a href=edit_user.aspx?id=' + convert(varchar,u.us_id) + '>edit</a>' [$no_sort_edit],");
            sql.AppendLine("'<a href=edit_user.aspx?copy=y&id=' + convert(varchar,u.us_id) + '>copy</a>' [$no_sort_add<br>like<br>this],");
            sql.AppendLine("'<a href=delete_user.aspx?id=' + convert(varchar,u.us_id) + '>delete</a>' [$no_sort_delete],");

            sql.AppendLine("u.us_username [username],");
            sql.AppendLine("isnull(u.us_firstname,'') + ' ' + isnull(u.us_lastname,'') [name],");
            sql.AppendLine("og_name [org],");
            sql.AppendLine("isnull(u.us_email,'') [email],");
            sql.AppendLine("case when u.us_admin = 1 then 'Y' else 'N' end [admin],");
            sql.AppendLine("case when pu_user is null then 'N' else 'Y' end [project<br>admin],");
            sql.AppendLine("case when u.us_active = 1 then 'Y' else 'N' end [active],");
            sql.AppendLine("case when og_external_user = 1 then 'Y' else 'N' end [external],");
            sql.AppendLine("isnull(pj_name,'') [forced<br>project],");
            sql.AppendLine("isnull(qu_desc,'') [default query],");
            sql.AppendLine("case when u.us_enable_notifications = 1 then 'Y' else 'N' end [notif-<br>ications],");
            sql.AppendLine("u.us_most_recent_login_datetime [most recent login]");
            sql.AppendLine("from users u");
            sql.AppendLine("inner join orgs on us_org = og_id");
            sql.AppendLine("left outer join queries on us_default_query = qu_id");
            sql.AppendLine("left outer join projects on us_forced_project = pj_id ");
            sql.AppendLine("left outer join #t on us_id = pu_user");
            sql.AppendLine("where us_created_user = @Id");
            sql.AppendLine("AND u.us_active = 1");

            if (showInactiveUsers)
            {
                sql.AppendLine("OR u.us_active = 0");
            }

            if (!string.IsNullOrEmpty(filterText))
            {
                sql.AppendLine("AND u.us_username LIKE @filterText");
            }

            sql.AppendLine("order by us_username;");

            sql.AppendLine("drop table #t");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();

            if (!string.IsNullOrEmpty(filterText)) 
            { 
                cmd.Parameters.AddWithValue("@filterText", filterText + "%");
            }

            cmd.Parameters.AddWithValue("@Id", userId);

            return DbUtil.get_dataset(cmd);
        }

        /// <summary>
        /// Gets a user by their Id
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <returns>DataRow</returns>
        public DataRow GetUserById(int id) 
        {
            string sql = "SELECT us_username, us_created_user, us_admin FROM users WHERE us_id = @Id";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@Id", id);

            return DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Checks to see if the username already exists in the system
        /// </summary>
        /// <param name="userName"></param>
        /// /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsUserNameUnique(string userName, int userId)
        {
            bool isUnique = false;
            var dr = GetUserByUserName(userName);

            if (dr == null)
            {
                isUnique = true;
            }
            else
            {
                if (Convert.ToInt32(dr["us_id"]) == userId)
                {
                    isUnique = true;
                }
            }

            return isUnique;
        }

        /// <summary>
        /// Gets user by username
        /// </summary>
        /// <param name="userName">Username of person you are search for</param>
        /// <returns>DataRow</returns>
        public DataRow GetUserByUserName(string userName)
        {
            string sql = "select us_id from users where us_username = @userName";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@userName", userName);

            return btnet.DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email of person you are searching for</param>
        /// <returns></returns>
        public DataRow GetUserByEmail(string email)
        {
            string sql = "select us_id from users where us_email = @email";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@email", email);

            return btnet.DbUtil.get_datarow(cmd);
        }

        public DataRow GetUserDetailsById(int userId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT");
            sql.AppendLine("us_username,");
            sql.AppendLine("isnull(us_firstname, '')[us_firstname],");
			sql.AppendLine("isnull(us_lastname, '')[us_lastname],");
			sql.AppendLine("isnull(us_bugs_per_page, 10)[us_bugs_per_page],");
			sql.AppendLine("us_use_fckeditor,");
			sql.AppendLine("us_enable_bug_list_popups,");
			sql.AppendLine("isnull(us_email, '')[us_email],");
			sql.AppendLine("us_active,");
			sql.AppendLine("us_admin,");
			sql.AppendLine("us_enable_notifications,");
			sql.AppendLine("us_send_notifications_to_self,");
            sql.AppendLine("us_reported_notifications,");
            sql.AppendLine("us_assigned_notifications,");
            sql.AppendLine("us_subscribed_notifications,");
			sql.AppendLine("us_auto_subscribe,");
			sql.AppendLine("us_auto_subscribe_own_bugs,");
			sql.AppendLine("us_auto_subscribe_reported_bugs,");
			sql.AppendLine("us_default_query,");
			sql.AppendLine("us_org,");
			sql.AppendLine("isnull(us_signature, '')[us_signature],");
			sql.AppendLine("isnull(us_forced_project, 0)[us_forced_project],");
			sql.AppendLine("us_created_user");
            sql.AppendLine("FROM users");
            sql.AppendLine("WHERE us_id = @userId");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            cmd.Parameters.AddWithValue("@userId", userId);

            return DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Checks to see if the current user is an admin in any project
        /// </summary>
        /// <param name="userId">Id of the user to check</param>
        /// <returns>True if they are an admin, otherwise false</returns>
        public bool IsUserProjectAdmin(int userId)
        {
            StringBuilder sql = new StringBuilder();
            
            sql.AppendLine("select pu_project");
			sql.AppendLine("from project_user_xref");
			sql.AppendLine("where pu_user = @Id");
            sql.AppendLine("and pu_admin = 1");
            
            SqlCommand cmd = new SqlCommand(sql.ToString());
            cmd.Parameters.AddWithValue("@Id", userId);
            
            var ds_projects = DbUtil.get_dataset(cmd);

            return ds_projects.Tables[0].Rows.Count == 0;
        }

        public DataSet GetProjectPermissionsForProjectAdmin(int currentUserId, int editUserId, int permissionLevel)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("select pj_id, pj_name,");
            sql.AppendLine("isnull(a.pu_permission_level,@permissionLevel) [pu_permission_level],");
			sql.AppendLine("isnull(a.pu_auto_subscribe, 0)[pu_auto_subscribe],");
			sql.AppendLine("isnull(a.pu_admin, 0)[pu_admin]");
            sql.AppendLine("from projects");
            sql.AppendLine("inner join project_user_xref project_admin on pj_id = project_admin.pu_project");
            sql.AppendLine("and project_admin.pu_user = @currentUserId");
            sql.AppendLine("and project_admin.pu_admin = 1");
            sql.AppendLine("left outer join project_user_xref a on pj_id = a.pu_project");
            sql.AppendLine("and a.pu_user = @editUserId");
            sql.AppendLine("order by pj_name; ");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            cmd.Parameters.AddWithValue("@permissionLevel", permissionLevel);
            cmd.Parameters.AddWithValue("@currentUserId", currentUserId);
            cmd.Parameters.AddWithValue("@editUserId", editUserId);

            return DbUtil.get_dataset(cmd);
        }

        public DataSet GetProjectPermissionsForSiteAdmin(int editUserId, int permissionLevel)
        {
            StringBuilder sql = new StringBuilder();
   
            sql.AppendLine("SELECT pj_id, pj_name,");
            sql.AppendLine("isnull(pu_permission_level,@permissionLevel) [pu_permission_level],");
			sql.AppendLine("isnull(pu_auto_subscribe, 0)[pu_auto_subscribe],");
			sql.AppendLine("isnull(pu_admin, 0)[pu_admin]");
            sql.AppendLine("FROM projects");
            sql.AppendLine("LEFT OUTER JOIN project_user_xref ON pj_id = pu_project");
            sql.AppendLine("AND pu_user = @editUserId");
            sql.AppendLine("ORDER BY pj_name;");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            cmd.Parameters.AddWithValue("@permissionLevel", permissionLevel);
            cmd.Parameters.AddWithValue("@editUserId", editUserId);

            return DbUtil.get_dataset(cmd);
        }

        /// <summary>
        /// Get pending user by username
        /// </summary>
        /// <param name="userName">Username of person you are searching for</param>
        /// <returns></returns>
        public DataRow GetPendingUserByUserName(string userName)
        {
            string sql = "select el_id from emailed_links where el_username = @username";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@username", userName);

            return btnet.DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Get pending user by email
        /// </summary>
        /// <param name="email">Email of person youare searching for</param>
        /// <returns></returns>
        public DataRow GetPendingUserByEmail(string email)
        {
            string sql = "select el_id from emailed_links where el_email = @email";
            
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@email", email);

            return btnet.DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Adds a user to the Registration Table
        /// </summary>
        /// <param name="model">Registered User</param>
        /// <returns>Id from the registration table</returns>
        public string AddRegisteredUser(RegisterVM model)
        {
            string guid = Guid.NewGuid().ToString();

            string salt = Util.GenerateRandomString();
            string hashedPassword = EncryptionService.HashString(model.Password, Convert.ToString(salt));

            StringBuilder sql = new StringBuilder();
            sql.Append("insert into emailed_links ");
            sql.Append("(el_id, el_date, el_email, el_action, el_username, el_salt, el_password, el_firstname, el_lastname) ");
            sql.Append("values (@Id, getdate(), @email, @register, @username, @salt, @password, @firstname, @lastname)");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("@Id", guid);
            cmd.Parameters.AddWithValue("@email", model.Email);
            cmd.Parameters.AddWithValue("@register", "register");
            cmd.Parameters.AddWithValue("@username", model.UserName);
            cmd.Parameters.AddWithValue("@salt", salt);
            cmd.Parameters.AddWithValue("@password", hashedPassword);
            cmd.Parameters.AddWithValue("@firstname", model.FirstName);
            cmd.Parameters.AddWithValue("@lastname", model.LastName);

            DbUtil.execute_nonquery(cmd);

            return guid;
        }

        public RegisteredUser GetRegisteredUser(string linkId)
        {
            var dr = GetEmailedLink(linkId);

            RegisteredUser user = null;

            if(dr != null)
            {
                user = new RegisteredUser();

                user.Id = linkId;
                user.UserName = (string)dr["el_username"];
                user.Email = (string)dr["el_email"];
                user.FirstName = (string)dr["el_firstname"];
                user.LastName = (string)dr["el_lastname"];
                user.Salt = (string)dr["el_salt"];
                user.Password = (string)dr["el_password"];

            }

            return user;
        }

        public RegisteredUser GetPasswordResetUser(string linkId)
        {
            var dr = GetEmailedLink(linkId);

            RegisteredUser user = null;

            if (dr != null)
            {
                user = new RegisteredUser();

                user.Id = linkId;
                user.Email = (string)dr["el_email"];
                user.UserId = (int)dr["el_user_id"];

            }

            return user;
        }

        private DataRow GetEmailedLink(string linkId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("declare @expiration datetime");
            sql.AppendLine("set @expiration = dateadd(n,-@minutes,getdate())");
            sql.AppendLine("select * from emailed_links where @expiration < el_date AND el_id = @linkId");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();

            cmd.Parameters.AddWithValue("@minutes", int.Parse(Util.get_setting("RegistrationExpiration", "20")));
            cmd.Parameters.AddWithValue("@linkId", linkId);

            return DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Removes a user and all associated records.
        /// This is permanent and will result in loss of data.
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteUser(int userId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("delete from emailed_links where el_username in (select us_username from users where us_id = @Id");
            sql.AppendLine("delete users where us_id = @Id");
            sql.AppendLine("delete project_user_xref where pu_user = @Id");
            sql.AppendLine("delete bug_subscriptions where bs_user = @Id");
            sql.AppendLine("delete bug_user where bu_user = @Id");
            sql.AppendLine("delete queries where qu_user = @Id");
            sql.AppendLine("delete queued_notifications where qn_user = @Id");
            sql.AppendLine("delete dashboard_items where ds_user = @Id");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();

            cmd.Parameters.AddWithValue("@Id", userId);

            DbUtil.execute_nonquery(cmd);
        }

        public int AddNewUser(NewUser user)
        {
            StringBuilder createUserSql = new StringBuilder();
            createUserSql.AppendLine("insert into users (us_username, us_firstname, us_lastname, ");
            createUserSql.Append("us_bugs_per_page, us_use_fckeditor, us_enable_bug_list_popups, ");
            createUserSql.Append("us_email, us_active, us_admin, us_enable_notifications, us_send_notifications_to_self, ");
            createUserSql.Append("us_reported_notifications, us_assigned_notifications, ");
            createUserSql.Append("us_subscribed_notifications, us_auto_subscribe, us_auto_subscribe_own_bugs, ");
            createUserSql.Append("us_auto_subscribe_reported_bugs, us_default_query, us_org, us_signature, ");
            createUserSql.Append("us_forced_project, us_created_user)");
            createUserSql.AppendLine("values (@username, @firstName, @lastName, @bugsPerPage, @fckEditor, @popups, ");
            createUserSql.Append("@email, @active, @isAdmin, @notifications, @selfNotifications, @reportedNotifications, ");
            createUserSql.Append("@assignedNotifications, @subscribedNotifications, @autoSubscribe, @subscribeOwnBugs, ");
            createUserSql.Append("@subscribeReportedBugs, @defaultQuery, @org, @signature, @forcedProject, @createdBy);");
            createUserSql.AppendLine("select scope_identity()");

            SqlCommand createUserCmd = new SqlCommand();
            createUserCmd.CommandText = createUserSql.ToString();

            createUserCmd.Parameters.AddWithValue("@username", user.UserName);
            createUserCmd.Parameters.AddWithValue("@firstName", user.FirstName);
            createUserCmd.Parameters.AddWithValue("@lastName", user.LastName);
            createUserCmd.Parameters.AddWithValue("@bugsPerPage", user.BugsPerPage);
            createUserCmd.Parameters.AddWithValue("@fckEditor", user.UseFckEditor);
            createUserCmd.Parameters.AddWithValue("@popups", user.EnablePopups);
            createUserCmd.Parameters.AddWithValue("@email", user.Email);
            createUserCmd.Parameters.AddWithValue("@active", user.IsActive);
            createUserCmd.Parameters.AddWithValue("@notifications", user.EnableNotifications);
            createUserCmd.Parameters.AddWithValue("@selfNotifications", user.SendToSelf);
            createUserCmd.Parameters.AddWithValue("@reportedNotifications", user.ReportedNotifications);
            createUserCmd.Parameters.AddWithValue("@assignedNotifications", user.AssignedNotifications);
            createUserCmd.Parameters.AddWithValue("@subscribedNotifications", user.SubscribedNotifications);
            createUserCmd.Parameters.AddWithValue("@autoSubscribe", user.AutoSubscribe);
            createUserCmd.Parameters.AddWithValue("@subscribeOwnBugs", user.AutoSubscribeOwn);
            createUserCmd.Parameters.AddWithValue("@subscribeReportedBugs", user.AutoSubscribeReported);
            createUserCmd.Parameters.AddWithValue("@defaultQuery", user.DefaultQueryId);
            createUserCmd.Parameters.AddWithValue("@org", user.OrginizationId);
            createUserCmd.Parameters.AddWithValue("@signature", user.Signature);
            createUserCmd.Parameters.AddWithValue("@forcedProject", user.ForcedProjectId);
            createUserCmd.Parameters.AddWithValue("@createdBy", user.CreatedById);
            createUserCmd.Parameters.AddWithValue("@isAdmin", user.IsAdmin);

            int userId = Convert.ToInt32(btnet.DbUtil.execute_scalar(createUserCmd));

            // now encrypt the password and update the db
            Util.UpdateUserPassword(userId, user.Password);

            return userId;
        }

        public void UpdateUser(NewUser user)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("UPDATE users SET");
            sql.AppendLine("us_username = @userName,");
            sql.AppendLine("us_firstname = @firstName,");
            sql.AppendLine("us_lastname = @lastName,");
            sql.AppendLine("us_bugs_per_page = @bugsPerPage,");
            sql.AppendLine("us_use_fckeditor = @fckEditor,");
            sql.AppendLine("us_enable_bug_list_popups = @popups,");
            sql.AppendLine("us_email = @email,");
            sql.AppendLine("us_active = @active,");
            sql.AppendLine("us_admin = @isAdmin,");
            sql.AppendLine("us_enable_notifications = @notifications,");
            sql.AppendLine("us_send_notifications_to_self = @selfNotifications,");
            sql.AppendLine("us_reported_notifications = @reportedNotifications,");
            sql.AppendLine("us_assigned_notifications = @assignedNotifications,");
            sql.AppendLine("us_subscribed_notifications = @subscribedNotifications,");
            sql.AppendLine("us_auto_subscribe = @autoSubscribe,");
            sql.AppendLine("us_auto_subscribe_own_bugs = @subscribeOwnBugs,");
            sql.AppendLine("us_auto_subscribe_reported_bugs = @subscribeReportedBugs,");
            sql.AppendLine("us_default_query = @defaultQuery,");
            sql.AppendLine("us_org = @org,");
            sql.AppendLine("us_signature = @signature,");
            sql.AppendLine("us_forced_project = @forcedProject");
            sql.AppendLine("WHERE us_id = @Id");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            cmd.Parameters.AddWithValue("@username", user.UserName);
            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
            cmd.Parameters.AddWithValue("@lastName", user.LastName);
            cmd.Parameters.AddWithValue("@bugsPerPage", user.BugsPerPage);
            cmd.Parameters.AddWithValue("@fckEditor", user.UseFckEditor);
            cmd.Parameters.AddWithValue("@popups", user.EnablePopups);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@active", user.IsActive);
            cmd.Parameters.AddWithValue("@notifications", user.EnableNotifications);
            cmd.Parameters.AddWithValue("@selfNotifications", user.SendToSelf);
            cmd.Parameters.AddWithValue("@reportedNotifications", user.ReportedNotifications);
            cmd.Parameters.AddWithValue("@assignedNotifications", user.AssignedNotifications);
            cmd.Parameters.AddWithValue("@subscribedNotifications", user.SubscribedNotifications);
            cmd.Parameters.AddWithValue("@autoSubscribe", user.AutoSubscribe);
            cmd.Parameters.AddWithValue("@subscribeOwnBugs", user.AutoSubscribeOwn);
            cmd.Parameters.AddWithValue("@subscribeReportedBugs", user.AutoSubscribeReported);
            cmd.Parameters.AddWithValue("@defaultQuery", user.DefaultQueryId);
            cmd.Parameters.AddWithValue("@org", user.OrginizationId);
            cmd.Parameters.AddWithValue("@signature", user.Signature);
            cmd.Parameters.AddWithValue("@forcedProject", user.ForcedProjectId);
            cmd.Parameters.AddWithValue("@createdBy", user.CreatedById);
            cmd.Parameters.AddWithValue("@isAdmin", user.IsAdmin);
            cmd.Parameters.AddWithValue("@Id", user.Id);

            DbUtil.execute_nonquery(cmd);

            if (!string.IsNullOrEmpty(user.Password))
            {
                Util.UpdateUserPassword(user.Id, user.Password);
            }
        }

    }
}