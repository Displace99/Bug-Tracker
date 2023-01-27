using btnet;
using BugTracker.Web.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class users : Page
    {
        protected DataSet ds;

        protected Security security;

        private UserService _userService = new UserService();

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN_OR_PROJECT_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "users";

            if (!IsPostBack)
            {
                HttpCookie cookie = Request.Cookies["hide_inactive_users"];
                if (cookie != null)
                {
                    if (cookie.Value == "1")
                    {
                        hide_inactive_users.Checked = true;
                    }
                }

                HttpCookie cookie2 = Request.Cookies["filter_users"];
                if (cookie2 != null)
                {
                    filter_users.Value = (string)cookie2.Value;
                }
                else
                {
                    filter_users.Value = "";
                }
            }

            if (security.user.is_admin)
            {
                ds = _userService.GetUserListForAdmins(filter_users.Value, !hide_inactive_users.Checked);
            }
            else
            {
                ds = _userService.GetUserListForNonAdmins(filter_users.Value, !hide_inactive_users.Checked, security.user.usid);
            }

            // cookies
            if (hide_inactive_users.Checked)
            {
                Response.Cookies["hide_inactive_users"].Value = "1";
            }
            else
            {
                Response.Cookies["hide_inactive_users"].Value = "0";
            }


            Response.Cookies["filter_users"].Value = filter_users.Value;

            DateTime dt = DateTime.Now;
            TimeSpan ts = new TimeSpan(365, 0, 0, 0);
            Response.Cookies["hide_inactive_users"].Expires = dt.Add(ts);
            Response.Cookies["filter_users"].Expires = dt.Add(ts);
        }
    }
}
