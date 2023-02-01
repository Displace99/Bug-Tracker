using btnet;
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
        String sql;

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

            Boolean good = true;
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
                if (id == 0)  // insert new
                {
                    sql = @"
insert into orgs
	(og_name,
	og_domain,
	og_active,
	og_non_admins_can_use,
	og_external_user,
	og_can_edit_sql,
	og_can_delete_bug,
	og_can_edit_and_delete_posts,
	og_can_merge_bugs,
	og_can_mass_edit_bugs,
	og_can_use_reports,
	og_can_edit_reports,
	og_can_be_assigned_to,
	og_can_view_tasks,
	og_can_edit_tasks,
	og_can_search,
	og_can_only_see_own_reported,
	og_can_assign_to_internal_users,
	og_other_orgs_permission_level,
	og_project_field_permission_level,
	og_org_field_permission_level,
	og_category_field_permission_level,
	og_tags_field_permission_level,
	og_priority_field_permission_level,
	og_status_field_permission_level,
	og_assigned_to_field_permission_level,
	og_udf_field_permission_level
	$custom1$
	)
	values (
	N'$name', 
	N'$domain',
	$active,
	$non_admins_can_use,
	$external_user,
	$can_edit_sql,
	$can_delete_bug,
	$can_edit_and_delete_posts,
	$can_merge_bugs,
	$can_mass_edit_bugs,
	$can_use_reports,
	$can_edit_reports,
	$can_be_assigned_to,
	$can_view_tasks,
	$can_edit_tasks,
	$can_search,
	$can_only_see_own_reported,
	$can_assign_to_internal_users,
	$other_orgs,
	$flp_project,
	$flp_org,
	$flp_category,
	$flp_tags,
	$flp_priority,
	$flp_status,
	$flp_assigned_to,
	$flp_udf
	$custom2$
)";
                }
                else // edit existing
                {

                    sql = @"
update orgs set
	og_name = N'$name',
	og_domain = N'$domain',
	og_active = $active,
	og_non_admins_can_use = $non_admins_can_use,
	og_external_user = $external_user,
	og_can_edit_sql = $can_edit_sql,
	og_can_delete_bug = $can_delete_bug,
	og_can_edit_and_delete_posts = $can_edit_and_delete_posts,
	og_can_merge_bugs = $can_merge_bugs,
	og_can_mass_edit_bugs = $can_mass_edit_bugs,
	og_can_use_reports = $can_use_reports,
	og_can_edit_reports = $can_edit_reports,
	og_can_be_assigned_to = $can_be_assigned_to,
	og_can_view_tasks = $can_view_tasks,
	og_can_edit_tasks = $can_edit_tasks,
	og_can_search = $can_search,
	og_can_only_see_own_reported = $can_only_see_own_reported,
	og_can_assign_to_internal_users = $can_assign_to_internal_users,
	og_other_orgs_permission_level = $other_orgs,
	og_project_field_permission_level = $flp_project,
	og_org_field_permission_level = $flp_org,
	og_category_field_permission_level = $flp_category,
	og_tags_field_permission_level = $flp_tags,
	og_priority_field_permission_level = $flp_priority,
	og_status_field_permission_level = $flp_status,
	og_assigned_to_field_permission_level = $flp_assigned_to,
	og_udf_field_permission_level = $flp_udf
	$custom3$
	where og_id = $og_id";

                    sql = sql.Replace("$og_id", Convert.ToString(id));

                }

                sql = sql.Replace("$name", og_name.Value.Replace("'", "''"));
                sql = sql.Replace("$domain", og_domain.Value.Replace("'", "''"));
                sql = sql.Replace("$active", Util.bool_to_string(og_active.Checked));
                sql = sql.Replace("$non_admins_can_use", Util.bool_to_string(non_admins_can_use.Checked));
                sql = sql.Replace("$external_user", Util.bool_to_string(external_user.Checked));
                sql = sql.Replace("$can_edit_sql", Util.bool_to_string(can_edit_sql.Checked));
                sql = sql.Replace("$can_delete_bug", Util.bool_to_string(can_delete_bug.Checked));
                sql = sql.Replace("$can_edit_and_delete_posts", Util.bool_to_string(can_edit_and_delete_posts.Checked));
                sql = sql.Replace("$can_merge_bugs", Util.bool_to_string(can_merge_bugs.Checked));
                sql = sql.Replace("$can_mass_edit_bugs", Util.bool_to_string(can_mass_edit_bugs.Checked));
                sql = sql.Replace("$can_use_reports", Util.bool_to_string(can_use_reports.Checked));
                sql = sql.Replace("$can_edit_reports", Util.bool_to_string(can_edit_reports.Checked));
                sql = sql.Replace("$can_be_assigned_to", Util.bool_to_string(can_be_assigned_to.Checked));
                sql = sql.Replace("$can_view_tasks", Util.bool_to_string(can_view_tasks.Checked));
                sql = sql.Replace("$can_edit_tasks", Util.bool_to_string(can_edit_tasks.Checked));
                sql = sql.Replace("$can_search", Util.bool_to_string(can_search.Checked));
                sql = sql.Replace("$can_only_see_own_reported", Util.bool_to_string(can_only_see_own_reported.Checked));
                sql = sql.Replace("$can_assign_to_internal_users", Util.bool_to_string(can_assign_to_internal_users.Checked));
                sql = sql.Replace("$other_orgs", other_orgs.SelectedValue);
                sql = sql.Replace("$flp_project", project_field.SelectedValue);
                sql = sql.Replace("$flp_org", org_field.SelectedValue);
                sql = sql.Replace("$flp_category", category_field.SelectedValue);
                sql = sql.Replace("$flp_tags", tags_field.SelectedValue);
                sql = sql.Replace("$flp_priority", priority_field.SelectedValue);
                sql = sql.Replace("$flp_status", status_field.SelectedValue);
                sql = sql.Replace("$flp_assigned_to", assigned_to_field.SelectedValue);
                sql = sql.Replace("$flp_udf", udf_field.SelectedValue);

                if (id == 0)  // insert new
                {
                    string custom1 = "";
                    string custom2 = "";
                    foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
                    {
                        string bg_name = (string)dr_custom["name"];
                        string og_col_name = "og_"
                            + bg_name
                            + "_field_permission_level";

                        custom1 += ",[" + og_col_name + "]";
                        custom2 += "," + btnet.Util.sanitize_integer(Request[bg_name]);

                    }
                    sql = sql.Replace("$custom1$", custom1);
                    sql = sql.Replace("$custom2$", custom2);
                }
                else
                {
                    string custom3 = "";
                    foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
                    {
                        string bg_name = (string)dr_custom["name"];
                        string og_col_name = "og_"
                            + bg_name
                            + "_field_permission_level";

                        custom3 += ",[" + og_col_name + "]=" + btnet.Util.sanitize_integer(Request[bg_name]);

                    }
                    sql = sql.Replace("$custom3$", custom3);
                }

                btnet.DbUtil.execute_nonquery(sql);
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
