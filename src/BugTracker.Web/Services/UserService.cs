using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services
{
    public class UserService
    {
        /// <summary>
        /// Gets user by username
        /// </summary>
        /// <param name="userName">Username of person you are search for</param>
        /// <returns></returns>
        public DataRow GetUserByUserName(string userName)
        {
            string sql = "select us_id from users where us_username = @userName";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@userName", userName);

            return btnet.DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email of person you are searching for</param>
        /// <returns></returns>
        public DataRow GetUserByEmail(string email)
        {
            string sql = "select us_id from users where us_email = @email";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@email", email);

            return btnet.DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Get pending user by username
        /// </summary>
        /// <param name="userName">Username of person you are searching for</param>
        /// <returns></returns>
        public DataRow GetPendingUserByUserName(string userName)
        {
            string sql = "select el_id from emailed_links where el_username = @username";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@username", userName);

            return btnet.DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Get pending user by email
        /// </summary>
        /// <param name="email">Email of person youare searching for</param>
        /// <returns></returns>
        public DataRow GetPendingUserByEmail(string email)
        {
            string sql = "select @pending_email_cnt = count(1) from emailed_links where el_email = @email";
            
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@email", email);

            return btnet.DbUtil.get_datarow(cmd);
        }
    }
}