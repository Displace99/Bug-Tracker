using btnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web.Controls
{
    public partial class MainMenu : System.Web.UI.UserControl
    {
        public User user = new User();
        public string SelectedItem { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}