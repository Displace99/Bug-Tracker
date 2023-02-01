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
    public partial class delete_org : Page
    {
        protected Security security;
        private OrganizationService _orgService = new OrganizationService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            int orgId = 0;
            Int32.TryParse(Request["id"], out orgId);

            if (IsPostBack)
            {
                // do delete here
                _orgService.DeleteOrganization(orgId);
                Server.Transfer("orgs.aspx");
            }
            else
            {
                titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete organization";

                bool hasRelatedEntities = _orgService.DoesOrgHaveRelatedEntities(orgId);

                if (hasRelatedEntities)
                {
                    Response.Write("You can't delete organization because some bugs, users, queries still reference it.");
                    Response.End();
                }
                else
                {
                    string orgName = _orgService.GetOrgNameById(orgId);

                    confirm_href.InnerText = string.Format("confirm delete of {0}", orgName);

                    row_id.Value = orgId.ToString();
                }
            }
        }
    }
}
