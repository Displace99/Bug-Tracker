using btnet;
using BugTracker.Web.Models.Search;
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
    //public class ProjectDropdown
    //{
    //    public bool enabled = false;
    //    public string label = "";
    //    public string values = "";
    //};

    //public class BtnetProject
    //{
    //    public Dictionary<int, ProjectDropdown> map_dropdowns = new Dictionary<int, ProjectDropdown>();
    //};

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
                    SubmitQuery();
                }
                else
                {
                    dv = (DataView)Session["bugs"];
                    if (dv == null)
                    {
                        SubmitQuery();
                    }
                    call_sort_and_filter_buglist_dataview();
                }
            }

            hit_submit_button.Value = "0";
            project_changed.Value = "0";

        }

       

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

        public List<ListItem> GetSelectedItemsFromListBox(ListBox listBox)
        {
            List<ListItem> selectedItems = new List<ListItem>();
            foreach (ListItem item in listBox.Items)
            {
                if (item.Selected)
                {
                    selectedItems.Add(item);
                }
            }

            return selectedItems;
        }

        public void SubmitQuery()
        {
            prev_sort.Value = "-1";
            prev_dir.Value = "ASC";
            new_page.Value = "0";
            string whereConditionalOperator = and.Checked ? "and " : "or ";

            ExtendedSearch extendedSearch = new ExtendedSearch();

            extendedSearch.DescriptionContainsFilter = like.Value;
            extendedSearch.CommentsContainsFilter = like2.Value;
            extendedSearch.UseFullNames = use_full_names;

            //Get selected items from ListBox Controls
            extendedSearch.ReportedByList = GetSelectedItemsFromListBox(reported_by);
            extendedSearch.AssignedToList = GetSelectedItemsFromListBox(assigned_to);
            extendedSearch.ProjectList = GetSelectedItemsFromListBox(project);
            extendedSearch.ProjectCustomDD1List = GetSelectedItemsFromListBox(project_custom_dropdown1);
            extendedSearch.ProjectCustomDD2List = GetSelectedItemsFromListBox(project_custom_dropdown2);
            extendedSearch.ProjectCustomDD3List = GetSelectedItemsFromListBox(project_custom_dropdown3);
            extendedSearch.OrgList = GetSelectedItemsFromListBox(org);
            extendedSearch.CategoryList = GetSelectedItemsFromListBox(category);
            extendedSearch.PriorityList = GetSelectedItemsFromListBox(priority);
            extendedSearch.StatusList = GetSelectedItemsFromListBox(status);
            extendedSearch.UDFList = new List<ListItem>();

            extendedSearch.IsExternalUser = security.user.external_user;
            extendedSearch.CommentSinceDateString = comments_since.Value;
            extendedSearch.FromDateString = from_date.Value;
            extendedSearch.ToDateString = to_date.Value;
            extendedSearch.LastUpdatedFromDateString = lu_from_date.Value;
            extendedSearch.LastUpdatedToDateString = lu_to_date.Value;

            extendedSearch.CustomColumns = ds_custom_cols;

            extendedSearch.Security = security;
            extendedSearch.MapProjects = map_projects;

            if (show_udf)
            {
                extendedSearch.UDFList = GetSelectedItemsFromListBox(udf);
            }

            DataSet ds = _searchService.BuildSearchQuery(extendedSearch, Request, whereConditionalOperator, security);

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
            //The SQL here is to update a variable on the page side used to populate the info needed to Save the Query.
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
