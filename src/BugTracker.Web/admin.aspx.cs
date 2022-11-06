using btnet;
using System;
using System.Web;
using System.Web.UI;

namespace BugTracker.Web
{
    public partial class admin : Page
    {
		protected Security security;

		void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

			title.InnerText = String.Format("{0} - admin", Util.get_setting("AppTitle", "BugTracker.NET")); 

		}
	}
}
