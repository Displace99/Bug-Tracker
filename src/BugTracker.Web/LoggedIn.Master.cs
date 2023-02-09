using btnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class LoggedIn : System.Web.UI.MasterPage
    {
        public Security security = new btnet.Security();
        public string pageLink;

        protected void Page_Load(object sender, EventArgs e)
        {
            //All logic is handled in aspx page.
        }
    }
}