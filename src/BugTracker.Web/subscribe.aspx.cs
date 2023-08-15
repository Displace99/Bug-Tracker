using btnet;
using BugTracker.Web.Services.Subscription;
using System;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class subscribe : Page
    {
		private SubscriptionService _subscriptionService = new SubscriptionService();

		void Page_Load(Object sender, EventArgs e)
		{
			string sql;
			Security security;

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

			int userId = security.user.usid;
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
				_subscriptionService.AddSubscription(bugid, userId);
			}
			else
			{
				_subscriptionService.DeleteSubscription(bugid, userId);
			}
		}
	}
}
