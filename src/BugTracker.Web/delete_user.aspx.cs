using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_user : Page
    {
        string sql;

        protected Security security;

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN_OR_PROJECT_ADMIN);

            string id = Util.sanitize_integer(Request["id"]);

            if (!security.user.is_admin)
            {
                sql = @"select us_created_user, us_admin from users where us_id = $us";
                sql = sql.Replace("$us", id);
                DataRow dr = btnet.DbUtil.get_datarow(sql);

                if (security.user.usid != (int)dr["us_created_user"])
                {
                    Response.Write("You not allowed to delete this user, because you didn't create it.");
                    Response.End();
                }
                else if ((int)dr["us_admin"] == 1)
                {
                    Response.Write("You not allowed to delete this user, because it is an admin.");
                    Response.End();
                }
            }

            if (IsPostBack)
            {
                // do delete here
                sql = @"
delete from emailed_links where el_username in (select us_username from users where us_id = $us)
delete users where us_id = $us
delete project_user_xref where pu_user = $us
delete bug_subscriptions where bs_user = $us
delete bug_user where bu_user = $us
delete queries where qu_user = $us
delete queued_notifications where qn_user = $us
delete dashboard_items where ds_user = $us";

                sql = sql.Replace("$us", Util.sanitize_integer(row_id.Value));
                btnet.DbUtil.execute_nonquery(sql);
                Server.Transfer("users.aspx");
            }
            else
            {
                titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete user";

                sql = @"declare @cnt int
select @cnt = count(1) from bugs where bg_reported_user = $us or bg_assigned_to_user = $us
if @cnt = 0
begin
	select @cnt = count(1) from bug_posts where bp_user = $us
end
select us_username, @cnt [cnt] from users where us_id = $us";


                sql = sql.Replace("$us", id);

                DataRow dr = btnet.DbUtil.get_datarow(sql);

                if ((int)dr["cnt"] > 0)
                {
                    Response.Write("You can't delete user \""
                        + Convert.ToString(dr["us_username"])
                        + "\" because some bugs or bug posts still reference it.");
                    Response.End();
                }
                else
                {

                    confirm_href.InnerText = "confirm delete of \""
                        + Convert.ToString(dr["us_username"])
                        + "\"";

                    row_id.Value = id;

                }
            }

        }
    }
}
