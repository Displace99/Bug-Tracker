using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

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
        public string BuildWhereFromList(List<string> selectedItems, string columnName)
        {
            string clause = "";
            if (selectedItems.Count > 0)
            {
                clause = string.Format("{0} in ({1}) ", columnName, string.Join(",", selectedItems));
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