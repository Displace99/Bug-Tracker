using btnet;
using BugTracker.Web.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class forgot : Page
    {
        private UserService _userService = new UserService();
        private SignInService _signInService = new SignInService();

        void Page_Load(Object sender, EventArgs e)
        {

            Util.set_context(HttpContext.Current);
            Util.do_not_cache(Response);

            if (Util.get_setting("ShowForgotPasswordLink", "0") == "0")
            {
                Response.Write("Sorry, Web.config ShowForgotPasswordLink is set to 0");
                Response.End();
            }

            DataRow user = null;

            if (!IsPostBack)
            {
                titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "forgot password";
            }
            else
            {
                msg.InnerHtml = "";

                if (email.Value == "" && username.Value == "")
                {
                    msg.InnerHtml = "Enter either your Username or your Email address.";
                }
                else if (email.Value != "" && !Util.validate_email(email.Value))
                {
                    msg.InnerHtml = "Format of email address is invalid.";
                }
                else
                {
                    //Check for email
                    if (email.Value != "" && username.Value == "")
                    {
                        user = _userService.GetUserByEmail(email.Value);
                    }
                    //Check for Username
                    else if (email.Value == "" && username.Value != "")
                    {
                        user = _userService.GetUserByUserName(username.Value);
                    }
                    else if (email.Value != "" && username.Value != "")
                    {
                        user = _userService.GetUserByUserNameAndEmail(username.Value, email.Value);
                    }

                    if(user != null)
                    {
                        int userId = (int)user["us_id"];

                        var result = _signInService.SendForgotPasswordLink(userId);

                        if (result.Success)
                        {
                            msg.InnerHtml = "An email with password info has been sent to you.";
                        }
                        else
                        {
                            msg.InnerHtml = "There was a problem sending the email.";
                        }
                    }
                    else
                    {
                        msg.InnerHtml = "Unknown username or email address.<br>Are you sure you spelled everything correctly?<br>Try just username, just email, or both.";
                    }
                }
            }
        }
    }
}
