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
        public DataRow FindByUserName(string userName)
        {
            string sql = "select us_id from users where us_username = @userName";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@userName", userName);

            return btnet.DbUtil.get_datarow(cmd);
        }
    }
}