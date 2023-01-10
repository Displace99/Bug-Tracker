using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services.Query
{
    public class QueryService
    {
        /// <summary>
        /// Returns a Query for a specific Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>DataRow of Query</returns>
        public DataRow GetQueryById(int Id)
        {
            string sql = @"select qu_desc, isnull(qu_user,0) qu_user from queries where qu_id = @Id";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Id", Id);

            return DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Deletes a query from the database
        /// </summary>
        /// <param name="queryId">Id of the query you want deleted</param>
        public void DeleteQuery(int queryId)
        {
            string sql = "delete queries where qu_id = @queryId";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@queryId", queryId);

            DbUtil.execute_nonquery(cmd);
        }
    }
}