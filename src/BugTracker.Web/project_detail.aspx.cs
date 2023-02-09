using btnet;
using BugTracker.Web.Models;
using BugTracker.Web.Services.Bug;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

namespace BugTracker.Web
{
    public partial class project_detail : System.Web.UI.Page
    {
        private BugService _bugService = new BugService();

        protected List<ProjectBugVM> BugList;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoggedIn masterPage = Page.Master as LoggedIn;
            masterPage.security = new btnet.Security();
            masterPage.security.check_security(HttpContext.Current, btnet.Security.ANY_USER_OK);
            masterPage.pageLink = "projects";

            lblBugLabel.Text = Util.get_setting("PluralBugLabel", "Bug");

            int projectId = 0;
            BugList = new List<ProjectBugVM>();

            if (Int32.TryParse(Request.QueryString["projectId"], out projectId))
            {
                BugList = _bugService.GetBugsByProject(projectId);
            }
            
        }
    }
}