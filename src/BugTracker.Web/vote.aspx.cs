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
    public partial class vote : Page
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
                    // treat it like a delta and update the cached vote count.
                    int vote = Convert.ToInt32(Util.sanitize_integer(Request["vote"]));
                    object obj_vote_count = Application[Convert.ToString(bugid)];
                    int vote_count = 0;

                    if (obj_vote_count != null)
                        vote_count = (int)obj_vote_count;

                    vote_count += vote;

                    Application[Convert.ToString(bugid)] = vote_count;

                    // now treat it more like a boolean
                    if (vote == -1)
                        vote = 0;

                    dv[i]["$VOTE"] = vote;
                    
                    _bugService.UpdateBugVote(bugid, vote, security.user.usid);

                    break;
                }
            }

        }
    }
}
