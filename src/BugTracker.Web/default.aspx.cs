using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using btnet;

namespace BugTracker.Web
{
    public partial class @default : Page
    {
        string sql;

		protected void Page_Load(Object sender, EventArgs e)
		{

			Util.set_context(HttpContext.Current);

			Util.do_not_cache(Response);

			title.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
				+ "Log in";

			msg.InnerText = "";

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


			// fill in the username first time in
			if (!IsPostBack)
			{
				if (previous_auth_mode == "0")
				{
					if ((Request.QueryString["user"] == null) || (Request.QueryString["password"] == null))
					{
						//	User name and password are not on the querystring.

						if (username_cookie != null)
						{
							//	Set the user name from the last logon.

							user.Value = username_cookie["name"];
						}
					}
					else
					{
						//	User name and password have been passed on the querystring.

						user.Value = Request.QueryString["user"];
						pw.Value = Request.QueryString["password"];

						on_logon();
					}
				}
			}
			else
			{
				on_logon();
			}

		}

		private void on_logon()
		{

			string auth_mode = Util.get_setting("WindowsAuthentication", "0");
			if (auth_mode != "0")
			{
				if (user.Value.Trim() == "")
				{
					btnet.Util.redirect("loginNT.aspx", Request, Response);
				}
			}

			bool authenticated = btnet.Authenticate.check_password(user.Value, pw.Value);

			if (authenticated)
			{
				sql = "select us_id from users where us_username = N'$us'";
				sql = sql.Replace("$us", user.Value.Replace("'", "''"));
                DataRow dr = btnet.DbUtil.get_datarow(sql);
				if (dr != null)
				{
					int us_id = (int)dr["us_id"];

					btnet.Security.create_session(
						Request,
						Response,
						us_id,
						user.Value,
						"0");

					btnet.Util.redirect(Request, Response);
				}
				else
				{
					// How could this happen?  If someday the authentication
					// method uses, say LDAP, then check_password could return
					// true, even though there's no user in the database";
					msg.InnerText = "User not found in database";
				}
			}
			else
			{
				msg.InnerText = "Invalid User or Password.";
			}

		}
	}
}
