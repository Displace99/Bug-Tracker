using btnet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using BugTracker.Web.Services.Attachment;

namespace BugTracker.Web
{
    public partial class view_attachment : Page
    {
        //Copyright 2002-2011 Corey Trager
        //Distributed under the terms of the GNU General Public License

        protected Security security;
        private AttachmentService _attachmentService = new AttachmentService();

        void Page_Load(Object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request["bug_id"]))
            {
                // This is to prevent exceoptions and error emails from getting triggered
                // by "Microsoft Office Existence Discovery".  Google it for more info.
                Response.End();
            }

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            //This is used further down the page.
            string bp_id = btnet.Util.sanitize_integer(Request["id"]);
            string bug_id = btnet.Util.sanitize_integer(Request["bug_id"]);

            int bugPostId = 0;
            int bugId = 0;

            int.TryParse(Request["id"], out bugPostId);
            int.TryParse(Request["bug_id"], out bugId);

            int permission_level = Bug.get_bug_permission_level(bugId, security);
            if (permission_level == Security.PERMISSION_NONE)
            {
                Response.Write("You are not allowed to view this item");
                Response.End();
            }

            DataRow dr = _attachmentService.GetAttachmentInfo(bugPostId, bugId);

            if (dr == null)
            {
                Response.End();
            }

            string var = Request["download"];
            bool download;
            if (var == null || var == "1")
            {
                download = true;
            }
            else
            {
                download = false;
            }

            string filename = (string)dr["bp_file"];
            string content_type = (string)dr["bp_content_type"];

            _attachmentService.GetAttachment(Response, bugPostId, bugId, filename, content_type, download);  
        } 
    }
}
