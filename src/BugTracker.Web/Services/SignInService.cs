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

        public UserRegistrationResult RegisterUser(RegisterVM model)
        {
            var result = new UserRegistrationResult();

            //Validate unique user
            //TODO: Currently if the user has registered, but not confirmed, there is no way to get the confirmation email to resend.
            if (userService.GetUserByEmail(model.Email) != null || userService.GetPendingUserByEmail(model.Email) != null)
            {
                result.AddError("Email is already registered.");
                return result;
            }

            if (userService.GetUserByUserName(model.UserName) != null || userService.GetPendingUserByUserName(model.UserName) != null)
            {
                result.AddError("Username is already taken.");
                return result;
            }

            //At this point everything is valid
            string registrationCode = userService.AddRegisteredUser(model);

            Email.send_email(
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
    }
}