using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class dashboard : Page
    {
        protected Security security;
        protected DataSet ds = null;

        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            //titl.InnerText = String.Format("{0} - Dashboard", Util.get_setting("AppTitle", "BugTracker.NET")); 

            if (security.user.is_admin || security.user.can_use_reports)
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            string sql = @"
                select ds.*, rp_desc
                from dashboard_items ds
                inner join reports on rp_id = ds_report
                where ds_user = $us
                order by ds_col, ds_row";

            sql = sql.Replace("$us", Convert.ToString(security.user.usid));
            ds = btnet.DbUtil.get_dataset(sql);

        }

        protected void write_column(int col)
        {
            int iframe_id = 0;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if ((int)dr["ds_col"] == col)
                {
                    if ((string)dr["ds_chart_type"] == "data")
                    {
                        iframe_id++;
                        Response.Write("\n<div class=panel>");
                        Response.Write("\n<iframe frameborder='0' src=view_report.aspx?view=data&id=" + dr["ds_report"] + "></iframe>");
                        Response.Write("\n</div>");
                    }
                    else
                    {
                        Response.Write("\n<div class=panel>");
                        Response.Write("\n<img src=view_report.aspx?scale=2&view=" + dr["ds_chart_type"] + "&id=" + dr["ds_report"] + ">");
                        Response.Write("\n</div>");
                    }
                }
            }
        }
    }
}
