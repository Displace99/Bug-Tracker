using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
            string sql = "SELECT qu_desc, qu_sql, isnull(qu_user,0) [qu_user], isnull(qu_org,0) [qu_org] FROM queries WHERE qu_id = @Id";

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
            string sql = "DELETE queries WHERE qu_id = @queryId";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@queryId", queryId);

            DbUtil.execute_nonquery(cmd);
        }

        public bool IsQueryUnique(string queryName, int? queryId = null)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT COUNT(1) FROM queries WHERE qu_desc = @queryName");

            if(queryId.HasValue)
            {
                sql.Append(" AND qu_id <> @Id");
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("@queryName", queryName);

            if (queryId.HasValue)
            {
                cmd.Parameters.AddWithValue("@Id", queryId);
            }

            var queryCount = (int)DbUtil.execute_scalar(cmd);

            return queryCount == 0;
        }
    }
}