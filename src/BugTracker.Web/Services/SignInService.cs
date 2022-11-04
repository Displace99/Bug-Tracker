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
        public UserService userService = new UserService();

        public bool ValidateCustomer(string userName, string password)
        {
            //Checks to see if the user exists. If not return false
            DataRow dr = userService.GetUserByUserName(userName);

            if(dr == null)
            {
                return false;
            }

            //If user exists, try authenticating their password
            return btnet.Authenticate.check_password(userName, password);
        }

        public void RegisterUser(RegisterVM model)
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

			Email.send_email(
				model.Email,
				Util.get_setting("NotificationEmailFrom", ""),
				"", // cc
				"Please complete registration",

				"Click to <a href='"
					+ Util.get_setting("AbsoluteUrlPrefix", "")
					+ "complete_registration.aspx?id="
					+ guid
					+ "'>complete registration</a>.",

				BtnetMailFormat.Html);
		}
    }
}