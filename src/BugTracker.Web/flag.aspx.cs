using btnet;
using BugTracker.Web.Services.Bug;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class flag : Page
    {
        Security security;
        private BugService _bugService = new BugService();

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            if (!security.user.is_guest)
            {
                if (Request.QueryString["ses"] != (string)Session["session_cookie"])
                {
                    Response.Write("session in URL doesn't match session cookie");
                    Response.End();
                }
            }

            DataView dv = (DataView)Session["bugs"];
            if (dv == null)
            {
                Response.End();
            }

            int bugid = Convert.ToInt32(Util.sanitize_integer(Request["bugid"]));

            int permission_level = Bug.get_bug_permission_level(bugid, security);
            if (permission_level == Security.PERMISSION_NONE)
            {
                Response.End();
            }

            for (int i = 0; i < dv.Count; i++)
            {
                if ((int)dv[i][1] == bugid)
                {
                    int flag = Convert.ToInt32(Util.sanitize_integer(Request["flag"]));
                    dv[i]["$FLAG"] = flag;

                    _bugService.FlagBug(bugid, security.user.usid, flag);

                    break;
                }
            }

        }
    }
}
