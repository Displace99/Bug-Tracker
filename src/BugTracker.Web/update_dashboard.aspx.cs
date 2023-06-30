using btnet;
using BugTracker.Web.Services.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class update_dashboard : Page
    {
        protected Security security;

        private DashboardService _dashboardService = new DashboardService();

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

            int userId = security.user.usid;

            if (security.user.is_admin || security.user.can_use_reports)
            {
                //Do nothing as they have proper access
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            if (Request.QueryString["ses"] != (string)Session["session_cookie"])
            {
                Response.Write("session in URL doesn't match session cookie");
                Response.End();
            }

            string action = Request["actn"];
            if (string.IsNullOrEmpty(action))
            {
                Response.End();
            }

            string sql = "";

            if (action == "add")
            {
                int rp_id = Convert.ToInt32(Util.sanitize_integer(Request["rp_id"]));
                int rp_col = Convert.ToInt32(Util.sanitize_integer(Request["rp_col"]));
                string chartType = ((string)Request["rp_chart_type"]).Replace("'", "''");

                _dashboardService.AddDashboardItem(userId, rp_id, chartType, rp_col);
                Response.Redirect("edit_dashboard.aspx");
            }
            else if (action == "delete")
            {
                int ds_id = Convert.ToInt32(Util.sanitize_integer(Request["ds_id"]));

                _dashboardService.DeleteDashboardItem(ds_id, userId);
                Response.Redirect("edit_dashboard.aspx");
            }
            else if (action == "moveup" || action == "movedown")
            {
                int ds_id = Convert.ToInt32(Util.sanitize_integer(Request["ds_id"]));
                int delta = action == "moveup" ? -1 : 1;

                _dashboardService.MoveDashboardItem(ds_id, delta, userId);
                Response.Redirect("edit_dashboard.aspx");
            }
        }
    }
}
