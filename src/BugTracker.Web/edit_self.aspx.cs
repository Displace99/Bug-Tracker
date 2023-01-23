using btnet;
using BugTracker.Web.Models.User;
using BugTracker.Web.Services;
using BugTracker.Web.Services.Project;
using BugTracker.Web.Services.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class edit_self : Page
    {
        private int id;
        String sql;

        //We always default to 2
        int defaultPermissionLevel = 2;
        protected Security security;
        private QueryService _queryService = new QueryService();
        private ProjectService _projectService = new ProjectService();
        private UserService _userService = new UserService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit your settings";

            msg.InnerText = "";

            id = security.user.usid;
            defaultPermissionLevel = Convert.ToInt32(Util.get_setting("DefaultPermissionLevel", "2"));

            if (!IsPostBack)
            {
                var queryList = _queryService.GetQueriesByUserForSelf(id);

                query.DataSource = queryList;
                query.DataTextField = "qu_desc";
                query.DataValueField = "qu_id";
                query.DataBind();


                DataView projects_dv = _projectService.GetProjectListForSelf(id, defaultPermissionLevel);

                project_auto_subscribe.DataSource = projects_dv;
                project_auto_subscribe.DataTextField = "pj_name";
                project_auto_subscribe.DataValueField = "pj_id";
                project_auto_subscribe.DataBind();


                // Get this entry's data from the db and fill in the form
                DataRow dr = _userService.GetUserDetailsById(id);

                firstname.Value = (string)dr["us_firstname"];
                lastname.Value = (string)dr["us_lastname"];
                bugs_per_page.Value = Convert.ToString(dr["us_bugs_per_page"]);

                if (Util.get_setting("DisableFCKEditor", "0") == "1")
                {
                    use_fckeditor.Visible = false;
                    use_fckeditor_label.Visible = false;
                }

                use_fckeditor.Checked = Convert.ToBoolean((int)dr["us_use_fckeditor"]);
                enable_popups.Checked = Convert.ToBoolean((int)dr["us_enable_bug_list_popups"]);
                email.Value = (string)dr["us_email"];
                enable_notifications.Checked = Convert.ToBoolean((int)dr["us_enable_notifications"]);
                reported_notifications.Items[(int)dr["us_reported_notifications"]].Selected = true;
                assigned_notifications.Items[(int)dr["us_assigned_notifications"]].Selected = true;
                subscribed_notifications.Items[(int)dr["us_subscribed_notifications"]].Selected = true;
                send_to_self.Checked = Convert.ToBoolean((int)dr["us_send_notifications_to_self"]);
                auto_subscribe.Checked = Convert.ToBoolean((int)dr["us_auto_subscribe"]);
                auto_subscribe_own.Checked = Convert.ToBoolean((int)dr["us_auto_subscribe_own_bugs"]);
                auto_subscribe_reported.Checked = Convert.ToBoolean((int)dr["us_auto_subscribe_reported_bugs"]);
                signature.InnerText = (string)dr["us_signature"];

                foreach (ListItem li in query.Items)
                {
                    if (Convert.ToInt32(li.Value) == (int)dr["us_default_query"])
                    {
                        li.Selected = true;
                        break;
                    }
                }

                // select projects
                foreach (DataRowView drv in projects_dv)
                {
                    foreach (ListItem li in project_auto_subscribe.Items)
                    {
                        if (Convert.ToInt32(li.Value) == (int)drv["pj_id"])
                        {
                            if ((int)drv["pu_auto_subscribe"] == 1)
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
            else
            {
                UpdateUser();
            }
        }


        /// <summary>
        /// Validates Form values
        /// </summary>
        /// <returns></returns>
        protected bool ValidateForm()
        {

            bool good = true;

            pw_err.InnerText = "";

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

        /// <summary>
        /// Updates the user
        /// </summary>
        protected void UpdateUser()
        {
            bool isValid = ValidateForm();

            if (isValid)
            {

                NewUser user = new NewUser
                {
                    Id = id,
                    Password = pw.Value,
                    FirstName = firstname.Value,
                    LastName = lastname.Value,
                    BugsPerPage = Convert.ToInt32(bugs_per_page.Value),
                    UseFckEditor = use_fckeditor.Checked,
                    EnablePopups = enable_popups.Checked,
                    Email = email.Value,
                    EnableNotifications = enable_notifications.Checked,
                    SendToSelf = send_to_self.Checked,
                    ReportedNotifications = Convert.ToInt32(reported_notifications.SelectedItem.Value),
                    AssignedNotifications = Convert.ToInt32(assigned_notifications.SelectedItem.Value),
                    SubscribedNotifications = Convert.ToInt32(subscribed_notifications.SelectedItem.Value),
                    AutoSubscribe = auto_subscribe.Checked,
                    AutoSubscribeOwn = auto_subscribe_own.Checked,
                    AutoSubscribeReported = auto_subscribe_reported.Checked,
                    DefaultQueryId = Convert.ToInt32(query.SelectedItem.Value),
                    Signature = signature.InnerText,

                };

                _userService.UpdateSelf(user);

                // Now update project_user_xref

                // First turn everything off, then turn selected ones on.
                sql = @"update project_user_xref
				set pu_auto_subscribe = 0 where pu_user = $id";
                sql = sql.Replace("$id", Convert.ToString(id));
                btnet.DbUtil.execute_nonquery(sql);

                // Second see what to turn back on
                string projects = "";
                foreach (ListItem li in project_auto_subscribe.Items)
                {
                    if (li.Selected)
                    {
                        if (projects != "")
                        {
                            projects += ",";
                        }
                        projects += Convert.ToInt32(li.Value);
                    }
                }

                // If we need to turn anything back on
                if (projects != "")
                {

                    sql = @"update project_user_xref
				set pu_auto_subscribe = 1 where pu_user = $id and pu_project in ($projects)

			insert into project_user_xref (pu_project, pu_user, pu_auto_subscribe)
				select pj_id, $id, 1
				from projects
				where pj_id in ($projects)
				and pj_id not in (select pu_project from project_user_xref where pu_user = $id)";

                    sql = sql.Replace("$id", Convert.ToString(id));
                    sql = sql.Replace("$projects", projects);
                    btnet.DbUtil.execute_nonquery(sql);
                }


                // apply subscriptions retroactively
                if (retroactive.Checked)
                {
                    sql = @"delete from bug_subscriptions where bs_user = $id;";
                    if (auto_subscribe.Checked)
                    {
                        sql += @"insert into bug_subscriptions (bs_bug, bs_user)
					select bg_id, $id from bugs;";
                    }
                    else
                    {
                        if (auto_subscribe_reported.Checked)
                        {
                            sql += @"insert into bug_subscriptions (bs_bug, bs_user)
						select bg_id, $id from bugs where bg_reported_user = $id
						and bg_id not in (select bs_bug from bug_subscriptions where bs_user = $id);";
                        }

                        if (auto_subscribe_own.Checked)
                        {
                            sql += @"insert into bug_subscriptions (bs_bug, bs_user)
						select bg_id, $id from bugs where bg_assigned_to_user = $id
						and bg_id not in (select bs_bug from bug_subscriptions where bs_user = $id);";
                        }

                        if (projects != "")
                        {
                            sql += @"insert into bug_subscriptions (bs_bug, bs_user)
						select bg_id, $id from bugs where bg_project in ($projects)
						and bg_id not in (select bs_bug from bug_subscriptions where bs_user = $id);";
                        }
                    }

                    sql = sql.Replace("$id", Convert.ToString(id));
                    sql = sql.Replace("$projects", projects);
                    btnet.DbUtil.execute_nonquery(sql);

                }

                msg.InnerText = "Your settings have been updated.";
            }
            else
            {
                msg.InnerText = "Your settings have not been updated.";
            }

        }
    }
}
