using btnet;
using BugTracker.Web.Services.Attachment;
using System;
using System.Collections.Generic;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class edit_attachment : Page
    {
        int id;
        protected int bugid;
        String sql;

        protected Security security;

        private AttachmentService _attachmentService = new AttachmentService();

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

            titl.InnerText = String.Format("{0} - Edit Attachment", Util.get_setting("AppTitle", "BugTracker.NET"));
            msg.InnerText = "";

            int.TryParse(Request["id"], out id);
            int.TryParse(Request["bug_id"], out bugid);

            int permission_level = btnet.Bug.get_bug_permission_level(bugid, security);
            if (permission_level != Security.PERMISSION_ALL)
            {
                Response.Write("You are not allowed to edit this item");
                Response.End();
            }

            if (security.user.external_user || Util.get_setting("EnableInternalOnlyPosts", "0") == "0")
            {
                internal_only.Visible = false;
                internal_only_label.Visible = false;
            }

            if (!IsPostBack)
            {
                // Get this entry's data from the db and fill in the form
                DataRow dr = _attachmentService.GetAttachmentData(id);

                // Fill in this form
                desc.Value = (string)dr["bp_comment"];
                filename.InnerText = (string)dr["bp_file"];
                internal_only.Checked = Convert.ToBoolean((int)dr["bp_hidden_from_external_users"]);
            }
            else
            {
                UpdateAttachment();
            }
        }
        
        void UpdateAttachment()
        {
            string comment = desc.Value.Replace("'", "''");
            _attachmentService.UpdateAttachment(id, comment, internal_only.Checked);

            if (!internal_only.Checked)
            {
                btnet.Bug.send_notifications(btnet.Bug.UPDATE, bugid, security);
            }

            Response.Redirect("edit_bug.aspx?id=" + Convert.ToString(bugid));
        }
    }
}
