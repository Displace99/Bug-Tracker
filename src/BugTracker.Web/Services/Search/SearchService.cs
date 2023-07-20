using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

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
    }
}