using btnet;
using BugTracker.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class logoff : Page
    {
		SignInService signInService = new SignInService();

		void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			Util.set_context(HttpContext.Current);


			btnet.DbUtil.get_sqlconnection();

			// delete the session row

			HttpCookie cookie = Request.Cookies["se_id"];

			if (cookie != null)
			{

				string se_id = cookie.Value.Replace("'", "''");

				//Removes session from database
				signInService.RemoveSession(se_id);
				

				//Removes infomriaton from the actual session object
				Session[se_id] = 0;
				Session["SelectedBugQuery"] = null;
				Session["bugs"] = null;
				Session["bugs_unfiltered"] = null;
				Session["project"] = null;

			}

			Response.Redirect("default.aspx?msg=logged+off");
		}
	}
}
