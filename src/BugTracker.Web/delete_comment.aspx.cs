using btnet;
using BugTracker.Web.Services.Comment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_comment : Page
    {
        String sql;

        protected Security security;
        private CommentService _commentService = new CommentService();

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

            if (IsPostBack)
            {
                // do delete here
                int commentId = int.Parse(row_id.Value);

                _commentService.DeleteComment(commentId);
                Response.Redirect("edit_bug.aspx?id=" + btnet.Util.sanitize_integer(redirect_bugid.Value));
            }
            else
            {

                string bug_id = Util.sanitize_integer(Request["bug_id"]);
                redirect_bugid.Value = bug_id;

                int permission_level = btnet.Bug.get_bug_permission_level(Convert.ToInt32(bug_id), security);
                if (permission_level != Security.PERMISSION_ALL)
                {
                    Response.Write("You are not allowed to edit this item");
                    Response.End();
                }

                titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete comment";

                string id = Util.sanitize_integer(Request["id"]);

                back_href.HRef = "edit_bug.aspx?id=" + bug_id;

                sql = @"select bp_comment from bug_posts where bp_id = $1";
                sql = sql.Replace("$1", id);

                DataRow dr = btnet.DbUtil.get_datarow(sql);

                // show the first few chars of the comment
                string s = Convert.ToString(dr["bp_comment"]);
                int len = 20;
                if (s.Length < len) { len = s.Length; }

                confirm_href.InnerText = "confirm delete of comment: "
                        + s.Substring(0, len)
                        + "...";

                row_id.Value = id;
            }


        }
    }
}
