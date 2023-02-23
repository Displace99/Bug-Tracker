using btnet;
using BugTracker.Web.Models.Project;
using BugTracker.Web.Services;
using BugTracker.Web.Services.Project;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class edit_project : Page
    {
        protected int id = 0;

        protected Security security;

        private UserService _userService = new UserService();
        private ProjectService _projectService = new ProjectService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit project";

            msg.InnerText = "";

            Int32.TryParse(Request.QueryString["id"], out id);

            if (!IsPostBack)
            {
                default_user.DataSource = _userService.GetUserDataView();
                default_user.DataTextField = "us_username";
                default_user.DataValueField = "us_id";
                default_user.DataBind();
                default_user.Items.Insert(0, new ListItem("", "0"));

                // add or edit?
                if (id == 0)
                {
                    sub.Value = "Create";
                    active.Checked = true;
                }
                else
                {
                    sub.Value = "Update";

                    // Get this entry's data from the db and fill in the form
                    DataRow dr = _projectService.GetFullProjectDetails(id);

                    name.Value = (string)dr["pj_name"];
                    active.Checked = Convert.ToBoolean((int)dr["pj_active"]);
                    auto_assign.Checked = Convert.ToBoolean((int)dr["pj_auto_assign_default_user"]);
                    auto_subscribe.Checked = Convert.ToBoolean((int)dr["pj_auto_subscribe_default_user"]);
                    default_selection.Checked = Convert.ToBoolean((int)dr["pj_default"]);
                    enable_pop3.Checked = Convert.ToBoolean((int)dr["pj_enable_pop3"]);
                    pop3_username.Value = (string)dr["pj_pop3_username"];
                    pop3_email_from.Value = (string)dr["pj_pop3_email_from"];

                    enable_custom_dropdown1.Checked = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown1"]);
                    enable_custom_dropdown2.Checked = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown2"]);
                    enable_custom_dropdown3.Checked = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown3"]);

                    custom_dropdown_label1.Value = (string)dr["pj_custom_dropdown_label1"];
                    custom_dropdown_label2.Value = (string)dr["pj_custom_dropdown_label2"];
                    custom_dropdown_label3.Value = (string)dr["pj_custom_dropdown_label3"];

                    custom_dropdown_values1.Value = (string)dr["pj_custom_dropdown_values1"];
                    custom_dropdown_values2.Value = (string)dr["pj_custom_dropdown_values2"];
                    custom_dropdown_values3.Value = (string)dr["pj_custom_dropdown_values3"];

                    desc.Value = (string)dr["pj_description"];

                    foreach (ListItem li in default_user.Items)
                    {
                        if (Convert.ToInt32(li.Value) == (int)dr["pj_default_user"])
                        {
                            li.Selected = true;
                            break;
                        }
                    }

                    permissions_href.HRef = "edit_user_permissions2.aspx?id=" + Convert.ToString(id)
                        + "&label=" + HttpUtility.UrlEncode(name.Value);
                }
            }
            else
            {
                on_update();
            }
        }


        bool ValidateForm()
        {
            bool good = true;

            //Clear error messages 
            name_err.InnerText = "";
            custom_dropdown_values1_err.InnerText = "";
            custom_dropdown_values2_err.InnerText = "";
            custom_dropdown_values3_err.InnerText = "";

            if (name.Value == "")
            {
                good = false;
                name_err.InnerText = "Description is required.";
            }

            string vals_error_string = "";
            bool errors_with_custom_dropdowns = false;
            vals_error_string = btnet.Util.validate_dropdown_values(custom_dropdown_values1.Value);
            if (!string.IsNullOrEmpty(vals_error_string))
            {
                good = false;
                custom_dropdown_values1_err.InnerText = vals_error_string;
                errors_with_custom_dropdowns = true;
            }

            vals_error_string = btnet.Util.validate_dropdown_values(custom_dropdown_values2.Value);
            if (!string.IsNullOrEmpty(vals_error_string))
            {
                good = false;
                custom_dropdown_values2_err.InnerText = vals_error_string;
                errors_with_custom_dropdowns = true;
            }

            vals_error_string = btnet.Util.validate_dropdown_values(custom_dropdown_values3.Value);
            if (!string.IsNullOrEmpty(vals_error_string))
            {
                good = false;
                custom_dropdown_values3_err.InnerText = vals_error_string;
                errors_with_custom_dropdowns = true;
            }

            if (errors_with_custom_dropdowns)
            {
                msg.InnerText += "Custom fields have errors.  ";
            }

            return good;
        }


        void on_update()
        {
            bool isValid = ValidateForm();

            if (isValid)
            {
                UpdateProject projectModel = new UpdateProject
                {
                    Id = id,
                    Name = name.Value,
                    IsActive = active.Checked,
                    DefaultUserId = Convert.ToInt32(default_user.SelectedItem.Value),
                    IsAutoAssign = auto_assign.Checked,
                    IsAutoSubscribe = auto_subscribe.Checked,
                    IsDefaultSelection = default_selection.Checked,
                    EnablePop3 = enable_pop3.Checked,
                    Pop3UserName = pop3_username.Value,
                    Pop3Password = pop3_password.Value,
                    Pop3EmailFrom = pop3_email_from.Value,
                    Description = desc.Value,
                    EnableDropdown1 = enable_custom_dropdown1.Checked,
                    Dropdown1Label = custom_dropdown_label1.Value,
                    Dropdown1Values = custom_dropdown_values1.Value,
                    EnableDropdown2 = enable_custom_dropdown2.Checked,
                    Dropdown2Label = custom_dropdown_label2.Value,
                    Dropdown2Values = custom_dropdown_values2.Value,
                    EnableDropdown3 = enable_custom_dropdown3.Checked,
                    Dropdown3Label = custom_dropdown_label3.Value,
                    Dropdown3Values = custom_dropdown_values3.Value,
                };

                if (id == 0)  // insert new
                {
                    _projectService.AddNewProject(projectModel);
                }
                else // edit existing
                {
                    _projectService.UpdateProject(projectModel);
                }
                
                Server.Transfer("projects.aspx");
            }
            else
            {
                if (id == 0)  // insert new
                {
                    msg.InnerText += "Project was not created.";
                }
                else // edit existing
                {
                    msg.InnerText += "Project was not updated.";
                }
            }
        }
    }
}
