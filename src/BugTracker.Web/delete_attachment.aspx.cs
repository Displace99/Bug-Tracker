using btnet;
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

            string attachment_id_string = Util.sanitize_integer(Request["id"]);
            string bug_id_string = Util.sanitize_integer(Request["bug_id"]);

            int permission_level = btnet.Bug.get_bug_permission_level(Convert.ToInt32(bug_id_string), security);
            if (permission_level != Security.PERMISSION_ALL)
            {
                Response.Write("You are not allowed to edit this item");
                Response.End();
            }


            if (IsPostBack)
            {
                // save the filename before deleting the row
                sql = @"select bp_file from bug_posts where bp_id = $ba";
                sql = sql.Replace("$ba", attachment_id_string);
                string filename = (string)btnet.DbUtil.execute_scalar(sql);

                // delete the row representing the attachment
                sql = @"delete bug_post_attachments where bpa_post = $ba
            delete bug_posts where bp_id = $ba";
                sql = sql.Replace("$ba", attachment_id_string);
                btnet.DbUtil.execute_nonquery(sql);

                // delete the file too
                string upload_folder = Util.get_upload_folder();
                if (upload_folder != null)
                {
                    StringBuilder path = new StringBuilder(upload_folder);
                    path.Append("\\");
                    path.Append(bug_id_string);
                    path.Append("_");
                    path.Append(attachment_id_string);
                    path.Append("_");
                    path.Append(filename);
                    if (System.IO.File.Exists(path.ToString()))
                    {
                        System.IO.File.Delete(path.ToString());
                    }
                }


                Response.Redirect("edit_bug.aspx?id=" + bug_id_string);
            }
            else
            {
                titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete attachment";

                back_href.HRef = "edit_bug.aspx?id=" + bug_id_string;

                sql = @"select bp_file from bug_posts where bp_id = $1";
                sql = sql.Replace("$1", attachment_id_string);

                DataRow dr = btnet.DbUtil.get_datarow(sql);

                string s = Convert.ToString(dr["bp_file"]);

                confirm_href.InnerText = "confirm delete of attachment: " + s;

                row_id.Value = attachment_id_string;
            }

        }
    }
}
