using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace BugTracker.Web.Services.Priority
{
    public class PriorityService
    {
        /// <summary>
        /// Returns a list of all Priorities in the system
        /// </summary>
        /// <returns></returns>
        public DataSet GetPriorityList()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select pr_id [id], pr_name [description], pr_sort_seq [sort seq], ");
            sql.AppendLine("'<div style=''background:' + pr_background_color + ';''>' + pr_background_color + '</div>' [background<br>color], ");
            sql.AppendLine("pr_style [css<br>class], case when pr_default = 1 then 'Y' else 'N' end [default], pr_id [hidden]");
            sql.AppendLine("from priorities");

            DataSet priorityDS = DbUtil.get_dataset(sql.ToString());

            return priorityDS;
        }

        /// <summary>
        /// Deletes a priority from the database
        /// </summary>
        /// <param name="priorityId">Id of the priority you want deleted</param>
        public void DeletePriority(int priorityId)
        {
            string sql = "delete priorities where pr_id = @priorityId";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@priorityId", priorityId);
            
            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Returns details of a specific priority
        /// </summary>
        /// <param name="priorityId">Id of the priority</param>
        /// <returns></returns>
        public DataRow GetPriorityById(int priorityId)
        {
            string sql = "SELECT pr_id, pr_name, pr_sort_seq, pr_background_color, pr_style, pr_default FROM priorities WHERE pr_id = @Id";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Id", priorityId);

            return DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Returns a count of bugs that are tied to a specific priority
        /// </summary>
        /// <param name="priorityId">The Id of the priority</param>
        /// <returns></returns>
        public int GetBugCountByPriority(int priorityId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select count(1) [cnt] from bugs where bg_priority = @priorityId");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("@priorityId", priorityId);

            DataRow dr = DbUtil.get_datarow(cmd);

            return (int)dr["cnt"];
        }
    }
}