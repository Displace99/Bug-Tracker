using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class print_bugs2 : Page
    {
        String sql;


        protected Security security;
        protected DataSet ds = null;
        protected DataView dv = null;
        protected bool images_inline;
        protected bool history_inline;

        ///////////////////////////////////////////////////////////////////////
        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "print " + Util.get_setting("PluralBugLabel", "bugs");


            // are we doing the query to get the bugs or are we using the cached dataview?
            string qu_id_string = Request.QueryString["qu_id"];

            if (qu_id_string != null)
            {

                // use sql specified in query string
                int qu_id = Convert.ToInt32(qu_id_string);
                sql = @"select qu_sql from queries where qu_id = $1";
                sql = sql.Replace("$1", qu_id_string);
                string bug_sql = (string)btnet.DbUtil.execute_scalar(sql);

                // replace magic variables
                bug_sql = bug_sql.Replace("$ME", Convert.ToString(security.user.usid));
                bug_sql = Util.alter_sql_per_project_permissions(bug_sql, security);

                // all we really need is the bugid, but let's do the same query as print_bugs.aspx
                ds = btnet.DbUtil.get_dataset(bug_sql);
            }
            else
            {
                dv = (DataView)Session["bugs"];
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
