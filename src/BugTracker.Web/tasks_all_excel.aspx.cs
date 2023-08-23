using btnet;
using BugTracker.Web.Services.Bug;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace BugTracker.Web
{
    public partial class tasks_all_excel : Page
    {
        protected DataSet ds_tasks;

        protected Security security;
        private TaskService _taskService = new TaskService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            if (security.user.is_admin || security.user.can_view_tasks)
            {
                // allowed
            }
            else
            {
                Response.Write("You are not allowed to view tasks");
                Response.End();
            }

            ds_tasks = _taskService.GetAllTasks(security, 0);
            DataView dv = new DataView(ds_tasks.Tables[0]);

            Util.print_as_excel(Response, dv);
        }
    }
}
