using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class project_list : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            BugTracker masterPage = Page.Master as BugTracker;
            masterPage.security = new btnet.Security();
            masterPage.security.check_security(HttpContext.Current, btnet.Security.ANY_USER_OK);
        }
    }
}