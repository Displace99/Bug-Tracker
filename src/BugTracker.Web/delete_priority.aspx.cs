using btnet;
using BugTracker.Web.Models.Category;
using BugTracker.Web.Services.Category;
using BugTracker.Web.Services.Priority;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_priority : Page
    {
        protected string sql;
        protected Security security;

        private PriorityService _priorityService = new PriorityService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            int priorityId = 0;
            int.TryParse(Request["id"], out priorityId);

            if (IsPostBack)
            {
                _priorityService.DeletePriority(priorityId);
                
                Server.Transfer("priorities.aspx");
            }
            else
            {

                titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete priority";

                int bugCount = _priorityService.GetBugCountByPriority(priorityId);
                var priorityDr = _priorityService.GetPriorityById(priorityId);
                string priorityName = (string)priorityDr[1];

                if (bugCount > 0)
                {
                    Response.Write(String.Format("You can't delete priority {0} because some bugs still reference it.", priorityName));
                    Response.End();
                }
                else
                {
                    confirm_href.InnerText = String.Format("Confirm delete of {0}", priorityName);

                    row_id.Value = priorityId.ToString();
                }

            }

        }
    }
}
