using btnet;
using BugTracker.Web.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class change_password : Page
    {
		private SignInService _signInService = new SignInService();

		void Page_Load(Object sender, EventArgs e)
		{

			Util.set_context(HttpContext.Current);
			Util.do_not_cache(Response);

			if (!IsPostBack)
			{
				title.InnerText = String.Format("{0} - Change Password", Util.get_setting("AppTitle", "BugTracker.NET"));
			}
			else
			{
				msg.InnerHtml = "";

				bool isValid = ValidateForm();

				if (isValid)
				{
					string linkId = Request["id"];

					if (string.IsNullOrEmpty(linkId))
					{
						msg.InnerHtml = "The change password link is invalid. Please request a new Password Reset.";
						return;
					}

                    if (_signInService.IsRegistrationLinkValid(linkId))
                    {
						_signInService.ResetUsersPassword(linkId, password.Value);
						
						msg.InnerHtml = "Your password has been changed.";
					}
					else
					{
						msg.InnerHtml = "The change password link is invalid. Please request a new Password Reset.";
					}
				}
			}
		}

		public bool ValidateForm()
        {
			bool valid = true;

			if (string.IsNullOrEmpty(password.Value))
			{
				msg.InnerHtml = "Enter your password twice.";
				valid = false;
			}
			else if (password.Value != confirm.Value)
			{
				msg.InnerHtml = "Re-entered password doesn't match password.";
				valid = false;
			}
			else if (!Util.check_password_strength(password.Value))
			{
				msg.InnerHtml = "Password is not difficult enough to guess.";
				msg.InnerHtml += "<br>Avoid common words.";
				msg.InnerHtml += "<br>Try using a mixture of lowercase, uppercase, digits, and special characters.";
				valid = false;
			}

			return valid;
		}
	}
}
