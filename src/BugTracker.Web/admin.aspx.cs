using btnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class admin : Page
    {
		public Security security;
		public bool nag = false;

		///////////////////////////////////////////////////////////////////////
		void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

			titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
				+ "admin";

			if (false) // change this to if(true) to make the donation nag message go away
			{

			}
			else
			{
				int bugs = Convert.ToInt32(btnet.DbUtil.execute_scalar("select count(1) from bugs"));
				if (bugs > 3)
				{
					nag = true;
				}
			}
		}
	}
}
