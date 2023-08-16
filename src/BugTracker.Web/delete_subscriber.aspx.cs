using btnet;
using BugTracker.Web.Services.Subscription;
using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_subscriber : Page
    {
		Security security;
		private SubscriptionService _subscriptionService = new SubscriptionService();

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

			_subscriptionService.DeleteSubscription(bugId, userId);

			Response.Redirect("view_subscribers.aspx?id=" + bugId);

		}
	}
}
