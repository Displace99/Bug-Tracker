using btnet;
using BugTracker.Web.Services.Organization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class orgs : Page
    {
        protected DataSet ds;

        protected Security security;
        private OrganizationService _orgService = new OrganizationService();

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "organizations";

            ds = _orgService.GetOrganizationList();

        }
    }
}
