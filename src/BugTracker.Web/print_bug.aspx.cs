using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class print_bug : Page
    {
        protected Security security;
        protected DataRow dr;
        protected bool images_inline;
        protected bool history_inline;

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            string string_bugid = Request.QueryString["id"];

            int bugid = Convert.ToInt32(string_bugid);

            dr = btnet.Bug.get_bug_datarow(bugid, security);

            if (dr == null)
            {
                btnet.Util.display_bug_not_found(Response, security, bugid);
                return;
            }

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug"))
                + " ID" + string_bugid + " " + (string)dr["short_desc"];


            // don't allow user to view a bug he is not allowed to view
            if ((int)dr["pu_permission_level"] == 0)
            {
                btnet.Util.display_you_dont_have_permission(Response, security);
                return;
            }

            HttpCookie cookie = Request.Cookies["images_inline"];
            if (cookie == null || cookie.Value == "0")
            {
                images_inline = false;
            }
            else
            {
                images_inline = true;
            }

            cookie = Request.Cookies["history_inline"];
            if (cookie == null || cookie.Value == "0")
            {
                history_inline = false;
            }
            else
            {
                history_inline = true;
            }
        }
    }
}
