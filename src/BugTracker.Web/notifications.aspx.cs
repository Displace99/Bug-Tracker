using btnet;
using BugTracker.Web.Services.Notification;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class notifications : Page
    {
        protected DataSet ds;
        protected Security security;
        protected string ses;

        private NotificationService _notificationService = new NotificationService();

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "queued notifications";

            ds = _notificationService.GetQueuedNotifications();

            ses = (string)Session["session_cookie"];
        }
    }
}
