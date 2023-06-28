using btnet;
using BugTracker.Web.Services.Attachment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_attachment : Page
    {
        private string sql;
        private AttachmentService _attachmentService = new AttachmentService();

        protected Security security;

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();

            security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

            if (security.user.is_admin || security.user.can_edit_and_delete_posts)
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            int attachmentId = 0;
            int bugId = 0;

            int.TryParse(Request["id"], out attachmentId);
            int.TryParse(Request["bug_id"], out bugId);

            int permission_level = Bug.get_bug_permission_level(bugId, security);
            if (permission_level != Security.PERMISSION_ALL)
            {
                Response.Write("You are not allowed to edit this item");
                Response.End();
            }


            if (IsPostBack)
            {
                string filename = _attachmentService.GetAttachmentFileName(attachmentId);

                _attachmentService.DeleteAttachment(attachmentId, bugId, filename);

                Response.Redirect("edit_bug.aspx?id=" + bugId);
            }
            else
            {
                titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete attachment";

                back_href.HRef = "edit_bug.aspx?id=" + bugId;

                sql = @"select bp_file from bug_posts where bp_id = $1";
                sql = sql.Replace("$1", attachmentId.ToString());

                DataRow dr = btnet.DbUtil.get_datarow(sql);

                string s = Convert.ToString(dr["bp_file"]);

                confirm_href.InnerText = "confirm delete of attachment: " + s;

                row_id.Value = attachmentId.ToString();
            }
        }
    }
}
