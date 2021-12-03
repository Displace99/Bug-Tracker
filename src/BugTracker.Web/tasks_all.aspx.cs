using System;
using System.Data;
using System.Web;
using System.Web.UI;
using btnet;

namespace BugTracker.Web
{
    public partial class tasks_all : Page
    {
		public DataSet ds_tasks;

		public Security security;

		public void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

		public void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.ANY_USER_OK);

			titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
					+ "all tasks";

			if (security.user.is_admin || security.user.can_view_tasks)
			{
				// allowed
			}
			else
			{
				Response.Write("You are not allowed to view tasks");
				Response.End();
			}

			ds_tasks = btnet.Util.get_all_tasks(security, 0);
		}
	}
}
