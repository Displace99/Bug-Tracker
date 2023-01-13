using btnet;
using BugTracker.Web.Models.Category;
using BugTracker.Web.Services;
using BugTracker.Web.Services.Bug;
using BugTracker.Web.Services.Category;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_user : Page
    {
        string sql;

        protected Security security;
        private UserService _userService = new UserService();
        private BugService _bugService = new BugService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN_OR_PROJECT_ADMIN);

            int userId = 0;
            int.TryParse(Request["id"], out userId);

            //Checks to see if current user is able to delete selected user
            if (!security.user.is_admin)
            {
                DataRow dr = _userService.GetUserById(userId);

                if (security.user.usid != (int)dr["us_created_user"])
                {
                    Response.Write("You not allowed to delete this user, because you didn't create it.");
                    Response.End();
                }
                else if ((int)dr["us_admin"] == 1)
                {
                    Response.Write("You not allowed to delete this user, because it is an admin.");
                    Response.End();
                }
            }

            if (IsPostBack)
            {
                // do delete here
                _userService.DeleteUser(userId);
                Server.Transfer("users.aspx");
            }
            else
            {
                title.InnerText = string.Format("{0} - Delete User", Util.get_setting("AppTitle", "BugTracker.NET"));

                int bugCount = _bugService.GetBugCountByUserId(userId);
                var userDr = _userService.GetUserById(userId);
                string userName = (string)userDr["us_username"];

                if (bugCount > 0)
                {
                    Response.Write(string.Format("You can't delete user {0} because some bugs or bug posts still reference it.", userName));
                    Response.End();
                }
                else
                {
                    confirm_href.InnerText = string.Format("Confirm delete of {0}", userName);

                    row_id.Value = userId.ToString();
                }
            }

        }
    }
}
