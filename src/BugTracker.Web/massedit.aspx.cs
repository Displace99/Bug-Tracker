using btnet;
using BugTracker.Web.Services.Attachment;
using BugTracker.Web.Services.Bug;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class massedit : Page
    {
        //String sql;

        protected Security security;
        private BugService _bugService = new BugService();
        private AttachmentService _attachmentService = new AttachmentService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }
        
        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();

            security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

            if (security.user.is_admin || security.user.can_mass_edit_bugs)
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }


            //string list = "";
            List<int> bugList = new List<int>();
            int projectId = 0;
            int orgId = 0;
            int categoryId = 0;
            int priorityId = 0;
            int assignedTo = 0;
            int reportedBy = 0;
            int statusId = 0;

            int.TryParse(Request["mass_project"], out projectId);
            int.TryParse(Request["mass_org"], out orgId);
            int.TryParse(Request["mass_category"], out categoryId);
            int.TryParse(Request["mass_priority"], out priorityId);
            int.TryParse(Request["mass_assigned_to"], out assignedTo);
            int.TryParse(Request["mass_reported_by"], out reportedBy);
            int.TryParse(Request["mass_status"], out statusId);

            // create list of bugs affected
            foreach (string var in Request.QueryString)
            {
                if (Util.is_int(var))
                {
                    bugList.Add(int.Parse(var));
                };
            }

            if (!IsPostBack)
            {
                titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "massedit";

                if (Request["mass_delete"] != null)
                {
                    update_or_delete.Value = "delete";
                    confirm_href.InnerText = "Confirm Delete";
                }
                else
                {
                    update_or_delete.Value = "update";
                    confirm_href.InnerText = "Confirm Update";
                }

            }
            else // postback
            {
                if (update_or_delete.Value == "delete")
                {
                    _attachmentService.MassDeleteAttachments(bugList);
                    _bugService.MassDeleteBugs(bugList);
                }
                else
                {
                    _bugService.MassUpdateBugs(bugList, projectId, orgId, categoryId, priorityId, assignedTo, reportedBy, statusId);
                }

                Response.Redirect("search.aspx");
            }
        }
    }
}
