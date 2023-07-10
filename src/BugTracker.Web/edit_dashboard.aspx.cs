using btnet;
using BugTracker.Web.Services.Dashboard;
using System;
using System.Data;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class edit_dashboard : Page
    {
        protected Security security;
        protected DataSet ds = null;
        protected string ses = "";

        private DashboardService _dashboardService = new DashboardService();

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

            titl.InnerText = String.Format("{0} - Edit Dashboard", Util.get_setting("AppTitle", "BugTracker.NET"));

            if (security.user.is_admin || security.user.can_use_reports)
            {
                //Do nothing since they have access to page
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            ses = (string)Session["session_cookie"];

            ds = _dashboardService.GetDashboardItems(security.user.usid);

        }

        void write_link(int id, string action, string text)
        {
            Response.Write("<a href=update_dashboard.aspx?actn=");
            Response.Write(action);
            Response.Write("&ds_id=");
            Response.Write(Convert.ToString(id));
            Response.Write("&ses=");
            Response.Write(ses);
            Response.Write(">[");
            Response.Write(text);
            Response.Write("]</a>&nbsp;&nbsp;&nbsp;");
        }

        protected void write_column(int col)
        {
            bool first_row = true;
            int last_row = -1;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if ((int)dr["ds_col"] == col)
                {
                    last_row = (int)dr["ds_row"];
                }
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if ((int)dr["ds_col"] == col)
                {
                    Response.Write("<div class=panel>");

                    write_link((int)dr["ds_id"], "delete", "delete");

                    if (first_row)
                    {
                        first_row = false;
                    }
                    else
                    {
                        write_link((int)dr["ds_id"], "moveup", "move up");
                    }

                    if ((int)dr["ds_row"] == last_row)
                    {
                        // skip
                    }
                    else
                    {
                        write_link((int)dr["ds_id"], "movedown", "move down");
                    }

                    Response.Write("<p><div style='text-align: center; font-weight: bold;'>");
                    Response.Write((string)dr["rp_desc"] + "&nbsp;-&nbsp; " + (string)dr["ds_chart_type"]);
                    Response.Write("</div>");

                    Response.Write("</div>");
                }
            }
        }
    }
}
