using btnet;
using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_subscriber : Page
    {
		Security security;

		///////////////////////////////////////////////////////////////////////
		void Page_Load(Object sender, EventArgs e)
		{
			Util.do_not_cache(Response);

			int bugId = 0;
			int userId = 0;

			security = new Security();
			security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

			if (Request.QueryString["ses"] != (string)Session["session_cookie"])
			{
				Response.Write("session in URL doesn't match session cookie");
				Response.End();
			}

			if(!Int32.TryParse(Request["bg_id"], out bugId))
			{
				Response.Write("Not able to find this item");
				Response.End();
			}

			if (!Int32.TryParse(Request["us_id"], out userId))
			{
				Response.Write("Not able to find this user");
				Response.End();
			}

			string sql = "delete from bug_subscriptions where bs_bug = @bg_id and bs_user = @us_id";

			SqlCommand cmd = new SqlCommand(sql);
			cmd.Parameters.AddWithValue("@bg_id", bugId);
			cmd.Parameters.AddWithValue("@us_id", userId);
			
			btnet.DbUtil.execute_nonquery(cmd);

			Response.Redirect("view_subscribers.aspx?id=" + bugId);

		}
	}
}
