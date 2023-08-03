using btnet;
using BugTracker.Web.Models.Organization;
using BugTracker.Web.Models.Priority;
using BugTracker.Web.Models.Search;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using static Lucene.Net.Index.CheckIndex;

namespace BugTracker.Web.Services.Search
{
    public class SearchService
    {
        public string GetSavedQuery(int queryId)
        {
            // use sql specified in query string
            
            string sql = @"select qu_sql from queries where qu_id = @queryId";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@queryId", queryId);

            string bug_sql = (string)btnet.DbUtil.execute_scalar(cmd);

            return bug_sql;
        }

        public DataSet FindBugsFromSavedQuery(int queryId, int userId, Security security)
        {
            string sql = GetSavedQuery(queryId);
            
            sql = sql.Replace("$ME", userId.ToString());
            sql = Util.alter_sql_per_project_permissions(sql, security);

            // all we really need is the bugid, but let's do the same query as print_bugs.aspx
            return DbUtil.get_dataset(sql);
        }

        #region SearchQueryBuilder

        public DataSet BuildSearchQuery(ExtendedSearch extendedSearch, HttpRequest request, string condOperator, Security security)
        {
            string sql = string.Empty;
            string whereConditionalOperator = condOperator.ToLower();

            //TODO: Remove this and replace with parameters
            //Get selected items from ListBox Controls
            //List<ListItem> reportedBySelectedItemsList = GetSelectedItemsFromListBox(reported_by);
            //List<ListItem> assignedToSelectedItemsList = GetSelectedItemsFromListBox(assigned_to);
            //List<ListItem> projectSelectedItemsList = GetSelectedItemsFromListBox(project);
            //List<ListItem> projectCustomDD1SelectedItemsList = GetSelectedItemsFromListBox(project_custom_dropdown1);
            //List<ListItem> projectCustomDD2SelectedItemsList = GetSelectedItemsFromListBox(project_custom_dropdown2);
            //List<ListItem> projectCustomDD3SelectedItemsList = GetSelectedItemsFromListBox(project_custom_dropdown3);
            //List<ListItem> orgSelectedItemsList = GetSelectedItemsFromListBox(org);
            //List<ListItem> categorySelectedItemsList = GetSelectedItemsFromListBox(category);
            //List<ListItem> prioritySelectedItemsList = GetSelectedItemsFromListBox(priority);
            //List<ListItem> statusSelectedItemsList = GetSelectedItemsFromListBox(status);
            //List<ListItem> udfSelectedItemsList = new List<ListItem>();

            bool showUDF = extendedSearch.UDFList.Count > 0;

            // Create "WHERE" clause
            string where = "";

            string reported_by_clause = BuildWhereFromList(extendedSearch.ReportedByList, "bg_reported_user");
            string assigned_to_clause = BuildWhereFromList(extendedSearch.AssignedToList, "bg_assigned_to_user");
            string project_clause = BuildWhereFromList(extendedSearch.ProjectList, "bg_project");

            string project_custom_dropdown1_clause
                = BuildWhereFromList(extendedSearch.ProjectCustomDD1List, "bg_project_custom_dropdown_value1");
            string project_custom_dropdown2_clause
                = BuildWhereFromList(extendedSearch.ProjectCustomDD2List, "bg_project_custom_dropdown_value2");
            string project_custom_dropdown3_clause
                = BuildWhereFromList(extendedSearch.ProjectCustomDD3List, "bg_project_custom_dropdown_value3");

            string org_clause = BuildWhereFromList(extendedSearch.OrgList, "bg_org");
            string category_clause = BuildWhereFromList(extendedSearch.CategoryList, "bg_category");
            string priority_clause = BuildWhereFromList(extendedSearch.PriorityList, "bg_priority");
            string status_clause = BuildWhereFromList(extendedSearch.StatusList, "bg_status");
            string udf_clause = "";

            //if (show_udf)
            //{
            //    udfSelectedItemsList = GetSelectedItemsFromListBox(udf);
            //    udf_clause = BuildWhereFromList(udfSelectedItemsList, "bg_user_defined_attribute");
            //}

            if (showUDF)
            {
                udf_clause = BuildWhereFromList(extendedSearch.UDFList, "bg_user_defined_attribute");
            }

            // SQL "LIKE" uses [, %, and _ in a special way

            string like_string = extendedSearch.DescriptionContainsFilter;
            like_string = like_string.Replace("'", "''");
            like_string = like_string.Replace("[", "[[]");
            like_string = like_string.Replace("%", "[%]");
            like_string = like_string.Replace("_", "[_]");

            string like2_string = extendedSearch.CommentsContainsFilter;
            like2_string = like2_string.Replace("'", "''");
            like2_string = like2_string.Replace("[", "[[]");
            like2_string = like2_string.Replace("%", "[%]");
            like2_string = like2_string.Replace("_", "[_]");

            string desc_clause = "";
            if (like_string != "")
            {
                desc_clause = " bg_short_desc like";
                desc_clause += " N'%" + like_string + "%'\n";
            }

            string comments_clause = "";
            if (like2_string != "")
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
            if (extendedSearch.CommentSinceDateString != "")
            {
                comments_since_clause = " bg_id in (select bp_bug from bug_posts where bp_type in ('comment','received','sent') and bp_date > '";
                comments_since_clause += FormatToDate(extendedSearch.CommentSinceDateString);
                comments_since_clause += "')\n";
            }

            string from_clause = "";
            if (extendedSearch.FromDateString != "")
            {
                from_clause = " bg_reported_date >= '" + FormatFromDate(extendedSearch.FromDateString) + "'\n";
            }

            string to_clause = "";
            if (extendedSearch.ToDateString != "")
            {
                to_clause = " bg_reported_date <= '" + FormatToDate(extendedSearch.ToDateString) + "'\n";
            }

            string lu_from_clause = "";
            if (extendedSearch.LastUpdatedFromDateString != "")
            {
                lu_from_clause = " bg_last_updated_date >= '" + FormatFromDate(extendedSearch.LastUpdatedFromDateString) + "'\n";
            }

            string lu_to_clause = "";
            if (extendedSearch.LastUpdatedToDateString != "")
            {
                lu_to_clause = " bg_last_updated_date <= '" + FormatToDate(extendedSearch.LastUpdatedToDateString) + "'\n";
            }


            where = BuildWhere(where, reported_by_clause, whereConditionalOperator);
            where = BuildWhere(where, assigned_to_clause, whereConditionalOperator);
            where = BuildWhere(where, project_clause, whereConditionalOperator);
            where = BuildWhere(where, project_custom_dropdown1_clause, whereConditionalOperator);
            where = BuildWhere(where, project_custom_dropdown2_clause, whereConditionalOperator);
            where = BuildWhere(where, project_custom_dropdown3_clause, whereConditionalOperator);
            where = BuildWhere(where, org_clause, whereConditionalOperator);
            where = BuildWhere(where, category_clause, whereConditionalOperator);
            where = BuildWhere(where, priority_clause, whereConditionalOperator);
            where = BuildWhere(where, status_clause, whereConditionalOperator);
            where = BuildWhere(where, desc_clause, whereConditionalOperator);
            where = BuildWhere(where, comments_clause, whereConditionalOperator);
            where = BuildWhere(where, comments_since_clause, whereConditionalOperator);
            where = BuildWhere(where, from_clause, whereConditionalOperator);
            where = BuildWhere(where, to_clause, whereConditionalOperator);
            where = BuildWhere(where, lu_from_clause, whereConditionalOperator);
            where = BuildWhere(where, lu_to_clause, whereConditionalOperator);

            if (showUDF)
            {
                where = BuildWhere(where, udf_clause, whereConditionalOperator);
            }

            foreach (DataRow drcc in extendedSearch.CustomColumns.Tables[0].Rows)
            {
                string column_name = (string)drcc["name"];
                if (security.user.dict_custom_field_permission_level[column_name] == Security.PERMISSION_NONE)
                {
                    continue;
                }

                string values = request[column_name];

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
                            where = BuildWhere(where, custom_clause, whereConditionalOperator);
                        }
                    }
                    else if (datatype == "datetime")
                    {
                        if (values != "")
                        {
                            custom_clause = " [" + column_name + "] >= '" + FormatFromDate(values) + "'\n";
                            where = BuildWhere(where, custom_clause, whereConditionalOperator);

                            // reset, and do the to date
                            custom_clause = "";
                            values = request["to__" + column_name];
                            if (values != "")
                            {
                                custom_clause = " [" + column_name + "] <= '" + FormatToDate(values) + "'\n";
                                where = BuildWhere(where, custom_clause, whereConditionalOperator);
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
                            string in_not_in = FormatInNotIn(values);
                            custom_clause = " [" + column_name + "] in " + in_not_in + "\n";
                            where = BuildWhere(where, custom_clause, whereConditionalOperator);
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
                if (extendedSearch.UseFullNames)
                {
                    select += "\n,isnull(rpt.us_lastname + ', ' + rpt.us_firstname,'') [reported by]";
                }
                else
                {
                    select += "\n,isnull(rpt.us_username,'') [reported by]";
                }
                select += "\n,bg_reported_date [reported on]";

                // last updated
                if (extendedSearch.UseFullNames)
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
                    if (extendedSearch.UseFullNames)
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
                    if (showUDF)
                    {
                        string udf_name = Util.get_setting("UserDefinedBugAttributeName", "YOUR ATTRIBUTE");
                        select += ",\nisnull(udf_name,'') [" + udf_name + "]";
                    }
                }

                // let results include custom columns
                string custom_cols_sql = "";
                int user_type_cnt = 1;
                foreach (DataRow drcc in extendedSearch.CustomColumns.Tables[0].Rows)
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
                string project_dropdown_select_cols_server_side = "";

                string alias1 = null;
                string alias2 = null;
                string alias3 = null;

                foreach (ListItem selected_project in extendedSearch.ProjectList)
                {
                    if (selected_project.Value == "0")
                        continue;

                    int pj_id = Convert.ToInt32(selected_project.Value);

                    if (extendedSearch.MapProjects.ContainsKey(pj_id))
                    {

                        BtnetProject btnet_project = extendedSearch.MapProjects[pj_id];

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

                //This variable is used on the ASPX page to help construct the SQL query.
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
                foreach (DataRow drcc in extendedSearch.CustomColumns.Tables[0].Rows)
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

                if (showUDF)
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
            return ds;
            //DataView dv = new DataView(ds.Tables[0]);
            //Session["bugs"] = dv;
            //Session["bugs_unfiltered"] = ds.Tables[0];
        }

        /// <summary>
        /// Used to build the full where clause for a sql statement. 
        /// This is called multiple times to build out the where clause for all possible selections.
        /// </summary>
        /// <param name="where">The current where clause</param>
        /// <param name="clause">What needs to be added to the where clause</param>
        /// <returns>Full where clause for a specific property</returns>
        public string BuildWhere(string where, string clause, string conditionalOperator)
        {
            if (string.IsNullOrEmpty(clause))
            {
                return where;
            }

            string sql = "";

            if (string.IsNullOrEmpty(where))
            {
                sql = string.Format(" where {0}", clause);
            }
            else
            {
                sql = string.Format("{0} {1} {2}", where, conditionalOperator, clause);
            }

            return sql;
        }

        /// <summary>
        /// Takes the list of selected values from a list and builds part of the where clause 
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string BuildWhereFromList(List<ListItem> selectedItems, string columnName)
        {
            string clause = "";
            if (selectedItems.Count > 0)
            {
                clause = string.Format("{0} in ({1}) ", columnName, string.Join(",", selectedItems.Select(x => x.Value).ToArray()));
            }

            return clause;
        }


        /// <summary>
        /// Formats values for the IN or NOT IN sql clause
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string FormatInNotIn(string s)
        {

            string vals = "(";
            string opts = "";

            string[] s2 = Util.split_string_using_commas(s);
            for (int i = 0; i < s2.Length; i++)
            {
                if (opts != "")
                {
                    opts += ",";
                }

                string one_opt = "N'";
                one_opt += s2[i].Replace("'", "''");
                one_opt += "'";

                opts += one_opt;
            }
            vals += opts;
            vals += ")";

            return vals;
        }

        
        /// <summary>
        /// Changes the date from a page element into a database date format
        /// </summary>
        /// <param name="dt">Date that you want to format</param>
        /// <returns>Database formatted date</returns>
        public string FormatFromDate(string date)
        {
            return Util.format_local_date_into_db_format(date).Replace(" 12:00:00", "").Replace(" 00:00:00", "");
        }

        /// <summary>
        /// Changes the date from a page element into a database date format
        /// </summary>
        /// <param name="dt">Date that you want to format</param>
        /// <returns>Database formatted date</returns>
        public string FormatToDate(string date)
        {
            return Util.format_local_date_into_db_format(date).Replace(" 12:00:00", " 23:59:59").Replace(" 00:00:00", " 23:59:59");
        }
        #endregion
    }
}