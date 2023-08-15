using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class write_posts : Page
    {
        Security security;

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            int bugid = Convert.ToInt32(Request["id"]);
            bool images_inline = (Request["images_inline"] == "1");
            bool history_inline = (Request["history_inline"] == "1");

            int permission_level = Bug.get_bug_permission_level(bugid, security);
            if (permission_level == Security.PERMISSION_NONE)
            {
                Response.Write("You are not allowed to view this item");
                Response.End();
            }

            DataSet ds_posts = PrintBug.get_bug_posts(bugid, security.user.external_user, history_inline);

            PrintBug.write_posts(
                ds_posts,
                Response,
                bugid,
                permission_level,
                true, // write links
                images_inline,
                history_inline,
                true, // internal_posts
                security.user);

        }
    }
}
