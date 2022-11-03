using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services
{
    public class SignInService
    {
        public UserService userService = new UserService();

        public bool ValidateCustomer(string userName, string password)
        {
            //Checks to see if the user exists. If not return false
            DataRow dr = userService.FindByUserName(userName);

            if(dr == null)
            {
                return false;
            }

            //If user exists, try authenticating their password
            return btnet.Authenticate.check_password(userName, password);
        }
    }
}