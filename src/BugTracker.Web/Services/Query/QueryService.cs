using btnet;
using BugTracker.Web.Models.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace BugTracker.Web.Services.Query
{
    public class QueryService
    {
        /// <summary>
        /// Returns a DataSet of all Queries in the system
        /// </summary>
        /// <param name="showAll">Show all queries or only global</param>
        /// <param name="userId">User Id of user to show queries</param>
        /// <returns>DataSet</returns>
        public DataSet GetAllUsersQueries(bool showAll, int userId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT qu_desc [query],");
            sql.AppendLine("case");
            sql.AppendLine("when isnull(qu_user,0) = 0 and isnull(qu_org,0) is null then 'everybody'");
            sql.AppendLine("when isnull(qu_user,0) <> 0 then 'user:' + us_username");
            sql.AppendLine("when isnull(qu_org,0) <> 0 then 'org:' + og_name");
            sql.AppendLine("else ' '");
            sql.AppendLine("end [visibility],");
            sql.AppendLine("'<a href=bugs.aspx?qu_id=' + convert(varchar,qu_id) + '>view list</a>' [view list],");
            sql.AppendLine("'<a target=_blank href=print_bugs.aspx?qu_id=' + convert(varchar,qu_id) + '>print list</a>' [print list],");
            sql.AppendLine("'<a target=_blank href=print_bugs.aspx?format=excel&qu_id=' + convert(varchar,qu_id) + '>export as excel</a>' [export as excel],");
            sql.AppendLine("'<a target=_blank href=print_bugs2.aspx?qu_id=' + convert(varchar,qu_id) + '>print detail</a>' [print list<br>with detail],");
            sql.AppendLine("'<a href=edit_query.aspx?id=' + convert(varchar,qu_id) + '>edit</a>' [edit],");
            sql.AppendLine("'<a href=delete_query.aspx?id=' + convert(varchar,qu_id) + '>delete</a>' [delete]");
            sql.AppendLine("FROM queries");
            sql.AppendLine("LEFT OUTER JOIN users ON qu_user = us_id");
            sql.AppendLine("LEFT OUTER JOIN orgs ON qu_org = og_id");
            sql.AppendLine("WHERE 1 = @showAll /* all */");
            sql.AppendLine("OR isnull(qu_user,0) = @userId");
            sql.AppendLine("OR isnull(qu_user,0) = 0");
            sql.AppendLine("ORDER BY qu_desc");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();

            cmd.Parameters.AddWithValue("@showAll", showAll);
            cmd.Parameters.AddWithValue("@userId", userId);

            return DbUtil.get_dataset(cmd);
        }

        /// <summary>
        /// Returns a DataSet of all queries for specific user
        /// </summary>
        /// <param name="userId">UserId to filter on</param>
        /// <returns>DataSet</returns>
        public DataSet GetQueriesByUserId(int userId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT qu_desc [query],");
            sql.AppendLine("'<a href=bugs.aspx?qu_id=' + convert(varchar,qu_id) + '>view list</a>' [view list],");
            sql.AppendLine("'<a target=_blank href=print_bugs.aspx?qu_id=' + convert(varchar,qu_id) + '>print list</a>' [print list],");
            sql.AppendLine("'<a target=_blank href=print_bugs.aspx?format=excel&qu_id=' + convert(varchar,qu_id) + '>export as excel</a>' [export as excel],");
            sql.AppendLine("'<a target=_blank href=print_bugs2.aspx?qu_id=' + convert(varchar,qu_id) + '>print detail</a>' [print list<br>with detail],");
            sql.AppendLine("'<a href=edit_query.aspx?id=' + convert(varchar,qu_id) + '>rename</a>' [rename],");
            sql.AppendLine("'<a href=delete_query.aspx?id=' + convert(varchar,qu_id) + '>delete</a>' [delete]");
            sql.AppendLine("FROM queries");
            sql.AppendLine("INNER JOIN users ON qu_user = us_id");
            sql.AppendLine("WHERE isnull(qu_user,0) = @userId");
            sql.AppendLine("ORDER BY qu_desc");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();

            cmd.Parameters.AddWithValue("@userId", userId);

            return DbUtil.get_dataset(cmd);
        }

        /// <summary>
        /// Gets a list of queries tied to a specific users organization
        /// </summary>
        /// <param name="userId">User ID to search</param>
        /// <returns>DataSet of Queries</returns>
        public DataSet GetQueriesByUsersOrg(int userId)
        {
            StringBuilder sql = new StringBuilder();

		    sql.AppendLine("DECLARE @org int");
		    sql.AppendLine("SET @org = null");
		    sql.AppendLine("SELECT @org = us_org FROM users WHERE us_id = @userId");
			
            sql.AppendLine("SELECT qu_id, qu_desc");
			sql.AppendLine("FROM queries");
			sql.AppendLine("WHERE (isnull(qu_user,0) = 0 AND isnull(qu_org,0) = 0)");
			sql.AppendLine("OR isnull(qu_user,0) = @userId");
			sql.AppendLine("OR isnull(qu_org,0) = isnull(@org,-1)");
            sql.AppendLine("ORDER BY qu_desc;");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();

            cmd.Parameters.AddWithValue("@userId", userId);

            return DbUtil.get_dataset(cmd);
        }

        /// <summary>
        /// Gets a list of queries tied to a specific users organization
        /// </summary>
        /// <param name="userId">User ID to search</param>
        /// <returns>DataView</returns>
        public DataView GetQueriesByUserForSelf(int userId)
        {
            //This is very similar to the GetQeriesByUsersOrg (above). 
            //See if we can combine these somehow.
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("DECLARE @org INT");
			sql.AppendLine("SELECT @org = us_org FROM users WHERE us_id = @userId");
			
            sql.AppendLine("SELECT qu_id, qu_desc");
			sql.AppendLine("FROM queries");
			sql.AppendLine("WHERE (isnull(qu_user,0) = 0 AND isnull(qu_org,0) = 0)");
			sql.AppendLine("OR isnull(qu_user,0) = @userId");
			sql.AppendLine("OR isnull(qu_org,0) = @org");
            sql.AppendLine("ORDER BY qu_desc");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();

            cmd.Parameters.AddWithValue("@userId", userId);

            return DbUtil.get_dataview(cmd);
        }

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
        /// Creates new Query
        /// </summary>
        /// <param name="query">Update Query Model</param>
        public void CreateQuery(QueryUpdate query)
        {
            string sql = "INSERT INTO queries (qu_desc, qu_sql, qu_default, qu_user, qu_org) VALUES (@description, @sqlText, 0, @selectedUser, @selectedOrg)";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@description", query.Title);
            cmd.Parameters.AddWithValue("@sqlText", query.SqlText);
            cmd.Parameters.AddWithValue("@selectedUser", query.SelectedUserId);
            cmd.Parameters.AddWithValue("@selectedOrg", query.SelectedOrgId);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Updates an existing query
        /// </summary>
        /// <param name="query">Update Query Model</param>
        public void UpdateQuery(QueryUpdate query)
        {
            string sql = "UPDATE queries SET qu_desc = @description, qu_sql = @sqlText,	qu_user = @selectedUser, qu_org = @selectedOrg WHERE qu_id = @Id";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@description", query.Title);
            cmd.Parameters.AddWithValue("@sqlText", query.SqlText);
            cmd.Parameters.AddWithValue("@selectedUser", query.SelectedUserId);
            cmd.Parameters.AddWithValue("@selectedOrg", query.SelectedOrgId);
            cmd.Parameters.AddWithValue("@Id", query.Id);

            DbUtil.execute_nonquery(cmd);
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