using btnet;
using BugTracker.Web.Services.Status;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class statuses : Page
    {
        protected DataSet ds;
        protected Security security;

        private StatusService _statusService = new StatusService();

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);


            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "statuses";

            ds = _statusService.GetStatusList();
        }

    }
}
