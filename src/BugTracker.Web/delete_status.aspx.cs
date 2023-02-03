using btnet;
using BugTracker.Web.Models.Category;
using BugTracker.Web.Services.Category;
using BugTracker.Web.Services.Status;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_status : Page
    {
        String sql;

        protected Security security;

        private StatusService _statusService = new StatusService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            int id = 0;
            Int32.TryParse(Request["id"], out id);

            if (IsPostBack)
            {
                // do delete here
                _statusService.DeleteStatus(id);
                Server.Transfer("statuses.aspx");
            }
            else
            {
                title.InnerText = string.Format("{0} - Delete Status", Util.get_setting("AppTitle", "BugTracker.NET"));

                int bugCount = _statusService.GetBugCountByStatus(id);
                var statusDr = _statusService.GetStatusById(id);
                string statusName = statusDr["st_name"].ToString();

                if (bugCount > 0)
                {
                    Response.Write(string.Format("You can't delete status {0} because some bugs still reference it.", statusName));
                    Response.End();
                }
                else
                {
                    confirm_href.InnerText = string.Format("Confirm delete of {0}", statusName);

                    row_id.Value = id.ToString();
                }
            }
        }
    }
}
