using btnet;
using BugTracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;


namespace BugTracker.Web.Services
{
    public class SignInService
    {
        private UserService _userService = new UserService();

        public bool ValidateCustomer(string userName, string password)
        {
            //Checks to see if the user exists. If not return false
            DataRow dr = _userService.GetUserByUserName(userName);

            if(dr == null)
            {
                return false;
            }

            //If user exists, try authenticating their password
            return btnet.Authenticate.check_password(userName, password);
        }

        public UserRegistrationResult RegisterUser(RegisterVM model)
        {
            var result = new UserRegistrationResult();

            //Validate unique user
            //Currently if the user has registered, but not confirmed, there is no way to get the confirmation email to resend.
            //They will need to wait the configured number of minutes until the link expires
            if (_userService.GetUserByEmail(model.Email) != null || _userService.GetPendingUserByEmail(model.Email) != null)
            {
                result.AddError("Email is already registered.");
                return result;
            }

            if (_userService.GetUserByUserName(model.UserName) != null || _userService.GetPendingUserByUserName(model.UserName) != null)
            {
                result.AddError("Username is already taken.");
                return result;
            }

            //At this point everything is valid
            string registrationCode = _userService.AddRegisteredUser(model);

            btnet.Email.send_email(
                model.Email,
                Util.get_setting("NotificationEmailFrom", ""),
                "", // cc
                "Please complete registration",

                "Click to <a href='"
                    + Util.get_setting("AbsoluteUrlPrefix", "")
                    + "complete_registration.aspx?id="
                    + registrationCode
                    + "'>complete registration</a>.",

                BtnetMailFormat.Html);

            return result;
        }

        /// <summary>
        /// Checks to see if the registration link is valid
        /// </summary>
        /// <param name="linkId">Id of the registration link</param>
        /// <returns>True if it there is a record and it is not expired, otherwise returns false</returns>
        public bool IsRegistrationLinkValid(string linkId)
        {
            bool isValid = true;

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("declare @expiration datetime");
            sql.AppendLine("set @expiration = dateadd(n,-@minutes,getdate())");
            sql.AppendLine("select *, case when el_date < @expiration then 1 else 0 end[expired]");
            sql.AppendLine("from emailed_links where el_id = @linkId");
            sql.AppendLine("delete from emailed_links where el_date < dateadd(n, -240, getdate())");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();

            cmd.Parameters.AddWithValue("@minutes", int.Parse(Util.get_setting("RegistrationExpiration", "20")));
            cmd.Parameters.AddWithValue("@linkId", linkId);

            DataRow dr = DbUtil.get_datarow(cmd);

            if (dr == null || (int)dr["expired"] == 1)
            {
                isValid = false;
            }

            return isValid;
        }


        public void AddConfirmedUser(string linkId)
        {
            var registeredUser = _userService.GetRegisteredUser(linkId);

            if (registeredUser != null)
            {
                //Copy the user from the registration table to the Users table
                User.copy_user(
                        registeredUser.UserName,
                        registeredUser.Email,
                        registeredUser.FirstName,
                        registeredUser.LastName,
                        "",
                        registeredUser.Salt,
                        registeredUser.Password,
                        Util.get_setting("SelfRegisteredUserTemplate", "[error - missing user template]"),
                        false);

                DeleteRegisteredUser(linkId);
            }
        }

        public void DeleteRegisteredUser(string linkId)
        {
            string sql = @"delete from emailed_links where el_id = @linkId";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@linkId", linkId);
            
            DbUtil.execute_nonquery(cmd);
        }

        public void DeleteLinksOverFourHours()
        {
            string sql = "DELETE FROM emailed_links WHERE el_date < dateadd(n,-240,getdate())";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Removes the session information from the database
        /// </summary>
        /// <param name="sessionId"></param>
        public void RemoveSession(string sessionId)
        {
            string sql = @"delete from sessions	where se_id = @sessionId or datediff(d, se_date, getdate()) > 2";
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@sessionId", sessionId);

            DbUtil.execute_nonquery(cmd);
        }

        public void ResetUsersPassword(string linkId, string password)
        {
            var registeredUser = _userService.GetPasswordResetUser(linkId);

            if (registeredUser != null)
            {
                Util.UpdateUserPassword(registeredUser.UserId.Value, password);

                DeleteRegisteredUser(linkId);
            }
        }

        public PasswordResetResult SendForgotPasswordLink(int userId)
        {
            string guid = Guid.NewGuid().ToString();
            var result = new PasswordResetResult();

            DataRow dr = AddPasswordResetLink(userId, guid);

            string emailResult = btnet.Email.send_email(
                (string)dr["us_email"],
                Util.get_setting("NotificationEmailFrom", ""),
                "", // cc
                "reset password",

                "Click to <a href='"
                    + Util.get_setting("AbsoluteUrlPrefix", "")
                    + "change_password.aspx?id="
                    + guid
                    + "'>reset password</a> for user \""
                    + (string)dr["us_username"]
                    + "\".",

                BtnetMailFormat.Html);

            if (!string.IsNullOrEmpty(emailResult))
            {
                result.AddError(emailResult);
            }
            
            return result;
        }

        private DataRow AddPasswordResetLink(int userId, string guid)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("declare @username nvarchar(255)");
            sql.AppendLine("declare @email nvarchar(255)");
            
            sql.AppendLine("select @username = us_username, @email = us_email from users where us_id = @userId");
            
            sql.AppendLine("insert into emailed_links"); 
            sql.AppendLine("(el_id, el_date, el_email, el_action, el_user_id)"); 
            sql.AppendLine("values (@guid, getdate(), @email, N'forgot', @userId)");
            
            sql.AppendLine("select @username us_username, @email us_email");

            SqlCommand cmd = new SqlCommand(sql.ToString());
            cmd.Parameters.AddWithValue("@guid", guid);
            cmd.Parameters.AddWithValue("@userId", userId);

            return DbUtil.get_datarow(cmd);
        }
    }
}