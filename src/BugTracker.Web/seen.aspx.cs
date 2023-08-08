using btnet;
using BugTracker.Web.Services.Bug;
using System;
using System.Data;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class seen : Page
    {

        Security security;
        BugService _bugService = new BugService();

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            if (!security.user.is_guest)
            {
                if (Request.QueryString["ses"] != (string)Session["session_cookie"])
                {
                    Response.Redirect("bugs.aspx");
                }
            }

            DataView dv = (DataView)Session["bugs"];
            if (dv == null)
            {
                Response.End();
            }

            int bugid = 0;
            int.TryParse(Request["bugid"], out bugid);

            int permission_level = Bug.get_bug_permission_level(bugid, security);
            if (permission_level == Security.PERMISSION_NONE)
            {
                Response.End();
            }
            
            int seen = Convert.ToInt32(Util.sanitize_integer(Request["seen"]));

            _bugService.MarkBugSeen(dv, bugid, seen, security.user.usid);
        }
    }
}
