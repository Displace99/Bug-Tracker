using btnet;
using BugTracker.Web.Models;
using BugTracker.Web.Models.Registration;
using BugTracker.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
            string sql = "select el_id from emailed_links where el_email = @email";
            
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@email", email);

            return btnet.DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Adds a user to the Registration Table
        /// </summary>
        /// <param name="model">Registered User</param>
        /// <returns>Id from the registration table</returns>
        public string AddRegisteredUser(RegisterVM model)
        {
            string guid = Guid.NewGuid().ToString();

            string salt = Util.GenerateRandomString();
            string hashedPassword = EncryptionService.HashString(model.Password, Convert.ToString(salt));

            StringBuilder sql = new StringBuilder();
            sql.Append("insert into emailed_links ");
            sql.Append("(el_id, el_date, el_email, el_action, el_username, el_salt, el_password, el_firstname, el_lastname) ");
            sql.Append("values (@Id, getdate(), @email, @register, @username, @salt, @password, @firstname, @lastname)");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("@Id", guid);
            cmd.Parameters.AddWithValue("@email", model.Email);
            cmd.Parameters.AddWithValue("@register", "register");
            cmd.Parameters.AddWithValue("@username", model.UserName);
            cmd.Parameters.AddWithValue("@salt", salt);
            cmd.Parameters.AddWithValue("@password", hashedPassword);
            cmd.Parameters.AddWithValue("@firstname", model.FirstName);
            cmd.Parameters.AddWithValue("@lastname", model.LastName);

            DbUtil.execute_nonquery(cmd);

            return guid;
        }

        public RegisteredUser GetRegisteredUser(string linkId)
        {
            var dr = GetEmailedLink(linkId);

            RegisteredUser user = null;

            if(dr != null)
            {
                user = new RegisteredUser();

                user.Id = linkId;
                user.UserName = (string)dr["el_username"];
                user.Email = (string)dr["el_email"];
                user.FirstName = (string)dr["el_firstname"];
                user.LastName = (string)dr["el_lastname"];
                user.Salt = (string)dr["el_salt"];
                user.Password = (string)dr["el_password"];

            }

            return user;
        }

        public RegisteredUser GetPasswordResetUser(string linkId)
        {
            var dr = GetEmailedLink(linkId);

            RegisteredUser user = null;

            if (dr != null)
            {
                user = new RegisteredUser();

                user.Id = linkId;
                user.Email = (string)dr["el_email"];
                user.UserId = (int)dr["el_user_id"];

            }

            return user;
        }

        private DataRow GetEmailedLink(string linkId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("declare @expiration datetime");
            sql.AppendLine("set @expiration = dateadd(n,-@minutes,getdate())");
            sql.AppendLine("select * from emailed_links where @expiration < el_date AND el_id = @linkId");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();

            cmd.Parameters.AddWithValue("@minutes", int.Parse(Util.get_setting("RegistrationExpiration", "20")));
            cmd.Parameters.AddWithValue("@linkId", linkId);

            return DbUtil.get_datarow(cmd);
        }

    }
}