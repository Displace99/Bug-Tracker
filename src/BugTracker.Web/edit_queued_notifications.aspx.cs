using btnet;
using BugTracker.Web.Services.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class edit_queued_notifications : Page
    {
        Security security;
        private NotificationService _notificationService = new NotificationService();

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            if (Request.QueryString["ses"] != (string)Session["session_cookie"])
            {
                Response.Write("session in URL doesn't match session cookie");
                Response.End();
            }

            if (Request.QueryString["actn"] == "delete")
            {
                _notificationService.DeleteNotifications();
            }
            else if (Request.QueryString["actn"] == "reset")
            {
                _notificationService.UpdateNotifications();
            }
            else if (Request.QueryString["actn"] == "resend")
            {
                // spawn a worker thread to send the emails
                System.Threading.Thread thread = new System.Threading.Thread(btnet.Bug.threadproc_notifications);
                thread.Start();
            }

            Response.Redirect("notifications.aspx");
        }
    }
}
