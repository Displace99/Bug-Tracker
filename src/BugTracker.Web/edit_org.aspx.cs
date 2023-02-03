using btnet;
using BugTracker.Web.Models.Organization;
using BugTracker.Web.Services.Organization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class edit_org : Page
    {
        int id;

        protected Security security;
        protected Dictionary<string, int> dict_custom_field_permission_level = new Dictionary<string, int>();
        protected DataSet ds_custom;

        private OrganizationService _orgService = new OrganizationService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit organization";

            msg.InnerText = "";

            string var = Request.QueryString["id"];
            if (var == null)
            {
                id = 0;
            }
            else
            {
                id = Convert.ToInt32(var);
            }

            //Get list of custom fields
            ds_custom = Util.get_custom_columns();

            if (!IsPostBack)
            {

                // add or edit?
                if (id == 0)
                {
                    //Assign default values
                    sub.Value = "Create";
                    og_active.Checked = true;
                    //other_orgs_permission_level.SelectedIndex = 2;
                    can_search.Checked = true;
                    can_be_assigned_to.Checked = true;
                    other_orgs.SelectedValue = "2";

                    project_field.SelectedValue = "2";
                    org_field.SelectedValue = "2";
                    category_field.SelectedValue = "2";
                    tags_field.SelectedValue = "2";
                    priority_field.SelectedValue = "2";
                    status_field.SelectedValue = "2";
                    assigned_to_field.SelectedValue = "2";
                    udf_field.SelectedValue = "2";

                    //Loop through custom fields and assign default permission levels
                    foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
                    {
                        string bg_name = (string)dr_custom["name"];
                        dict_custom_field_permission_level[bg_name] = 2;
                    }
                }
                else
                {
                    sub.Value = "Update";

                    // Get this entry's data from the db and fill in the form
                    DataRow dr = _orgService.GetOrgDetailsById(id);

                    // Fill in this form
                    og_name.Value = (string)dr["og_name"];
                    og_domain.Value = (string)dr["og_domain2"];
                    og_active.Checked = Convert.ToBoolean((int)dr["og_active"]);
                    non_admins_can_use.Checked = Convert.ToBoolean((int)dr["og_non_admins_can_use"]);
                    external_user.Checked = Convert.ToBoolean((int)dr["og_external_user"]);
                    can_edit_sql.Checked = Convert.ToBoolean((int)dr["og_can_edit_sql"]);
                    can_delete_bug.Checked = Convert.ToBoolean((int)dr["og_can_delete_bug"]);
                    can_edit_and_delete_posts.Checked = Convert.ToBoolean((int)dr["og_can_edit_and_delete_posts"]);
                    can_merge_bugs.Checked = Convert.ToBoolean((int)dr["og_can_merge_bugs"]);
                    can_mass_edit_bugs.Checked = Convert.ToBoolean((int)dr["og_can_mass_edit_bugs"]);
                    can_use_reports.Checked = Convert.ToBoolean((int)dr["og_can_use_reports"]);
                    can_edit_reports.Checked = Convert.ToBoolean((int)dr["og_can_edit_reports"]);
                    can_be_assigned_to.Checked = Convert.ToBoolean((int)dr["og_can_be_assigned_to"]);
                    can_view_tasks.Checked = Convert.ToBoolean((int)dr["og_can_view_tasks"]);
                    can_edit_tasks.Checked = Convert.ToBoolean((int)dr["og_can_edit_tasks"]);
                    can_search.Checked = Convert.ToBoolean((int)dr["og_can_search"]);
                    can_only_see_own_reported.Checked = Convert.ToBoolean((int)dr["og_can_only_see_own_reported"]);
                    can_assign_to_internal_users.Checked = Convert.ToBoolean((int)dr["og_can_assign_to_internal_users"]);

                    other_orgs.SelectedValue = Convert.ToString((int)dr["og_other_orgs_permission_level"]);

                    project_field.SelectedValue = Convert.ToString((int)dr["og_project_field_permission_level"]);
                    org_field.SelectedValue = Convert.ToString((int)dr["og_org_field_permission_level"]);
                    category_field.SelectedValue = Convert.ToString((int)dr["og_category_field_permission_level"]);
                    tags_field.SelectedValue = Convert.ToString((int)dr["og_tags_field_permission_level"]);
                    priority_field.SelectedValue = Convert.ToString((int)dr["og_priority_field_permission_level"]);
                    status_field.SelectedValue = Convert.ToString((int)dr["og_status_field_permission_level"]);
                    assigned_to_field.SelectedValue = Convert.ToString((int)dr["og_assigned_to_field_permission_level"]);
                    udf_field.SelectedValue = Convert.ToString((int)dr["og_udf_field_permission_level"]);

                    foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
                    {
                        string bg_name = (string)dr_custom["name"];
                        object obj = dr["og_" + bg_name + "_field_permission_level"];
                        int permission;
                        if (Convert.IsDBNull(obj))
                        {
                            permission = Security.PERMISSION_ALL;
                        }
                        else
                        {
                            permission = (int)obj;
                        }
                        dict_custom_field_permission_level[bg_name] = permission;
                    }

                }
            }
            else
            {
                foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
                {
                    string bg_name = (string)dr_custom["name"];
                    dict_custom_field_permission_level[bg_name] = Convert.ToInt32(Request[bg_name]);
                }

                UpdateOrg();
            }
        }

        bool ValidateForm()
        {
            bool good = true;
            if (og_name.Value == "")
            {
                good = false;
                name_err.InnerText = "Name is required.";
            }
            else
            {
                name_err.InnerText = "";
            }

            return good;
        }

        void UpdateOrg()
        {

            bool isValid = ValidateForm();

            if (isValid)
            {
                Org org = new Org 
                {
                    Id = id,
                    Name = og_name.Value,
                    Domain = og_domain.Value,
                    IsActive = og_active.Checked,
                    NonAdminsCanUse = non_admins_can_use.Checked,
                    ExternalUser = external_user.Checked,
                    CanEditSQL= can_edit_sql.Checked,
                    CanDeleteBug = can_delete_bug.Checked,
                    CanEditDeleteComment = can_edit_and_delete_posts.Checked,
                    CanMergeBugs = can_merge_bugs.Checked,
                    CanMassEditBugs = can_mass_edit_bugs.Checked,
                    CanUseReports = can_use_reports.Checked,
                    CanEditReports = can_edit_reports.Checked,
                    CanBeAssignedTo = can_be_assigned_to.Checked,
                    CanViewTasks = can_view_tasks.Checked,
                    CanEditTasks = can_edit_tasks.Checked,
                    CanSearch = can_search.Checked,
                    CanOnlySeeOwnReportedBugs = can_only_see_own_reported.Checked,
                    CanAssignToInternalUsers = can_assign_to_internal_users.Checked,
                    OtherOrgPermission = Convert.ToInt32(other_orgs.SelectedValue),
                    ProjectFieldPermission = Convert.ToInt32(project_field.SelectedValue),
                    OrgFieldPermission = Convert.ToInt32(org_field.SelectedValue),
                    CategoryFieldPermission = Convert.ToInt32(category_field.SelectedValue),
                    TagFieldPermission = Convert.ToInt32(tags_field.SelectedValue),
                    PriorityFieldPermission = Convert.ToInt32(priority_field.SelectedValue),
                    StatusFieldPermission = Convert.ToInt32(status_field.SelectedValue),
                    AssignedToFieldPermission = Convert.ToInt32(assigned_to_field.SelectedValue),
                    UserDefinedFieldPermission = Convert.ToInt32(udf_field.SelectedValue)
                };

                List<CustomFieldPermissions> customFieldList = new List<CustomFieldPermissions>();

                foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
                {
                    string bg_name = (string)dr_custom["name"];
                    string og_col_name = "og_"
                        + bg_name
                        + "_field_permission_level";

                    customFieldList.Add(new CustomFieldPermissions { FieldName = og_col_name, PermissionLevel = Convert.ToInt32(Request[bg_name]) });
                }

                org.CustomFieldPermissions = customFieldList;

                if (id == 0)  // insert new
                {
                    _orgService.CreateOrganization(org);
                }
                else
                {
                    _orgService.UpdateOrganizaton(org);
                }

                Server.Transfer("orgs.aspx");
            }
            else
            {
                if (id == 0)  // insert new
                {
                    msg.InnerText = "Organization was not created.";
                }
                else // edit existing
                {
                    msg.InnerText = "Organization was not updated.";
                }
            }
        }
    }
}
