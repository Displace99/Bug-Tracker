using btnet;
using BugTracker.Web.Services.Category;
using BugTracker.Web.Services.CustomFields;
using BugTracker.Web.Services.Organization;
using BugTracker.Web.Services.Priority;
using BugTracker.Web.Services.Project;
using BugTracker.Web.Services.Search;
using BugTracker.Web.Services.Status;
using BugTracker.Web.Services.UserDefinedFields;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public class ProjectDropdown
    {
        public bool enabled = false;
        public string label = "";
        public string values = "";
    };

    public class BtnetProject
    {
        public Dictionary<int, ProjectDropdown> map_dropdowns = new Dictionary<int, ProjectDropdown>();
    };

    public partial class search : Page
    {
        public string sql;
        public DataView dv;
        public DataSet ds_custom_cols = null;
        
		public Security security;
		public bool show_udf;
		public bool use_full_names = false;
        
		public DataTable dt_users = null;
        
        //This is used in the aspx page to build out the SQL on the page
		public string project_dropdown_select_cols = "";

        public Dictionary<int, BtnetProject> map_projects = new Dictionary<int, BtnetProject>();

        private ProjectService _projectService = new ProjectService();
        private OrganizationService _organizationService = new OrganizationService();
        private CategoryService _categoryService = new CategoryService();
        private PriorityService _priorityService = new PriorityService();
        private StatusService _statusService = new StatusService();
        private UserDefinedFieldService _userDefinedFieldService = new UserDefinedFieldService();
        private CustomFieldService _customFieldService = new CustomFieldService(HttpContext.Current);
        private SearchService _searchService = new SearchService();


        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            if (security.user.is_admin || security.user.can_search)
            {
                //Do nothing. They have access
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "search";

            show_udf = (Util.get_setting("ShowUserDefinedBugAttribute", "1") == "1");
            use_full_names = (Util.get_setting("UseFullNames", "0") == "1");

            ds_custom_cols = _customFieldService.GetCustomFields();

            dt_users = Util.get_related_users(security, false);

            if (!IsPostBack)
            {
                load_drop_downs();
                project_custom_dropdown1_label.Style["display"] = "none";
                project_custom_dropdown1.Style["display"] = "none";

                project_custom_dropdown2_label.Style["display"] = "none";
                project_custom_dropdown2.Style["display"] = "none";

                project_custom_dropdown3_label.Style["display"] = "none";
                project_custom_dropdown3.Style["display"] = "none";

                // are there any custom columns in project?
                int projects_with_custom_dropdowns = _projectService.GetProjectsWithCustomColumCount();

                if (projects_with_custom_dropdowns == 0)
                {
                    project.AutoPostBack = false;
                }
            }
            else
            {

                // get the project dropdowns
                DataSet ds_projects = _projectService.GetProjectsWithCustomColumns(); 

                //Loads the custom drop down controls
                foreach (DataRow dr in ds_projects.Tables[0].Rows)
                {
                    BtnetProject btnet_project = new BtnetProject();

                    ProjectDropdown dropdown;

                    dropdown = new ProjectDropdown();
                    dropdown.enabled = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown1"]);
                    dropdown.label = (string)dr["pj_custom_dropdown_label1"];
                    dropdown.values = (string)dr["pj_custom_dropdown_values1"];
                    btnet_project.map_dropdowns[1] = dropdown;

                    dropdown = new ProjectDropdown();
                    dropdown.enabled = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown2"]);
                    dropdown.label = (string)dr["pj_custom_dropdown_label2"];
                    dropdown.values = (string)dr["pj_custom_dropdown_values2"];
                    btnet_project.map_dropdowns[2] = dropdown;

                    dropdown = new ProjectDropdown();
                    dropdown.enabled = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown3"]);
                    dropdown.label = (string)dr["pj_custom_dropdown_label3"];
                    dropdown.values = (string)dr["pj_custom_dropdown_values3"];
                    btnet_project.map_dropdowns[3] = dropdown;

                    map_projects[(int)dr["pj_id"]] = btnet_project;

                }

                // which button did the user hit?
                if (project_changed.Value == "1" && project.AutoPostBack == true)
                {
                    handle_project_custom_dropdowns();
                }
                else if (hit_submit_button.Value == "1")
                {
                    handle_project_custom_dropdowns();
                    do_query();
                }
                else
                {
                    dv = (DataView)Session["bugs"];
                    if (dv == null)
                    {
                        do_query();
                    }
                    call_sort_and_filter_buglist_dataview();
                }
            }

            hit_submit_button.Value = "0";
            project_changed.Value = "0";

        }

        /// <summary>
        /// Takes the list of selected values from a list box and builds part of the where clause 
        /// </summary>
        /// <param name="lb">ListBox element to pull selected values from</param>
        /// <param name="column_name">Name of the database column</param>
        /// <returns>part of the where clause of a sql statement e.x. "in (1,4)"</returns>
        //public string build_clause_from_listbox(ListBox lb, string column_name)
        //{

        //    string clause = "";
        //    foreach (ListItem li in lb.Items)
        //    {
        //        if (li.Selected)
        //        {
        //            if (clause == "")
        //            {
        //                clause += column_name + " in (";
        //            }
        //            else
        //            {
        //                clause += ",";
        //            }

        //            clause += li.Value;

        //        }
        //    }

        //    if (clause != "")
        //    {
        //        clause += ") ";
        //    }

        //    return clause;

        //}

        /// <summary>
        /// Returns a list of of Projects selected in the Project List Item control
        /// </summary>
        /// <returns>List of ListItems</returns>
        public List<ListItem> get_selected_projects()
        {
            List<ListItem> selected_projects = new List<ListItem>();

            foreach (ListItem li in project.Items)
            {
                if (li.Selected)
                {
                    selected_projects.Add(li);
                }
            }

            return selected_projects;
        }

        public List<string> GetSelectedItemsFromListBox(ListBox listBox)
        {
            List<string> selectedItems = new List<string>();
            foreach (ListItem item in listBox.Items)
            {
                if (item.Selected)
                {
                    selectedItems.Add(item.Value);
                }
            }

            return selectedItems;
        }

        /// <summary>
        /// Builds the sql query to do the search. This is built from all of the controls on the page. 
        /// </summary>
        public void do_query()
        {
            prev_sort.Value = "-1";
            prev_dir.Value = "ASC";
            new_page.Value = "0";
            string whereConditionalOperator = and.Checked ? "and " : "or ";

            //Get selected items from ListBox Controls
            List<string> reportedBySelectedItemsList = GetSelectedItemsFromListBox(reported_by);
            List<string> assignedToSelectedItemsList = GetSelectedItemsFromListBox(assigned_to);
            List<string> projectSelectedItemsList = GetSelectedItemsFromListBox(project);
            List<string> projectCustomDD1SelectedItemsList = GetSelectedItemsFromListBox(project_custom_dropdown1);
            List<string> projectCustomDD2SelectedItemsList = GetSelectedItemsFromListBox(project_custom_dropdown2);
            List<string> projectCustomDD3SelectedItemsList = GetSelectedItemsFromListBox(project_custom_dropdown3);
            List<string> orgSelectedItemsList = GetSelectedItemsFromListBox(org);
            List<string> categorySelectedItemsList = GetSelectedItemsFromListBox(category);
            List<string> prioritySelectedItemsList = GetSelectedItemsFromListBox(priority);
            List<string> statusSelectedItemsList = GetSelectedItemsFromListBox(status);
            List<string> udfSelectedItemsList = new List<string>();

            // Create "WHERE" clause

            string where = "";

            //string reported_by_clause = build_clause_from_listbox(reported_by, "bg_reported_user");
            //string assigned_to_clause = build_clause_from_listbox(assigned_to, "bg_assigned_to_user");
            //string project_clause = build_clause_from_listbox(project, "bg_project");

            //string project_custom_dropdown1_clause
            //    = build_clause_from_listbox(project_custom_dropdown1, "bg_project_custom_dropdown_value1");
            //string project_custom_dropdown2_clause
            //    = build_clause_from_listbox(project_custom_dropdown2, "bg_project_custom_dropdown_value2");
            //string project_custom_dropdown3_clause
            //    = build_clause_from_listbox(project_custom_dropdown3, "bg_project_custom_dropdown_value3");

            //string org_clause = build_clause_from_listbox(org, "bg_org");
            //string category_clause = build_clause_from_listbox(category, "bg_category");
            //string priority_clause = build_clause_from_listbox(priority, "bg_priority");
            //string status_clause = build_clause_from_listbox(status, "bg_status");

            string reported_by_clause = _searchService.BuildWhereFromList(reportedBySelectedItemsList, "bg_reported_user");
            string assigned_to_clause = _searchService.BuildWhereFromList(assignedToSelectedItemsList, "bg_assigned_to_user");
            string project_clause = _searchService.BuildWhereFromList(projectSelectedItemsList, "bg_project");

            string project_custom_dropdown1_clause
                = _searchService.BuildWhereFromList(projectCustomDD1SelectedItemsList, "bg_project_custom_dropdown_value1");
            string project_custom_dropdown2_clause
                = _searchService.BuildWhereFromList(projectCustomDD2SelectedItemsList, "bg_project_custom_dropdown_value2");
            string project_custom_dropdown3_clause
                = _searchService.BuildWhereFromList(projectCustomDD3SelectedItemsList, "bg_project_custom_dropdown_value3");

            string org_clause = _searchService.BuildWhereFromList(orgSelectedItemsList, "bg_org");
            string category_clause = _searchService.BuildWhereFromList(categorySelectedItemsList, "bg_category");
            string priority_clause = _searchService.BuildWhereFromList(prioritySelectedItemsList, "bg_priority");
            string status_clause = _searchService.BuildWhereFromList(statusSelectedItemsList, "bg_status");
            string udf_clause = "";

            if (show_udf)
            {
                udfSelectedItemsList = GetSelectedItemsFromListBox(udf);
                udf_clause = _searchService.BuildWhereFromList(udfSelectedItemsList, "bg_user_defined_attribute");
            }


            // SQL "LIKE" uses [, %, and _ in a special way

            string like_string = like.Value.Replace("'", "''");
            like_string = like_string.Replace("[", "[[]");
            like_string = like_string.Replace("%", "[%]");
            like_string = like_string.Replace("_", "[_]");

            string like2_string = like2.Value.Replace("'", "''");
            like2_string = like2_string.Replace("[", "[[]");
            like2_string = like2_string.Replace("%", "[%]");
            like2_string = like2_string.Replace("_", "[_]");

            string desc_clause = "";
            if (like.Value != "")
            {
                desc_clause = " bg_short_desc like";
                desc_clause += " N'%" + like_string + "%'\n";
            }

            string comments_clause = "";
            if (like2.Value != "")
            {
                comments_clause = " bg_id in (select bp_bug from bug_posts where bp_type in ('comment','received','sent') and isnull(bp_comment_search,bp_comment) like";
                comments_clause += " N'%" + like2_string + "%'";
                if (security.user.external_user)
                {
                    comments_clause += " and bp_hidden_from_external_users = 0";
                }
                comments_clause += ")\n";
            }


            string comments_since_clause = "";
            if (comments_since.Value != "")
            {
                comments_since_clause = " bg_id in (select bp_bug from bug_posts where bp_type in ('comment','received','sent') and bp_date > '";
                comments_since_clause += _searchService.FormatToDate(comments_since.Value);
                comments_since_clause += "')\n";
            }

            string from_clause = "";
            if (from_date.Value != "")
            {
                from_clause = " bg_reported_date >= '" + _searchService.FormatFromDate(from_date.Value) + "'\n";
            }

            string to_clause = "";
            if (to_date.Value != "")
            {
                to_clause = " bg_reported_date <= '" + _searchService.FormatToDate(to_date.Value) + "'\n";
            }

            string lu_from_clause = "";
            if (lu_from_date.Value != "")
            {
                lu_from_clause = " bg_last_updated_date >= '" + _searchService.FormatFromDate(lu_from_date.Value) + "'\n";
            }

            string lu_to_clause = "";
            if (lu_to_date.Value != "")
            {
                lu_to_clause = " bg_last_updated_date <= '" + _searchService.FormatToDate(lu_to_date.Value) + "'\n";
            }


            where = _searchService.BuildWhere(where, reported_by_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, assigned_to_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, project_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, project_custom_dropdown1_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, project_custom_dropdown2_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, project_custom_dropdown3_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, org_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, category_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, priority_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, status_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, desc_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, comments_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, comments_since_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, from_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, to_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, lu_from_clause, whereConditionalOperator);
            where = _searchService.BuildWhere(where, lu_to_clause, whereConditionalOperator);

            if (show_udf)
            {
                where = _searchService.BuildWhere(where, udf_clause, whereConditionalOperator);
            }

            foreach (DataRow drcc in ds_custom_cols.Tables[0].Rows)
            {
                string column_name = (string)drcc["name"];
                if (security.user.dict_custom_field_permission_level[column_name] == Security.PERMISSION_NONE)
                {
                    continue;
                }

                string values = Request[column_name];

                if (values != null)
                {

                    values = values.Replace("'", "''");

                    string custom_clause = "";

                    string datatype = (string)drcc["datatype"];

                    if ((datatype == "varchar" || datatype == "nvarchar" || datatype == "char" || datatype == "nchar")
                    && (string)drcc["dropdown type"] == "")
                    {
                        if (values != "")
                        {
                            custom_clause = " [" + column_name + "] like '%" + values + "%'\n";
                            where = _searchService.BuildWhere(where, custom_clause, whereConditionalOperator);
                        }
                    }
                    else if (datatype == "datetime")
                    {
                        if (values != "")
                        {
                            custom_clause = " [" + column_name + "] >= '" + _searchService.FormatFromDate(values) + "'\n";
                            where = _searchService.BuildWhere(where, custom_clause, whereConditionalOperator);

                            // reset, and do the to date
                            custom_clause = "";
                            values = Request["to__" + column_name];
                            if (values != "")
                            {
                                custom_clause = " [" + column_name + "] <= '" + _searchService.FormatToDate(values) + "'\n";
                                where = _searchService.BuildWhere(where, custom_clause, whereConditionalOperator);
                            }
                        }
                    }
                    else
                    {
                        if (values == "" && (datatype == "int" || datatype == "decimal"))
                        {
                            // skip
                        }
                        else
                        {
                            string in_not_in = _searchService.FormatInNotIn(values);
                            custom_clause = " [" + column_name + "] in " + in_not_in + "\n";
                            where = _searchService.BuildWhere(where, custom_clause, whereConditionalOperator);
                        }
                    }
                }
            }

            //Checks to see if there is a default select statement in the webconfig. By default this is commented out
            //and only used if you want to customize the columns that are displayed on the "search" page
            string search_sql = Util.get_setting("SearchSQL", "");

            //If nothing is is found in the web config, build out the standard select statement.
            if (search_sql == "")
            {

                string select = "select isnull(pr_background_color,'#ffffff') [color], bg_id [id],\nbg_short_desc [desc]";

                // reported
                if (use_full_names)
                {
                    select += "\n,isnull(rpt.us_lastname + ', ' + rpt.us_firstname,'') [reported by]";
                }
                else
                {
                    select += "\n,isnull(rpt.us_username,'') [reported by]";
                }
                select += "\n,bg_reported_date [reported on]";

                // last updated
                if (use_full_names)
                {
                    select += "\n,isnull(lu.us_lastname + ', ' + lu.us_firstname,'') [last updated by]";
                }
                else
                {
                    select += "\n,isnull(lu.us_username,'') [last updated by]";
                }
                select += "\n,bg_last_updated_date [last updated on]";


                if (security.user.tags_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(bg_tags,'') [tags]";
                }

                if (security.user.project_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(pj_name,'') [project]";
                }

                if (security.user.org_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(og_name,'') [organization]";
                }

                if (security.user.category_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(ct_name,'') [category]";
                }

                if (security.user.priority_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(pr_name,'') [priority]";
                }

                if (security.user.assigned_to_field_permission_level != Security.PERMISSION_NONE)
                {
                    if (use_full_names)
                    {
                        select += ",\nisnull(asg.us_lastname + ', ' + asg.us_firstname,'') [assigned to]";
                    }
                    else
                    {
                        select += ",\nisnull(asg.us_username,'') [assigned to]";
                    }
                }

                if (security.user.status_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(st_name,'') [status]";
                }

                if (security.user.udf_field_permission_level != Security.PERMISSION_NONE)
                {
                    if (show_udf)
                    {
                        string udf_name = Util.get_setting("UserDefinedBugAttributeName", "YOUR ATTRIBUTE");
                        select += ",\nisnull(udf_name,'') [" + udf_name + "]";
                    }
                }

                // let results include custom columns
                string custom_cols_sql = "";
                int user_type_cnt = 1;
                foreach (DataRow drcc in ds_custom_cols.Tables[0].Rows)
                {
                    string column_name = (string)drcc["name"];
                    if (security.user.dict_custom_field_permission_level[column_name] == Security.PERMISSION_NONE)
                    {
                        continue;
                    }

                    if (Convert.ToString(drcc["dropdown type"]) == "users")
                    {
                        custom_cols_sql += ",\nisnull(users"
                            + Convert.ToString(user_type_cnt++)
                            + ".us_username,'') "
                            + "["
                            + column_name + "]";
                    }
                    else
                    {
                        if (Convert.ToString(drcc["datatype"]) == "decimal")
                        {
                            custom_cols_sql += ",\nisnull(["
                                + column_name
                                + "],0)["
                                + column_name + "]";
                        }
                        else
                        {
                            custom_cols_sql += ",\nisnull(["
                                + column_name
                                + "],'')["
                                + column_name + "]";
                        }
                    }
                }

                select += custom_cols_sql;

                // Handle project custom dropdowns
                List<ListItem> selected_projects = get_selected_projects();

                string project_dropdown_select_cols_server_side = "";

                string alias1 = null;
                string alias2 = null;
                string alias3 = null;

                foreach (ListItem selected_project in selected_projects)
                {
                    if (selected_project.Value == "0")
                        continue;

                    int pj_id = Convert.ToInt32(selected_project.Value);

                    if (map_projects.ContainsKey(pj_id))
                    {

                        BtnetProject btnet_project = map_projects[pj_id];

                        if (btnet_project.map_dropdowns[1].enabled)
                        {
                            if (alias1 == null)
                            {
                                alias1 = btnet_project.map_dropdowns[1].label;
                            }
                            else
                            {
                                alias1 = "dropdown1";
                            }
                        }

                        if (btnet_project.map_dropdowns[2].enabled)
                        {
                            if (alias2 == null)
                            {
                                alias2 = btnet_project.map_dropdowns[2].label;
                            }
                            else
                            {
                                alias2 = "dropdown2";
                            }
                        }

                        if (btnet_project.map_dropdowns[3].enabled)
                        {
                            if (alias3 == null)
                            {
                                alias3 = btnet_project.map_dropdowns[3].label;
                            }
                            else
                            {
                                alias3 = "dropdown3";
                            }
                        }


                    }
                }

                if (alias1 != null)
                {
                    project_dropdown_select_cols_server_side
                        += ",\nisnull(bg_project_custom_dropdown_value1,'') [" + alias1 + "]";
                }
                if (alias2 != null)
                {
                    project_dropdown_select_cols_server_side
                        += ",\nisnull(bg_project_custom_dropdown_value2,'') [" + alias2 + "]";
                }
                if (alias3 != null)
                {
                    project_dropdown_select_cols_server_side
                        += ",\nisnull(bg_project_custom_dropdown_value3,'') [" + alias3 + "]";
                }

                select += project_dropdown_select_cols_server_side;

                select += @" from bugs
			left outer join users rpt on rpt.us_id = bg_reported_user
			left outer join users lu on lu.us_id = bg_last_updated_user
			left outer join users asg on asg.us_id = bg_assigned_to_user
			left outer join projects on pj_id = bg_project
			left outer join orgs on og_id = bg_org
			left outer join categories on ct_id = bg_category
			left outer join priorities on pr_id = bg_priority
			left outer join statuses on st_id = bg_status
			";

                user_type_cnt = 1;
                foreach (DataRow drcc in ds_custom_cols.Tables[0].Rows)
                {

                    string column_name = (string)drcc["name"];
                    if (security.user.dict_custom_field_permission_level[column_name] == Security.PERMISSION_NONE)
                    {
                        continue;
                    }

                    if (Convert.ToString(drcc["dropdown type"]) == "users")
                    {
                        select += "left outer join users users"
                            + Convert.ToString(user_type_cnt)
                            + " on users"
                            + Convert.ToString(user_type_cnt)
                            + ".us_id = bugs."
                            + "[" + column_name + "]\n";

                        user_type_cnt++;
                    }
                }

                if (show_udf)
                {
                    select += "left outer join user_defined_attribute on udf_id = bg_user_defined_attribute";
                }

                //Adds the where clause to the select statement to complete the sql statement
                sql = select + where + " order by bg_id desc";

            }
            else
            {
                search_sql = search_sql.Replace("[br]", "\n");
                sql = search_sql.Replace("$WHERE$", where);
            }

            sql = Util.alter_sql_per_project_permissions(sql, security);

            DataSet ds = btnet.DbUtil.get_dataset(sql);
            dv = new DataView(ds.Tables[0]);
            Session["bugs"] = dv;
            Session["bugs_unfiltered"] = ds.Tables[0];
        }

        /// <summary>
        /// Loads a list box control with values for the selected projects custom dropdown
        /// There will only be two ListBoxes shown on the page so values from multiple dropdowns will appear in a single ListBox
        /// </summary>
        /// <param name="dropdown">ListBox control for a projects custom dropdown</param>
        /// <param name="vals_string">Values of the custom dropdown</param>
        /// <param name="duplicate_detection_dictionary">Dictionary that holds duplicate values</param>
        public void load_project_custom_dropdown(ListBox dropdown, string vals_string, Dictionary<String, String> duplicate_detection_dictionary)
        {
            string[] vals_array = btnet.Util.split_dropdown_vals(vals_string);
            for (int i = 0; i < vals_array.Length; i++)
            {
                if (!duplicate_detection_dictionary.ContainsKey(vals_array[i]))
                {
                    dropdown.Items.Add(new ListItem(vals_array[i], "'" + vals_array[i].Replace("'", "''") + "'"));
                    duplicate_detection_dictionary.Add(vals_array[i], vals_array[i]);
                }
            }
        }

        /// <summary>
        /// Finds and loads the list of custom project dropdowns based on the selected projects
        /// </summary>
        public void handle_project_custom_dropdowns()
        {

            // How many projects selected?
            List<ListItem> selected_projects = get_selected_projects();
            Dictionary<String, String>[] dupe_detection_dictionaries = new Dictionary<String, String>[3];
            Dictionary<String, String>[] previous_selection_dictionaries = new Dictionary<String, String>[3];
            for (int i = 0; i < dupe_detection_dictionaries.Length; i++)
            {
                // Initialize Dictionary to accumulate ListItem values as they are added to the ListBox
                // so that duplicate values from multiple projects are not added to the ListBox twice.
                dupe_detection_dictionaries[i] = new Dictionary<String, String>();

                previous_selection_dictionaries[i] = new Dictionary<String, String>();
            }

            // Preserve user's previous selections (necessary if this is called during a postback).
            foreach (ListItem li in project_custom_dropdown1.Items)
            {
                if (li.Selected)
                {
                    previous_selection_dictionaries[0].Add(li.Value, li.Value);
                }
            }
            foreach (ListItem li in project_custom_dropdown2.Items)
            {
                if (li.Selected)
                {
                    previous_selection_dictionaries[1].Add(li.Value, li.Value);
                }
            }
            foreach (ListItem li in project_custom_dropdown3.Items)
            {
                if (li.Selected)
                {
                    previous_selection_dictionaries[2].Add(li.Value, li.Value);
                }
            }

            project_dropdown_select_cols = "";

            project_custom_dropdown1_label.InnerText = "";
            project_custom_dropdown2_label.InnerText = "";
            project_custom_dropdown3_label.InnerText = "";

            project_custom_dropdown1.Items.Clear();
            project_custom_dropdown2.Items.Clear();
            project_custom_dropdown3.Items.Clear();

            //For each project selected, get a list of any project specific dropdowns and populate the listbox
            foreach (ListItem selected_project in selected_projects)
            {

                if (selected_project.Value == "0")
                    continue;

                int pj_id = Convert.ToInt32(selected_project.Value);

                if (map_projects.ContainsKey(pj_id))
                {

                    BtnetProject btnet_project = map_projects[pj_id];

                    if (btnet_project.map_dropdowns[1].enabled)
                    {
                        if (project_custom_dropdown1_label.InnerText == "")
                        {
                            project_custom_dropdown1_label.InnerText = btnet_project.map_dropdowns[1].label;
                            project_custom_dropdown1_label.Style["display"] = "inline";
                            project_custom_dropdown1.Style["display"] = "block";
                        }
                        else if (project_custom_dropdown1_label.InnerText != btnet_project.map_dropdowns[1].label)
                        {
                            project_custom_dropdown1_label.InnerText = "dropdown1";
                        }
                        load_project_custom_dropdown(project_custom_dropdown1, btnet_project.map_dropdowns[1].values, dupe_detection_dictionaries[0]);
                    }

                    if (btnet_project.map_dropdowns[2].enabled)
                    {
                        if (project_custom_dropdown2_label.InnerText == "")
                        {
                            project_custom_dropdown2_label.InnerText = btnet_project.map_dropdowns[2].label;
                            project_custom_dropdown2_label.Style["display"] = "inline";
                            project_custom_dropdown2.Style["display"] = "block";
                        }
                        else if (project_custom_dropdown2_label.InnerText != btnet_project.map_dropdowns[2].label)
                        {
                            project_custom_dropdown2_label.InnerText = "dropdown2";
                        }
                        load_project_custom_dropdown(project_custom_dropdown2, btnet_project.map_dropdowns[2].values, dupe_detection_dictionaries[1]);
                    }

                    if (btnet_project.map_dropdowns[3].enabled)
                    {
                        if (project_custom_dropdown3_label.InnerText == "")
                        {
                            project_custom_dropdown3_label.InnerText = btnet_project.map_dropdowns[3].label;
                            project_custom_dropdown3_label.Style["display"] = "inline";
                            project_custom_dropdown3.Style["display"] = "block";
                            load_project_custom_dropdown(project_custom_dropdown3, btnet_project.map_dropdowns[3].values, dupe_detection_dictionaries[2]);
                        }
                        else if (project_custom_dropdown3_label.InnerText != btnet_project.map_dropdowns[3].label)
                        {
                            project_custom_dropdown3_label.InnerText = "dropdown3";
                        }
                        load_project_custom_dropdown(project_custom_dropdown3, btnet_project.map_dropdowns[3].values, dupe_detection_dictionaries[2]);
                    }
                }
            }

            //Chose to show or hide the dropdown list. This is based off of the label which is set in the above loop.
            //If we deselect a project we need to be able to hide the listbbox for any custom dropdowns that no longer exist 
            if (project_custom_dropdown1_label.InnerText == "")
            {
                project_custom_dropdown1.Items.Clear();
                project_custom_dropdown1_label.Style["display"] = "none";
                project_custom_dropdown1.Style["display"] = "none";
            }
            else
            {
                project_custom_dropdown1_label.Style["display"] = "inline";
                project_custom_dropdown1.Style["display"] = "block";
                project_dropdown_select_cols
                    += ",\\nisnull(bg_project_custom_dropdown_value1,'') [" + project_custom_dropdown1_label.InnerText + "]";
            }

            if (project_custom_dropdown2_label.InnerText == "")
            {
                project_custom_dropdown2.Items.Clear();
                project_custom_dropdown2_label.Style["display"] = "none";
                project_custom_dropdown2.Style["display"] = "none";
            }
            else
            {
                project_custom_dropdown2_label.Style["display"] = "inline";
                project_custom_dropdown2.Style["display"] = "block";
                project_dropdown_select_cols
                    += ",\\nisnull(bg_project_custom_dropdown_value2,'') [" + project_custom_dropdown2_label.InnerText + "]";
            }

            if (project_custom_dropdown3_label.InnerText == "")
            {
                project_custom_dropdown3.Items.Clear();
                project_custom_dropdown3_label.Style["display"] = "none";
                project_custom_dropdown3.Style["display"] = "none";
            }
            else
            {
                project_custom_dropdown3_label.Style["display"] = "inline";
                project_custom_dropdown3.Style["display"] = "block";
                project_dropdown_select_cols
                    += ",\\nisnull(bg_project_custom_dropdown_value3,'') [" + project_custom_dropdown3_label.InnerText + "]";
            }

            // Since this method fires on auto postback, we want to preserve the users previous selections
            foreach (ListItem li in project_custom_dropdown1.Items)
            {
                li.Selected = (previous_selection_dictionaries[0].ContainsKey(li.Value));
            }
            foreach (ListItem li in project_custom_dropdown2.Items)
            {
                li.Selected = (previous_selection_dictionaries[1].ContainsKey(li.Value));
            }
            foreach (ListItem li in project_custom_dropdown3.Items)
            {
                li.Selected = (previous_selection_dictionaries[2].ContainsKey(li.Value));
            }
        }

        /// <summary>
        /// Loads standard drop downs related to all bugs
        /// </summary>
        public void load_drop_downs()
        {

            reported_by.DataSource = dt_users;
            reported_by.DataTextField = "us_username";
            reported_by.DataValueField = "us_id";
            reported_by.DataBind();

            assigned_to.DataSource = dt_users;
            assigned_to.DataTextField = "us_username";
            assigned_to.DataValueField = "us_id";
            assigned_to.DataBind();
            assigned_to.Items.Insert(0, new ListItem("[not assigned]", "0"));


            //Only show projects where user has permissions
            DataView projectDVList = _projectService.GetProjectListByPermission(security.user.is_admin, security.user.usid);
            
            DataView orgDVList = _organizationService.GetOrganizationListByPermission(security.user.other_orgs_permission_level, security.user.org);
            DataView categoryDVList = _categoryService.GetCategoryListForSearch();
            DataView priorityDVList = _priorityService.GetPriorityListForSearch();
            DataView statusDVList = _statusService.GetStatusListForSearch();
            DataView udfDVList = _userDefinedFieldService.GetFieldListForSearch();

            if (security.user.other_orgs_permission_level == 0)
            {
                org.Visible = false;
                org_label.Visible = false;
            }

            project.DataSource = projectDVList; 
            project.DataTextField = "pj_name";
            project.DataValueField = "pj_id";
            project.DataBind();
            project.Items.Insert(0, new ListItem("[no project]", "0"));

            org.DataSource = orgDVList; 
            org.DataTextField = "og_name";
            org.DataValueField = "og_id";
            org.DataBind();
            org.Items.Insert(0, new ListItem("[no organization]", "0"));

            category.DataSource = categoryDVList; 
            category.DataTextField = "ct_name";
            category.DataValueField = "ct_id";
            category.DataBind();
            category.Items.Insert(0, new ListItem("[no category]", "0"));

            priority.DataSource = priorityDVList; 
            priority.DataTextField = "pr_name";
            priority.DataValueField = "pr_id";
            priority.DataBind();
            priority.Items.Insert(0, new ListItem("[no priority]", "0"));

            status.DataSource = statusDVList; 
            status.DataTextField = "st_name";
            status.DataValueField = "st_id";
            status.DataBind();
            status.Items.Insert(0, new ListItem("[no status]", "0"));

            if (show_udf)
            {
                udf.DataSource = udfDVList; 
                udf.DataTextField = "udf_name";
                udf.DataValueField = "udf_id";
                udf.DataBind();
                udf.Items.Insert(0, new ListItem("[none]", "0"));
            }

            if (security.user.project_field_permission_level == Security.PERMISSION_NONE)
            {
                project_label.Style["display"] = "none";
                project.Style["display"] = "none";
            }
            if (security.user.org_field_permission_level == Security.PERMISSION_NONE)
            {
                org_label.Style["display"] = "none";
                org.Style["display"] = "none";
            }
            if (security.user.category_field_permission_level == Security.PERMISSION_NONE)
            {
                category_label.Style["display"] = "none";
                category.Style["display"] = "none";
            }
            if (security.user.priority_field_permission_level == Security.PERMISSION_NONE)
            {
                priority_label.Style["display"] = "none";
                priority.Style["display"] = "none";
            }
            if (security.user.status_field_permission_level == Security.PERMISSION_NONE)
            {
                status_label.Style["display"] = "none";
                status.Style["display"] = "none";
            }
            if (security.user.assigned_to_field_permission_level == Security.PERMISSION_NONE)
            {
                assigned_to_label.Style["display"] = "none";
                assigned_to.Style["display"] = "none";
            }
            if (security.user.udf_field_permission_level == Security.PERMISSION_NONE)
            {
                udf_label.Style["display"] = "none";
                udf.Style["display"] = "none";
            }

        }

        /// <summary>
        /// Adds a javascript calendar control to the UI element 
        /// </summary>
        /// <param name="name"></param>
        public void write_custom_date_control(string name)
        {

            Response.Write("<input type=text class='txt date'");
            Response.Write("  onkeyup=\"on_change()\" ");
            int size = 10;
            string size_string = Convert.ToString(size);

            Response.Write(" size=" + size_string);
            Response.Write(" maxlength=" + size_string);

            Response.Write(" name=\"" + name + "\"");
            Response.Write(" id=\"" + name.Replace(" ", "") + "\"");

            Response.Write(" value=\"");
            if (Request[name] != "")
            {
                Response.Write(HttpUtility.HtmlEncode(Request[name]));
            }
            Response.Write("\"");
            Response.Write(">");

            Response.Write("<a style='font-size: 8pt;'  href=\"javascript:show_calendar('");
            Response.Write(name.Replace(" ", ""));
            Response.Write("')\">&nbsp;[select]</a>");
        }

        /// <summary>
        /// Called from the aspx page to add date controls to page.
        /// </summary>
        /// <param name="name"></param>
        public void write_custom_date_controls(string name)
        {
            Response.Write("from:&nbsp;&nbsp;");
            write_custom_date_control(name);
            Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;to:&nbsp;&nbsp;");
            write_custom_date_control("to__" + name); // magic
        }

        /// <summary>
        /// Called by the aspx page to display search results of query
        /// </summary>
        /// <param name="show_checkboxes"></param>
        public void display_bugs(bool show_checkboxes)
		{
			btnet.BugList.display_bugs(
				show_checkboxes,
				dv,
				Response,
				security,
				new_page.Value,
				IsPostBack,
				ds_custom_cols,
				filter.Value);
		}

        /// <summary>
        /// Sorts and filters the bug query results table
        /// </summary>
		public void call_sort_and_filter_buglist_dataview()
		{
			string filter_val = filter.Value;
			string sort_val = sort.Value;
			string prev_sort_val = prev_sort.Value;
			string prev_dir_val = prev_dir.Value;


			btnet.BugList.sort_and_filter_buglist_dataview(dv, IsPostBack,
				actn.Value,
				ref filter_val,
				ref sort_val,
				ref prev_sort_val,
				ref prev_dir_val);

			filter.Value = filter_val;
			sort.Value = sort_val;
			prev_sort.Value = prev_sort_val;
			prev_dir.Value = prev_dir_val;

		}

	}
	
}
