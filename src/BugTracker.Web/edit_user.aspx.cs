using btnet;
using BugTracker.Web.Models.User;
using BugTracker.Web.Services;
using BugTracker.Web.Services.Bug;
using BugTracker.Web.Services.Organization;
using BugTracker.Web.Services.Project;
using BugTracker.Web.Services.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class edit_user : Page
    {
        int id;
        bool copy = false;

        public Security security;
        private UserService _userService = new UserService();
        private QueryService _queryService = new QueryService();
        private OrganizationService _orgService = new OrganizationService();
        private ProjectService _projectService = new ProjectService();
        private BugSubscriptionService _subscriptionService = new BugSubscriptionService();

        const string _newUserErrorMessage = "User was not created.";
        const string _editUserErrorMessage = "User was not updated";
        
        string errorMessage = string.Empty;


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

            //Sets error message depending of if this is a new user or updating an existing user
            if (id == 0)
            {
                errorMessage = _newUserErrorMessage;
            }
            else
            {
                errorMessage = _editUserErrorMessage;
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

                var QueryList = _queryService.GetQueriesByUsersOrg(id);

                DataSet OrgList = new DataSet();
                if (security.user.is_admin)
                {
                    //Get Orgs for Admins
                    OrgList = _orgService.GetOrganizationDropDownList();
                }
                else
                {
                    if (security.user.other_orgs_permission_level == Security.PERMISSION_ALL)
                    {
                        //Get Orgs for Non Admins
                        OrgList = _orgService.GetOrgListForNonAdmins();
                    }
                }

                // query dropdown
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
                    org.DataSource = OrgList.Tables[0].DefaultView;
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
                    DataRow dr = _userService.GetUserDetailsById(id);

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

                    FillFormWithUserValues(dr, ProjectList);

                } // add or edit
            } // if !postback
            else
            {
                on_update();
            }
        }

        /// <summary>
        /// Fills in Page with User Information
        /// </summary>
        /// <param name="userDr"></param>
        /// <param name="projectList"></param>
        protected void FillFormWithUserValues(DataRow userDr, DataSet projectList)
        {
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
                username.Value = (string)userDr["us_username"];
                firstname.Value = (string)userDr["us_firstname"];
                lastname.Value = (string)userDr["us_lastname"];
                email.Value = (string)userDr["us_email"];
                signature.InnerText = (string)userDr["us_signature"];
            }

            bugs_per_page.Value = Convert.ToString(userDr["us_bugs_per_page"]);
            use_fckeditor.Checked = Convert.ToBoolean((int)userDr["us_use_fckeditor"]);
            enable_popups.Checked = Convert.ToBoolean((int)userDr["us_enable_bug_list_popups"]);
            active.Checked = Convert.ToBoolean((int)userDr["us_active"]);
            admin.Checked = Convert.ToBoolean((int)userDr["us_admin"]);
            enable_notifications.Checked = Convert.ToBoolean((int)userDr["us_enable_notifications"]);
            send_to_self.Checked = Convert.ToBoolean((int)userDr["us_send_notifications_to_self"]);
            reported_notifications.Items[(int)userDr["us_reported_notifications"]].Selected = true;
            assigned_notifications.Items[(int)userDr["us_assigned_notifications"]].Selected = true;
            subscribed_notifications.Items[(int)userDr["us_subscribed_notifications"]].Selected = true;
            auto_subscribe.Checked = Convert.ToBoolean((int)userDr["us_auto_subscribe"]);
            auto_subscribe_own.Checked = Convert.ToBoolean((int)userDr["us_auto_subscribe_own_bugs"]);
            auto_subscribe_reported.Checked = Convert.ToBoolean((int)userDr["us_auto_subscribe_reported_bugs"]);

            // select forced project
            int current_forced_project = (int)userDr["us_forced_project"];
            foreach (ListItem li in forced_project.Items)
            {
                if (Convert.ToInt32(li.Value) == current_forced_project)
                {
                    li.Selected = true;
                    break;
                }
            }

            // org
            foreach (ListItem li in org.Items)
            {
                if (Convert.ToInt32(li.Value) == (int)userDr["us_org"])
                {
                    li.Selected = true;
                    break;
                }
            }

            // query
            foreach (ListItem li in query.Items)
            {
                if (Convert.ToInt32(li.Value) == (int)userDr["us_default_query"])
                {
                    li.Selected = true;
                    break;
                }
            }

            // select projects
            foreach (DataRow dr2 in projectList.Tables[0].Rows)
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

            foreach (DataRow dr3 in projectList.Tables[0].Rows)
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
        }

        /// <summary>
        /// Validates Form values
        /// </summary>
        /// <returns></returns>
        protected bool ValidateForm()
        {

            bool good = true;
            if (username.Value == "")
            {
                good = false;
                username_err.InnerText = "Username is required.";
            }
            else
            {
                username_err.InnerText = "";
            }

            // See if the user already exists
            bool isUnquieName = _userService.IsUserNameUnique(username.Value, id);

            if (!isUnquieName)
            {
                good = false;
                username_err.InnerText = "User already exists.   Choose another username.";
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

        class Project
        {
            public int id = 0;
            public int admin = 0;
            public int permission_level = 0;
            public int auto_subscribe = 0;
            public bool maybe_insert = false;
        };

        /// <summary>
        /// Updates form. Used when adding a new user or editing an exiting user
        /// </summary>
        void on_update()
        {
            bool isValid = ValidateForm();

            if (isValid)
            {
                NewUser user = new NewUser
                {
                    Id = id,
                    UserName = username.Value,
                    Password = pw.Value,
                    FirstName = firstname.Value,
                    LastName = lastname.Value,
                    BugsPerPage = Convert.ToInt32(bugs_per_page.Value),
                    UseFckEditor = use_fckeditor.Checked,
                    EnablePopups = enable_popups.Checked,
                    Email = email.Value,
                    IsActive = active.Checked,
                    EnableNotifications = enable_notifications.Checked,
                    SendToSelf = send_to_self.Checked,
                    ReportedNotifications = Convert.ToInt32(reported_notifications.SelectedItem.Value),
                    AssignedNotifications = Convert.ToInt32(assigned_notifications.SelectedItem.Value),
                    SubscribedNotifications = Convert.ToInt32(subscribed_notifications.SelectedItem.Value),
                    AutoSubscribe = auto_subscribe.Checked,
                    AutoSubscribeOwn = auto_subscribe_own.Checked,
                    AutoSubscribeReported = auto_subscribe_reported.Checked,
                    DefaultQueryId = Convert.ToInt32(query.SelectedItem.Value),
                    OrginizationId = Convert.ToInt32(org.SelectedItem.Value),
                    Signature = signature.InnerText,
                    ForcedProjectId = Convert.ToInt32(forced_project.SelectedItem.Value),
                    CreatedById = security.user.usid
                };

                // only admins can create admins.
                if (security.user.is_admin)
                {
                    user.IsAdmin = admin.Checked;
                }
                else
                {
                    user.IsAdmin = false;
                }

                //Add or update user
                if (id == 0 || copy)  // insert new
                {
                    id = _userService.AddNewUser(user);
                        
                    update_project_user_xref();

                    Server.Transfer("users.aspx");  
                }
                else // edit existing
                {
                    _userService.UpdateUser(user);  

                    update_project_user_xref();

                    Server.Transfer("users.aspx");
                }
            }
            else
            {
                msg.InnerText = errorMessage;
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

            List<int> projectIds = new List<int>();
            foreach (Project p in hash_projects.Values)
            {
                if (p.maybe_insert)
                {
                    projectIds.Add(p.id);
                }
            }

            // Insert new recs - we will update them later
            // Downstream logic is now simpler in that it just deals with existing recs
            if(projectIds.Count > 0) 
            {
                _projectService.AddProjectSettings(projectIds, id, false);
            }

            // First turn everything off, then turn selected ones on.
            _projectService.SetDefaultProjectSettings(id, default_permission_level);

            projectIds.Clear();
            foreach (Project p in hash_projects.Values)
            {
                if (p.auto_subscribe == 1)
                {
                    projectIds.Add(p.id);
                }
            }
            // Save to possibly apply retroactively below
            List<int> autoSubscribeProjects = projectIds; 

            if (projectIds.Count > 0)
            {
                _projectService.UpdateAutoSubscribe(projectIds, id);
            }

            if (security.user.is_admin)
            {
                projectIds.Clear();
                foreach (Project p in hash_projects.Values)
                {
                    if (p.admin == 1)
                    {
                        projectIds.Add(p.id);
                    }
                }

                if (projectIds.Count > 0)
                {
                    _projectService.UpdateProjectAdmins(projectIds, id);
                }
            }

            // update permission levels to 0
            projectIds.Clear();
            foreach (Project p in hash_projects.Values)
            {
                if (p.permission_level == 0)
                {
                    projectIds.Add(p.id);
                }

            }
            if (projectIds.Count > 0)
            {
                _projectService.UpdateProjectPermissionLevels(projectIds, 0, id);
            }

            // update permission levels to 1
            projectIds.Clear();
            foreach (Project p in hash_projects.Values)
            {
                if (p.permission_level == 1)
                {
                    projectIds.Add(p.id);
                }

            }
            if (projectIds.Count > 0)
            {
                _projectService.UpdateProjectPermissionLevels(projectIds, 1, id);

            }

            // update permission levels to 2
            projectIds.Clear();
            foreach (Project p in hash_projects.Values)
            {
                if (p.permission_level == 2)
                {
                    projectIds.Add(p.id);
                }
            }
            if (projectIds.Count > 0)
            {
                _projectService.UpdateProjectPermissionLevels(projectIds, 2, id);
            }

            // update permission levels to 3
            projectIds.Clear();
            foreach (Project p in hash_projects.Values)
            {
                if (p.permission_level == 3)
                {
                    projectIds.Add(p.id);
                }
            }
            if (projectIds.Count > 0)
            {
                _projectService.UpdateProjectPermissionLevels(projectIds, 3, id);
            }


            // apply subscriptions retroactively
            if (retroactive.Checked)
            {
                _subscriptionService.DeleteSubscriptionsForUser(id);

                if (auto_subscribe.Checked)
                {
                    _subscriptionService.AddAutoSubscribeBugs(id);
                }
                else
                {
                    if (auto_subscribe_reported.Checked)
                    {
                        _subscriptionService.AddAutoSubscribeReportedBugs(id);
                    }

                    if (auto_subscribe_own.Checked)
                    {
                        _subscriptionService.AddAutoSubscribeOwnBugs(id);
                    }

                    if (autoSubscribeProjects.Count > 0)
                    {
                        _subscriptionService.AddAutoSubscribeProjects(id, autoSubscribeProjects);
                    }
                }
            }
        }
    }
}
