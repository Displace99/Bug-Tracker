using BugTracker.Web.Models;
using BugTracker.Web.Services.Project;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class project_list : System.Web.UI.Page
    {
        private ProjectService _projectService = new ProjectService();

        protected List<ProjectVM> projectList;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoggedIn masterPage = Page.Master as LoggedIn;
            masterPage.security = new btnet.Security();
            masterPage.security.check_security(HttpContext.Current, btnet.Security.ANY_USER_OK);
            masterPage.pageLink = "projects";

            int userID = masterPage.security.user.usid;
            string defaultPermissionLevel = btnet.Util.get_setting("DefaultPermissionLevel", "2");

            projectList = _projectService.GetUserProjectList(userID, defaultPermissionLevel);
        }
    }
}