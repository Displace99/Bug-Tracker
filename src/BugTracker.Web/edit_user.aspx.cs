using btnet;
using BugTracker.Web.Services;
using BugTracker.Web.Services.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class edit_user : Page
    {
        int id;
        string sql;
        bool copy = false;

        public Security security;
        private UserService _userService = new UserService();
        private QueryService _queryService = new QueryService();
        

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN_OR_PROJECT_ADMIN);
            int currentUserId = security.user.usid;
            bool isUserAdmin = security.user.is_admin;
            int permissionLevel = Convert.ToInt32(Util.get_setting("DefaultPermissionLevel", "2"));
            
            DataSet ProjectList = new DataSet();

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit user";

            if (!isUserAdmin)
            {
                // Check if the current user is an admin for any project
                //sql = @"select pu_project
			//from project_user_xref
			//where pu_user = $us
			//and pu_admin = 1";
                //sql = sql.Replace("$us", Convert.ToString(security.user.usid));
                //DataSet ds_projects = btnet.DbUtil.get_dataset(sql);

                if (_userService.IsUserProjectAdmin(currentUserId))
                {
                    Response.Write("You not allowed to add users.");
                    Response.End();
                }

                admin.Visible = false;
                admin_label.Visible = false;
                project_admin_label.Visible = false;
                project_admin.Visible = false;
                project_admin_help.Visible = false;

            }

            if (Request["copy"] != null && Request["copy"] == "y")
            {
                copy = true;
            }

            msg.InnerText = "";

            string var = Request.QueryString["id"];
            if (var == null)
            {
                //Set default settings when adding a new user
                id = 0;
                auto_subscribe_own.Checked = true;
                auto_subscribe_reported.Checked = true;
                enable_popups.Checked = true;
                reported_notifications.Items[4].Selected = true;
                assigned_notifications.Items[4].Selected = true;
                subscribed_notifications.Items[4].Selected = true;
            }
            else
            {
                id = Convert.ToInt32(var);
            }

            if (!IsPostBack)
            {

                if (!isUserAdmin)
                {
                    // logged in user is a project level admin
                    ProjectList = _userService.GetProjectPermissionsForProjectAdmin(currentUserId, id, permissionLevel);

                }
                else // user is a real admin
                {
                    ProjectList = _userService.GetProjectPermissionsForSiteAdmin(id, permissionLevel);

                }

                //TODO: REMOVE TEMPORARY TABLES
                //Table 0 - Temporary
                sql = "select pj_id, pj_name from projects;";
                //Table 1 - Temporary
                sql += "select qu_id, qu_desc from queries;";

                // Table 1
                //Queries by Org User
                var QueryList = _queryService.GetQueriesByUsersOrg(id);

   //             sql += @"/* populate query dropdown */
		 //   declare @org int
		 //   set @org = null
		 //   select @org = us_org from users where us_id = $us

			//select qu_id, qu_desc
			//from queries
			//where (isnull(qu_user,0) = 0 and isnull(qu_org,0) = 0)
			//or isnull(qu_user,0) = $us
			//or isnull(qu_org,0) = isnull(@org,-1)
			//order by qu_desc;";

                // Table 2
                if (security.user.is_admin)
                {
                    //Get Orgs for Admins
                    sql += @"/* populate org dropdown 1 */
				select og_id, og_name
				from orgs
				order by og_name;";
                }
                else
                {
                    if (security.user.other_orgs_permission_level == Security.PERMISSION_ALL)
                    {
                        //Get Orgs for Non Admins
                        sql += @"/* populate org dropdown 2 */
					select og_id, og_name
					from orgs
					where og_non_admins_can_use = 1
					order by og_name;";
                    }
                    else
                    {
                        sql += @"/* populate org dropdown 3 */
					select 1; -- dummy";
                    }
                }


                // Table 3
                if (id != 0)
                {

                    // get existing user values
                    sql += @"
			select
				us_username,
				isnull(us_firstname,'') [us_firstname],
				isnull(us_lastname,'') [us_lastname],
				isnull(us_bugs_per_page,10) [us_bugs_per_page],
				us_use_fckeditor,
				us_enable_bug_list_popups,
				isnull(us_email,'') [us_email],
				us_active,
				us_admin,
				us_enable_notifications,
				us_send_notifications_to_self,
                us_reported_notifications,
                us_assigned_notifications,
                us_subscribed_notifications,
				us_auto_subscribe,
				us_auto_subscribe_own_bugs,
				us_auto_subscribe_reported_bugs,
				us_default_query,
				us_org,
				isnull(us_signature,'') [us_signature],
				isnull(us_forced_project,0) [us_forced_project],
				us_created_user
				from users
				where us_id = $us";

                }


                sql = sql.Replace("$us", Convert.ToString(id));
                sql = sql.Replace("$dpl", Util.get_setting("DefaultPermissionLevel", "2"));

                DataSet ds = btnet.DbUtil.get_dataset(sql);

                // query dropdown (used to be ds.Tables[1]) 
                query.DataSource = QueryList.Tables[0].DefaultView;
                query.DataTextField = "qu_desc";
                query.DataValueField = "qu_id";
                query.DataBind();

                // forced project dropdown
                forced_project.DataSource = ProjectList.Tables[0].DefaultView;
                forced_project.DataTextField = "pj_name";
                forced_project.DataValueField = "pj_id";
                forced_project.DataBind();
                forced_project.Items.Insert(0, new ListItem("[no forced project]", "0"));

                // org dropdown
                if (security.user.is_admin
                || security.user.other_orgs_permission_level == Security.PERMISSION_ALL)
                {
                    org.DataSource = ds.Tables[2].DefaultView;
                    org.DataTextField = "og_name";
                    org.DataValueField = "og_id";
                    org.DataBind();
                    org.Items.Insert(0, new ListItem("[select org]", "0"));
                }
                else
                {
                    org.Items.Insert(0, new ListItem(security.user.org_name, Convert.ToString(security.user.org)));
                }

                // populate permissions grid
                MyDataGrid.DataSource = ProjectList.Tables[0].DefaultView;
                MyDataGrid.DataBind();

                // subscribe by project dropdown
                project_auto_subscribe.DataSource = ProjectList.Tables[0].DefaultView;
                project_auto_subscribe.DataTextField = "pj_name";
                project_auto_subscribe.DataValueField = "pj_id";
                project_auto_subscribe.DataBind();

                // project admin dropdown
                project_admin.DataSource = ProjectList.Tables[0].DefaultView;
                project_admin.DataTextField = "pj_name";
                project_admin.DataValueField = "pj_id";
                project_admin.DataBind();


                // add or edit?
                if (id == 0)
                {
                    sub.Value = "Create";
                    bugs_per_page.Value = "10";
                    active.Checked = true;
                    enable_notifications.Checked = true;

                }
                else
                {


                    sub.Value = "Update";

                    // get the values for this existing user
                    DataRow dr = ds.Tables[3].Rows[0];

                    // check if project admin is allowed to edit this user
                    if (!security.user.is_admin)
                    {
                        if (security.user.usid != (int)dr["us_created_user"])
                        {
                            Response.Write("You not allowed to edit this user, because you didn't create it.");
                            Response.End();
                        }
                        else if ((int)dr["us_admin"] == 1)
                        {
                            Response.Write("You not allowed to edit this user, because it is an admin.");
                            Response.End();
                        }
                    }


                    // select values in dropdowns

                    // select forced project
                    int current_forced_project = (int)dr["us_forced_project"];
                    foreach (ListItem li in forced_project.Items)
                    {
                        if (Convert.ToInt32(li.Value) == current_forced_project)
                        {
                            li.Selected = true;
                            break;
                        }
                    }

                    // Fill in this form
                    if (copy)
                    {
                        username.Value = "Enter username here";
                        firstname.Value = "";
                        lastname.Value = "";
                        email.Value = "";
                        signature.InnerText = "";
                    }
                    else
                    {
                        username.Value = (string)dr["us_username"];
                        firstname.Value = (string)dr["us_firstname"];
                        lastname.Value = (string)dr["us_lastname"];
                        email.Value = (string)dr["us_email"];
                        signature.InnerText = (string)dr["us_signature"];
                    }

                    bugs_per_page.Value = Convert.ToString(dr["us_bugs_per_page"]);
                    use_fckeditor.Checked = Convert.ToBoolean((int)dr["us_use_fckeditor"]);
                    enable_popups.Checked = Convert.ToBoolean((int)dr["us_enable_bug_list_popups"]);
                    active.Checked = Convert.ToBoolean((int)dr["us_active"]);
                    admin.Checked = Convert.ToBoolean((int)dr["us_admin"]);
                    enable_notifications.Checked = Convert.ToBoolean((int)dr["us_enable_notifications"]);
                    send_to_self.Checked = Convert.ToBoolean((int)dr["us_send_notifications_to_self"]);
                    reported_notifications.Items[(int)dr["us_reported_notifications"]].Selected = true;
                    assigned_notifications.Items[(int)dr["us_assigned_notifications"]].Selected = true;
                    subscribed_notifications.Items[(int)dr["us_subscribed_notifications"]].Selected = true;
                    auto_subscribe.Checked = Convert.ToBoolean((int)dr["us_auto_subscribe"]);
                    auto_subscribe_own.Checked = Convert.ToBoolean((int)dr["us_auto_subscribe_own_bugs"]);
                    auto_subscribe_reported.Checked = Convert.ToBoolean((int)dr["us_auto_subscribe_reported_bugs"]);


                    // org
                    foreach (ListItem li in org.Items)
                    {
                        if (Convert.ToInt32(li.Value) == (int)dr["us_org"])
                        {
                            li.Selected = true;
                            break;
                        }
                    }

                    // query
                    foreach (ListItem li in query.Items)
                    {
                        if (Convert.ToInt32(li.Value) == (int)dr["us_default_query"])
                        {
                            li.Selected = true;
                            break;
                        }
                    }

                    // select projects
                    foreach (DataRow dr2 in ProjectList.Tables[0].Rows)
                    {
                        foreach (ListItem li in project_auto_subscribe.Items)
                        {
                            if (Convert.ToInt32(li.Value) == (int)dr2["pj_id"])
                            {
                                if ((int)dr2["pu_auto_subscribe"] == 1)
                                {
                                    li.Selected = true;
                                }
                                else
                                {
                                    li.Selected = false;
                                }
                            }
                        }
                    }

                    foreach (DataRow dr3 in ProjectList.Tables[0].Rows)
                    {
                        foreach (ListItem li in project_admin.Items)
                        {
                            if (Convert.ToInt32(li.Value) == (int)dr3["pj_id"])
                            {
                                if ((int)dr3["pu_admin"] == 1)
                                {
                                    li.Selected = true;
                                }
                                else
                                {
                                    li.Selected = false;
                                }
                            }
                        }
                    }

                } // add or edit
            } // if !postback
            else
            {
                on_update();
            }
        }


        ///////////////////////////////////////////////////////////////////////
        Boolean validate()
        {

            Boolean good = true;
            if (username.Value == "")
            {
                good = false;
                username_err.InnerText = "Username is required.";
            }
            else
            {
                username_err.InnerText = "";
            }

            pw_err.InnerText = "";
            if (id == 0 || copy)
            {
                if (pw.Value == "")
                {
                    good = false;
                    pw_err.InnerText = "Password is required.";
                }
            }

            if (pw.Value != "")
            {
                if (!Util.check_password_strength(pw.Value))
                {
                    good = false;
                    pw_err.InnerHtml = "Password is not difficult enough to guess.";
                    pw_err.InnerHtml += "<br>Avoid common words.";
                    pw_err.InnerHtml += "<br>Try using a mixture of lowercase, uppercase, digits, and special characters.";
                }
            }

            if (confirm_pw.Value != pw.Value)
            {
                good = false;
                confirm_pw_err.InnerText = "Confirm Password must match Password.";
            }
            else
            {
                confirm_pw_err.InnerText = "";
            }

            if (org.SelectedItem.Text == "[select org]")
            {
                good = false;
                org_err.InnerText = "You must select a org";
            }
            else
            {
                org_err.InnerText = "";
            }

            if (!Util.is_int(bugs_per_page.Value))
            {
                good = false;
                bugs_per_page_err.InnerText = Util.get_setting("PluralBugLabel", "Bugs") + " Per Page must be a number.";
            }
            else
            {
                bugs_per_page_err.InnerText = "";
            }

            email_err.InnerHtml = "";
            if (email.Value != "")
            {
                if (!Util.validate_email(email.Value))
                {
                    good = false;
                    email_err.InnerHtml = "Format of email address is invalid.";
                }
            }

            return good;
        }

        ///////////////////////////////////////////////////////////////////////
        class Project
        {
            public int id = 0;
            public int admin = 0;
            public int permission_level = 0;
            public int auto_subscribe = 0;
            public bool maybe_insert = false;
        };


        ///////////////////////////////////////////////////////////////////////
        string replace_vars_in_sql_statement(string sql)
        {
            sql = sql.Replace("$un", username.Value.Replace("'", "''"));
            sql = sql.Replace("$fn", firstname.Value.Replace("'", "''"));
            sql = sql.Replace("$ln", lastname.Value.Replace("'", "''"));
            sql = sql.Replace("$bp", bugs_per_page.Value.Replace("'", "''"));
            sql = sql.Replace("$fk", Util.bool_to_string(use_fckeditor.Checked));
            sql = sql.Replace("$pp", Util.bool_to_string(enable_popups.Checked));
            sql = sql.Replace("$em", email.Value.Replace("'", "''"));
            sql = sql.Replace("$ac", Util.bool_to_string(active.Checked));
            sql = sql.Replace("$en", Util.bool_to_string(enable_notifications.Checked));
            sql = sql.Replace("$ss", Util.bool_to_string(send_to_self.Checked));
            sql = sql.Replace("$rn", reported_notifications.SelectedItem.Value);
            sql = sql.Replace("$an", assigned_notifications.SelectedItem.Value);
            sql = sql.Replace("$sn", subscribed_notifications.SelectedItem.Value);
            sql = sql.Replace("$as", Util.bool_to_string(auto_subscribe.Checked));
            sql = sql.Replace("$ao", Util.bool_to_string(auto_subscribe_own.Checked));
            sql = sql.Replace("$ar", Util.bool_to_string(auto_subscribe_reported.Checked));
            sql = sql.Replace("$dq", query.SelectedItem.Value);
            sql = sql.Replace("$org", org.SelectedItem.Value);
            sql = sql.Replace("$sg", signature.InnerText.Replace("'", "''"));
            sql = sql.Replace("$fp", forced_project.SelectedItem.Value);
            sql = sql.Replace("$id", Convert.ToString(id));


            // only admins can create admins.
            if (security.user.is_admin)
            {
                sql = sql.Replace("$ad", Util.bool_to_string(admin.Checked));
            }
            else
            {
                sql = sql.Replace("$ad", "0");
            }

            return sql;

        }

        ///////////////////////////////////////////////////////////////////////
        void on_update()
        {
            Boolean good = validate();

            if (good)
            {

                if (id == 0 || copy)  // insert new
                {
                    // See if the user already exists?
                    sql = "select count(1) from users where us_username = N'$1'";
                    sql = sql.Replace("$1", username.Value.Replace("'", "''"));
                    int user_count = (int)btnet.DbUtil.execute_scalar(sql);

                    if (user_count == 0)
                    {

                        StringBuilder createUserSql = new StringBuilder();
                        createUserSql.AppendLine("insert into users (us_username, us_password, us_firstname, us_lastname, ");
                        createUserSql.Append("us_bugs_per_page, us_use_fckeditor, us_enable_bug_list_popups, ");
                        createUserSql.Append("us_email, us_active, us_admin, us_enable_notifications, us_send_notifications_to_self, ");
                        createUserSql.Append("us_reported_notifications, us_assigned_notifications, ");
                        createUserSql.Append("us_subscribed_notifications, us_auto_subscribe, us_auto_subscribe_own_bugs, ");
                        createUserSql.Append("us_auto_subscribe_reported_bugs, us_default_query, us_org, us_signature, ");
                        createUserSql.Append("us_forced_project, us_created_user)");
                        createUserSql.AppendLine("values (@username, @password, @firstName, @lastName, @bugsPerPage, @fckEditor, @popups, ");
                        createUserSql.Append("@email, @active, @isAdmin, @notifications, @selfNotifications, @reportedNotifications, ");
                        createUserSql.Append("@assignedNotifications, @subscribedNotifications, @autoSubscribe, @subscribeOwnBugs, ");
                        createUserSql.Append("@subscribeReportedBugs, @defaultQuery, @org, @signature, @forcedProject, @createdBy);");
                        createUserSql.AppendLine("select scope_identity()");

                        SqlCommand createUserCmd = new SqlCommand();
                        createUserCmd.CommandText = createUserSql.ToString();

                        createUserCmd.Parameters.AddWithValue("@username", username.Value);

                        // This is old code to fill the password field with some junk, just temporarily. Password is updated in separate method
                        //TODO: Fix this
                        createUserCmd.Parameters.AddWithValue("@password", Convert.ToString(new Random().Next()));
                        createUserCmd.Parameters.AddWithValue("@firstName", firstname.Value);
                        createUserCmd.Parameters.AddWithValue("@lastName", lastname.Value);
                        createUserCmd.Parameters.AddWithValue("@bugsPerPage", bugs_per_page.Value);
                        createUserCmd.Parameters.AddWithValue("@fckEditor", use_fckeditor.Checked);
                        createUserCmd.Parameters.AddWithValue("@popups", enable_popups.Checked);
                        createUserCmd.Parameters.AddWithValue("@email", email.Value);
                        createUserCmd.Parameters.AddWithValue("@active", active.Checked);
                        createUserCmd.Parameters.AddWithValue("@notifications", enable_notifications.Checked);
                        createUserCmd.Parameters.AddWithValue("@selfNotifications", send_to_self.Checked);
                        createUserCmd.Parameters.AddWithValue("@reportedNotifications", reported_notifications.SelectedItem.Value);
                        createUserCmd.Parameters.AddWithValue("@assignedNotifications", assigned_notifications.SelectedItem.Value);
                        createUserCmd.Parameters.AddWithValue("@subscribedNotifications", subscribed_notifications.SelectedItem.Value);
                        createUserCmd.Parameters.AddWithValue("@autoSubscribe", auto_subscribe.Checked);
                        createUserCmd.Parameters.AddWithValue("@subscribeOwnBugs", auto_subscribe_own.Checked);
                        createUserCmd.Parameters.AddWithValue("@subscribeReportedBugs", auto_subscribe_reported.Checked);
                        createUserCmd.Parameters.AddWithValue("@defaultQuery", query.SelectedItem.Value);
                        createUserCmd.Parameters.AddWithValue("@org", org.SelectedItem.Value);
                        createUserCmd.Parameters.AddWithValue("@signature", signature.InnerText);
                        createUserCmd.Parameters.AddWithValue("@forcedProject", forced_project.SelectedItem.Value);
                        createUserCmd.Parameters.AddWithValue("@createdBy", security.user.usid);

                        // only admins can create admins.
                        if (security.user.is_admin)
                        {
                            createUserCmd.Parameters.AddWithValue("@isAdmin", admin.Checked);
                        }
                        else
                        {
                            createUserCmd.Parameters.AddWithValue("@isAdmin", 0);
                        }

                        // insert the user
                        id = Convert.ToInt32(btnet.DbUtil.execute_scalar(createUserCmd));

                        // now encrypt the password and update the db
                        btnet.Util.UpdateUserPassword(id, pw.Value);

                        update_project_user_xref();

                        Server.Transfer("users.aspx");
                    }
                    else
                    {
                        username_err.InnerText = "User already exists.   Choose another username.";
                        msg.InnerText = "User was not created.";
                    }


                }
                else // edit existing
                {

                    // See if the user already exists?
                    sql = @"select count(1)
				from users where us_username = N'$1' and us_id <> $2";
                    sql = sql.Replace("$1", username.Value.Replace("'", "''"));
                    sql = sql.Replace("$2", Convert.ToString(id));
                    int user_count = (int)btnet.DbUtil.execute_scalar(sql);

                    if (user_count == 0)
                    {

                        sql = @"
update users set
us_username = N'$un',
us_firstname = N'$fn',
us_lastname = N'$ln',
us_bugs_per_page = N'$bp',
us_use_fckeditor = $fk,
us_enable_bug_list_popups = $pp,
us_email = N'$em',
us_active = $ac,
us_admin = $ad,
us_enable_notifications = $en,
us_send_notifications_to_self = $ss,
us_reported_notifications = $rn,
us_assigned_notifications = $an,
us_subscribed_notifications = $sn,
us_auto_subscribe = $as,
us_auto_subscribe_own_bugs = $ao,
us_auto_subscribe_reported_bugs = $ar,
us_default_query = $dq,
us_org = $org,
us_signature = N'$sg',
us_forced_project = $fp
where us_id = $id";


                        sql = replace_vars_in_sql_statement(sql);

                        btnet.DbUtil.execute_nonquery(sql);

                        // update the password
                        if (pw.Value != "")
                        {
                            btnet.Util.UpdateUserPassword(id, pw.Value);
                        }

                        update_project_user_xref();

                        Server.Transfer("users.aspx");
                    }
                    else
                    {
                        username_err.InnerText = "Username already exists.   Choose another username.";
                        msg.InnerText = "User was not updated.";
                    }

                }
            }
            else
            {
                if (id == 0)  // insert new
                {
                    msg.InnerText = "User was not created.";
                }
                else // edit existing
                {
                    msg.InnerText = "User was not updated.";
                }

            }

        }

        void update_project_user_xref()
        {

            System.Collections.Hashtable hash_projects = new System.Collections.Hashtable();


            foreach (ListItem li in project_auto_subscribe.Items)
            {
                Project p = new Project();
                p.id = Convert.ToInt32(li.Value);
                hash_projects[p.id] = p;

                if (li.Selected)
                {
                    p.auto_subscribe = 1;
                    p.maybe_insert = true;
                }
                else
                {
                    p.auto_subscribe = 0;
                }
            }

            foreach (ListItem li in project_admin.Items)
            {
                Project p = (Project)hash_projects[Convert.ToInt32(li.Value)];
                if (li.Selected)
                {
                    p.admin = 1;
                    p.maybe_insert = true;
                }
                else
                {
                    p.admin = 0;
                }
            }


            RadioButton rb;
            int permission_level;
            int default_permission_level = Convert.ToInt32(Util.get_setting("DefaultPermissionLevel", "2"));

            foreach (DataGridItem dgi in MyDataGrid.Items)
            {
                rb = (RadioButton)dgi.FindControl("none");
                if (rb.Checked)
                {
                    permission_level = 0;
                }
                else
                {
                    rb = (RadioButton)dgi.FindControl("readonly");
                    if (rb.Checked)
                    {
                        permission_level = 1;
                    }
                    else
                    {
                        rb = (RadioButton)dgi.FindControl("reporter");
                        if (rb.Checked)
                        {
                            permission_level = 3;
                        }
                        else
                        {
                            permission_level = 2;
                        }
                    }
                }

                int pj_id = Convert.ToInt32(dgi.Cells[1].Text);

                Project p = (Project)hash_projects[pj_id];
                p.permission_level = permission_level;

                if (permission_level != default_permission_level)
                {
                    p.maybe_insert = true;
                }
            }

            string projects = "";
            foreach (Project p in hash_projects.Values)
            {
                if (p.maybe_insert)
                {
                    if (projects != "")
                    {
                        projects += ",";
                    }

                    projects += Convert.ToString(p.id);
                }
            }

            sql = "";

            // Insert new recs - we will update them later
            // Downstream logic is now simpler in that it just deals with existing recs
            if (projects != "")
            {
                sql += @"
			insert into project_user_xref (pu_project, pu_user, pu_auto_subscribe)
			select pj_id, $us, 0
			from projects
			where pj_id in ($projects)
			and pj_id not in (select pu_project from project_user_xref where pu_user = $us);";
                sql = sql.Replace("$projects", projects);
            }

            // First turn everything off, then turn selected ones on.
            sql += @"
		update project_user_xref
		set pu_auto_subscribe = 0,
		pu_admin = 0,
		pu_permission_level = $dpl
		where pu_user = $us;";

            projects = "";
            foreach (Project p in hash_projects.Values)
            {
                if (p.auto_subscribe == 1)
                {
                    if (projects != "")
                    {
                        projects += ",";
                    }

                    projects += Convert.ToString(p.id);
                }
            }
            string auto_subscribe_projects = projects; // save for later

            if (projects != "")
            {
                sql += @"
			update project_user_xref
			set pu_auto_subscribe = 1
			where pu_user = $us
			and pu_project in ($projects);";
                sql = sql.Replace("$projects", projects);
            }

            if (security.user.is_admin)
            {
                projects = "";
                foreach (Project p in hash_projects.Values)
                {
                    if (p.admin == 1)
                    {
                        if (projects != "")
                        {
                            projects += ",";
                        }

                        projects += Convert.ToString(p.id);
                    }
                }

                if (projects != "")
                {
                    sql += @"
				update project_user_xref
				set pu_admin = 1
				where pu_user = $us
				and pu_project in ($projects);";

                    sql = sql.Replace("$projects", projects);
                }
            }

            // update permission levels to 0
            projects = "";
            foreach (Project p in hash_projects.Values)
            {
                if (p.permission_level == 0)
                {
                    if (projects != "")
                    {
                        projects += ",";
                    }

                    projects += Convert.ToString(p.id);
                }

            }
            if (projects != "")
            {
                sql += @"
			update project_user_xref
			set pu_permission_level = 0
			where pu_user = $us
			and pu_project in ($projects);";

                sql = sql.Replace("$projects", projects);
            }

            // update permission levels to 1
            projects = "";
            foreach (Project p in hash_projects.Values)
            {
                if (p.permission_level == 1)
                {
                    if (projects != "")
                    {
                        projects += ",";
                    }

                    projects += Convert.ToString(p.id);
                }

            }
            if (projects != "")
            {
                sql += @"
			update project_user_xref
			set pu_permission_level = 1
			where pu_user = $us
			and pu_project in ($projects);";

                sql = sql.Replace("$projects", projects);
            }


            // update permission levels to 2
            projects = "";
            foreach (Project p in hash_projects.Values)
            {
                if (p.permission_level == 2)
                {
                    if (projects != "")
                    {
                        projects += ",";
                    }

                    projects += Convert.ToString(p.id);
                }
            }
            if (projects != "")
            {
                sql += @"
			update project_user_xref
			set pu_permission_level = 2
			where pu_user = $us
			and pu_project in ($projects);";

                sql = sql.Replace("$projects", projects);
            }

            // update permission levels to 3
            projects = "";
            foreach (Project p in hash_projects.Values)
            {
                if (p.permission_level == 3)
                {
                    if (projects != "")
                    {
                        projects += ",";
                    }

                    projects += Convert.ToString(p.id);
                }
            }
            if (projects != "")
            {
                sql += @"
			update project_user_xref
			set pu_permission_level = 3
			where pu_user = $us
			and pu_project in ($projects);";

                sql = sql.Replace("$projects", projects);
            }


            // apply subscriptions retroactively
            if (retroactive.Checked)
            {
                sql = @"
			delete from bug_subscriptions where bs_user = $us;";

                if (auto_subscribe.Checked)
                {
                    sql += @"
			insert into bug_subscriptions (bs_bug, bs_user)
				select bg_id, $us from bugs;";
                }
                else
                {
                    if (auto_subscribe_reported.Checked)
                    {
                        sql += @"
					insert into bug_subscriptions (bs_bug, bs_user)
					select bg_id, $us from bugs where bg_reported_user = $us
					and bg_id not in (select bs_bug from bug_subscriptions where bs_user = $us);";
                    }

                    if (auto_subscribe_own.Checked)
                    {
                        sql += @"
					insert into bug_subscriptions (bs_bug, bs_user)
					select bg_id, $us from bugs where bg_assigned_to_user = $us
					and bg_id not in (select bs_bug from bug_subscriptions where bs_user = $us);";
                    }

                    if (auto_subscribe_projects != "")
                    {
                        sql += @"
					insert into bug_subscriptions (bs_bug, bs_user)
					select bg_id, $us from bugs where bg_project in ($projects)
					and bg_id not in (select bs_bug from bug_subscriptions where bs_user = $us);";
                        sql = sql.Replace("$projects", auto_subscribe_projects);
                    }
                }
            }

            sql = sql.Replace("$us", Convert.ToString(id));
            sql = sql.Replace("$dpl", Convert.ToString(default_permission_level));
            btnet.DbUtil.execute_nonquery(sql);

        }
    }
}
