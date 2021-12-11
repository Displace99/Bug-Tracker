using btnet;
using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class subscribe : Page
    {
		void Page_Load(Object sender, EventArgs e)
		{
			string sql;
			Security security;

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

			int bugid = Convert.ToInt32(Request["id"]);
			int permission_level = Bug.get_bug_permission_level(bugid, security);
			if (permission_level == Security.PERMISSION_NONE)
			{
				Response.End();
			}

			if (Request.QueryString["ses"] != (string)Session["session_cookie"])
			{
				Response.Write("session in URL doesn't match session cookie");
				Response.End();
			}

			if (Request.QueryString["actn"] == "1")
			{
				sql = @"insert into bug_subscriptions (bs_bug, bs_user)
			values(@bg, @us)";
			}
			else
			{
				sql = @"delete from bug_subscriptions
			where bs_bug = @bg and bs_user = @us";
			}

			SqlCommand cmd = new SqlCommand(sql);
			cmd.Parameters.AddWithValue("@bg", bugid);
			cmd.Parameters.AddWithValue("@us", security.user.usid);
			
			btnet.DbUtil.execute_nonquery(cmd);

		}
	}
}
