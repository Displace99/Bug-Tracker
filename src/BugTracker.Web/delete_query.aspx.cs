using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_query : Page
    {
        private string sql;

        public Security security;

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();

            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            if (IsPostBack)
            {
                // do delete here
                sql = @"delete queries where qu_id = $1";
                sql = sql.Replace("$1", Util.sanitize_integer(row_id.Value));
                btnet.DbUtil.execute_nonquery(sql);
                Server.Transfer("queries.aspx");
            }
            else
            {
                titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete query";

                string id = Util.sanitize_integer(Request["id"]);

                sql = @"select qu_desc, isnull(qu_user,0) qu_user from queries where qu_id = $1";
                sql = sql.Replace("$1", id);

                DataRow dr = btnet.DbUtil.get_datarow(sql);

                if ((int)dr["qu_user"] != security.user.usid)
                {
                    if (security.user.is_admin || security.user.can_edit_sql)
                    {
                        // can do anything
                    }
                    else
                    {
                        Response.Write("You are not allowed to delete this item");
                        Response.End();
                    }
                }

                confirm_href.InnerText = "confirm delete of query: "
                        + Convert.ToString(dr["qu_desc"]);

                row_id.Value = id;

            }
        }
    }
}
