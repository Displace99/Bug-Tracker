using btnet;
using BugTracker.Web.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class register : Page
    {
		void Page_Load(Object sender, EventArgs e)
		{

			Util.set_context(HttpContext.Current);
			Util.do_not_cache(Response);

			if (Util.get_setting("AllowSelfRegistration", "0") == "0")
			{
				Response.Redirect("Default.aspx");
			}

			if (!IsPostBack)
			{
				title.InnerText = String.Format("{0} - Register", Util.get_setting("AppTitle", "BugTracker.NET"));
			}
			else
			{
				//Resets all error message fields
				msg.InnerHtml = "&nbsp;";
				username_err.InnerHtml = "&nbsp;";
				email_err.InnerHtml = "&nbsp;";
				password_err.InnerHtml = "&nbsp;";
				confirm_err.InnerHtml = "&nbsp;";
				firstname_err.InnerHtml = "&nbsp;";
				lastname_err.InnerHtml = "&nbsp;";

				bool isValid = validate();

				if (!isValid)
				{
					msg.InnerHtml = "Registration was not submitted.";
				}
				else
				{
					string guid = Guid.NewGuid().ToString();

					string salt = Util.GenerateRandomString();
					string hashedPassword = EncryptionService.HashString(password.Value, Convert.ToString(salt));

					StringBuilder sql = new StringBuilder();
					sql.Append("insert into emailed_links ");
					sql.Append("(el_id, el_date, el_email, el_action, el_username, el_salt, el_password, el_firstname, el_lastname) ");
					sql.Append("values (@Id, getdate(), @email, @register, @username, @salt, @password, @firstname, @lastname)");

					SqlCommand cmd = new SqlCommand();
					cmd.CommandText = sql.ToString();
					cmd.Parameters.AddWithValue("@Id", guid);
					cmd.Parameters.AddWithValue("@email", email.Value);
					cmd.Parameters.AddWithValue("@register", "register");
					cmd.Parameters.AddWithValue("@username", username.Value);
					cmd.Parameters.AddWithValue("@salt", salt);
					cmd.Parameters.AddWithValue("@password", hashedPassword);
					cmd.Parameters.AddWithValue("@firstname", firstname.Value);
					cmd.Parameters.AddWithValue("@lastname", lastname.Value);

					btnet.DbUtil.execute_nonquery(cmd);

					string result = btnet.Email.send_email(
						email.Value,
						Util.get_setting("NotificationEmailFrom", ""),
						"", // cc
						"Please complete registration",

						"Click to <a href='"
							+ Util.get_setting("AbsoluteUrlPrefix", "")
							+ "complete_registration.aspx?id="
							+ guid
							+ "'>complete registration</a>.",

						BtnetMailFormat.Html);

					msg.InnerHtml = "An email has been sent to " + email.Value;
					msg.InnerHtml += "<br>Please click on the link in the email message to complete registration.";

				}
			}
		}

		///////////////////////////////////////////////////////////////////////
		bool validate()
		{

			bool valid = true;

			if (username.Value == "")
			{
				username_err.InnerText = "Username is required.";
				valid = false;
			}

			if (email.Value == "")
			{
				email_err.InnerText = "Email is required.";
				valid = false;
			}
			else
			{
				if (!Util.validate_email(email.Value))
				{
					email_err.InnerHtml = "Format of email address is invalid.";
					valid = false;
				}
			}

			if (password.Value == "")
			{
				password_err.InnerText = "Password is required.";
				valid = false;
			}
			if (confirm.Value == "")
			{
				confirm_err.InnerText = "Confirm password is required.";
				valid = false;
			}

			if (password.Value != "" && confirm.Value != "")
			{
				if (password.Value != confirm.Value)
				{
					confirm_err.InnerText = "Confirm doesn't match password.";
					valid = false;
				}
				else if (!Util.check_password_strength(password.Value))
				{
					password_err.InnerHtml = "Password is not difficult enough to guess.";
					password_err.InnerHtml += "<br>Avoid common words.";
					password_err.InnerHtml += "<br>Try using a mixture of lowercase, uppercase, digits, and special characters.";
					valid = false;
				}
			}

			if (firstname.Value == "")
			{
				firstname_err.InnerText = "Firstname is required.";
				valid = false;
			}
			if (lastname.Value == "")
			{
				lastname_err.InnerText = "Lastname is required.";
				valid = false;
			}

			// check for dupes



			string sql = @"
				declare @user_cnt int
				declare @email_cnt int
				declare @pending_user_cnt int
				declare @pending_email_cnt int
				select @user_cnt = count(1) from users where us_username = N'$us'
				select @email_cnt = count(1) from users where us_email = N'$em'
				select @pending_user_cnt = count(1) from emailed_links where el_username = N'$us'
				select @pending_email_cnt = count(1) from emailed_links where el_email = N'$em'
				select @user_cnt, @email_cnt, @pending_user_cnt, @pending_email_cnt";
			sql = sql.Replace("$us", username.Value.Replace("'", "''"));
			sql = sql.Replace("$em", email.Value.Replace("'", "''"));

			DataRow dr = btnet.DbUtil.get_datarow(sql);

			if ((int)dr[0] > 0)
			{
				username_err.InnerText = "Username already being used. Choose another.";
				valid = false;
			}

			if ((int)dr[1] > 0)
			{
				email_err.InnerText = "Email already being used. Choose another.";
				valid = false;
			}

			if ((int)dr[2] > 0)
			{
				username_err.InnerText = "Registration pending for this username. Choose another.";
				valid = false;
			}

			if ((int)dr[3] > 0)
			{
				email_err.InnerText = "Registration pending for this email. Choose another.";
				valid = false;
			}


			return valid;
		}
	}
}
