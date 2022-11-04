using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using btnet;
using BugTracker.Web.Services;

namespace BugTracker.Web
{
    public partial class @default : Page
    {
        string sql;
		UserService userService = new UserService();
		SignInService signInService = new SignInService();

		protected void Page_Load(Object sender, EventArgs e)
		{

			Util.set_context(HttpContext.Current);

			Util.do_not_cache(Response);

			title.InnerText = String.Format("{0} - Log in", Util.get_setting("AppTitle", "BugTracker.NET"));

			msg.InnerText = "";
			
			//Determines if we need to stay here or redirect to Windows Auth login page.
			GetAuthenticationMode();

			if (IsPostBack)
			{
				Login();
			}

		}

		//Logs the user into the system
		private void Login()
		{

			string auth_mode = Util.get_setting("WindowsAuthentication", "0");
			if (auth_mode != "0")
			{
				if (user.Value.Trim() == "")
				{
					btnet.Util.redirect("loginNT.aspx", Request, Response);
				}
			}

			string userName = user.Value;
			string password = pw.Value;

			bool validCustomer = signInService.ValidateCustomer(userName, password);

			if (validCustomer)
			{
				var customerDataRow = userService.GetUserByUserName(userName);
				int userId = (int)customerDataRow["us_id"];
				

				btnet.Security.create_session(
					Request,
					Response,
					userId,
					userName,
					"0");

				btnet.Util.redirect(Request, Response);
				
			}
			else
			{
				//This can also happen if someday the authentication
				//method uses, say LDAP, then check_password could return
				//true, even though there's no user in the database";
				msg.InnerText = "Invalid Log in attempt.";
			}

		}

		//Determines the authetication mode set in the App Settings and redirects if necessary
		private void GetAuthenticationMode()
        {
			// Get authentication mode
			string auth_mode = Util.get_setting("WindowsAuthentication", "0");
			HttpCookie username_cookie = Request.Cookies["user"];
			string previous_auth_mode = "0";
			if (username_cookie != null)
			{
				previous_auth_mode = username_cookie["NTLM"];
			}

			// If an error occured, then force the authentication to manual
			if (Request.QueryString["msg"] == null)
			{
				// If windows authentication only, then redirect
				if (auth_mode == "1")
				{
					btnet.Util.redirect("loginNT.aspx", Request, Response);
				}

				// If previous login was with windows authentication, then try it again
				if (previous_auth_mode == "1" && auth_mode == "2")
				{
					Response.Cookies["user"]["name"] = "";
					Response.Cookies["user"]["NTLM"] = "0";
					btnet.Util.redirect("loginNT.aspx", Request, Response);
				}
			}
			else
			{
				if (Request.QueryString["msg"] != "logged off")
				{
					msg.InnerHtml = "Error during windows authentication:<br>"
						+ HttpUtility.HtmlEncode(Request.QueryString["msg"]);
				}
			}
		}
	}
}
