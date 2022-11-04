using btnet;
using BugTracker.Web.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class complete_registration : Page
    {
		SignInService signInService = new SignInService();

		void Page_Load(Object sender, EventArgs e)
		{
			Util.set_context(HttpContext.Current);
			Util.do_not_cache(Response);

			string linkId = Request["id"];

			if(signInService.IsRegistrationLinkValid(linkId))
            {

				signInService.AddConfirmedUser(linkId);

				msg.InnerHtml = "Your registration is complete.";
			}
            else
            {
				msg.InnerHtml = "The link you clicked on is no longer valid.<br>Please register again.";
			}

		}
	}
}
