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
    public partial class merge_bug : Page
    {
        String sql;

        protected Security security;
        DataRow fromBugDr;
        DataRow toBugDr;

        private BugService _bugService = new BugService();
        private AttachmentService _attachmentService = new AttachmentService();

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();

            security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

            if (security.user.is_admin || security.user.can_merge_bugs)
            {
                //Do nothing as they have proper access
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "merge " + Util.get_setting("SingularBugLabel", "bug");

            if (!IsPostBack)
            {
                string orig_id_string = Util.sanitize_integer(Request["id"]);
                orig_id.Value = orig_id_string;
                back_href.HRef = "edit_bug.aspx?id=" + orig_id_string;
                from_bug.Value = orig_id_string;
            }
            else
            {
                from_err.InnerText = "";
                into_err.InnerText = "";
                on_update();
            }
        }

        bool ValidateForm()
        {
            bool good = true;

            // validate FROM
            if (from_bug.Value == "")
            {
                from_err.InnerText = "\"From\" bug is required.";
                good = false;
            }
            else
            {
                if (!int.TryParse(from_bug.Value, out int fromBug))
                {
                    from_err.InnerText = "\"From\" bug is not valid.";
                    good = false;
                }
            }

            // validate INTO
            if (into_bug.Value == "")
            {
                into_err.InnerText = "\"Into\" bug is required.";
                good = false;
            }
            else
            {
                if (!int.TryParse(into_bug.Value, out int intoBug))
                {
                    into_err.InnerText = "\"Into\" bug is not valid.";
                    good = false;
                }
            }

            if (!good)
            {
                return false;
            }

            if (from_bug.Value == into_bug.Value)
            {
                from_err.InnerText = "\"From\" bug cannot be the same as \"Into\" bug.";
                return false;
            }

            //Searches for the "FROM" bug
            fromBugDr = _bugService.GetBugById(int.Parse(from_bug.Value));
            if (fromBugDr == null)
            {
                from_err.InnerText = "\"From\" bug not found.";
                good = false;
            }

            //Searches for the "INTO" bug
            toBugDr = _bugService.GetBugById(int.Parse(into_bug.Value));
            if (toBugDr == null)
            {
                into_err.InnerText = "\"Into\" bug not found.";
                good = false;
            }

            return good;
        }


        void on_update()
        {
            // does it say "Merge" or "Confirm Merge"?
            if (submit.Value == "Merge")
            {
                if (!ValidateForm())
                {
                    prev_from_bug.Value = "";
                    prev_into_bug.Value = "";
                    return;
                }
            }

            //This if statement will only be true on the "Confirm Merge" action
            if (prev_from_bug.Value == from_bug.Value
            && prev_into_bug.Value == into_bug.Value)
            {
                //We don't need a try parse here because it was already sanitized before we assigned it.
                //If it is no longer an integer at this point then the hidden field was altered. 
                int fromBugId = int.Parse(prev_from_bug.Value);
                int intoBugId = int.Parse(prev_into_bug.Value);

                _attachmentService.MoveAttachmentToOtherBug(fromBugId, intoBugId);
                
                _bugService.MergeBugs(fromBugId, intoBugId);

                // record the merge itself

                sql = @"insert into bug_posts
			(bp_bug, bp_user, bp_date, bp_type, bp_comment, bp_comment_search)
			values($into,$us,getdate(), 'comment', 'merged bug $from into this bug:', 'merged bug $from into this bug:')
			select scope_identity()";

                sql = sql.Replace("$from", fromBugId.ToString());
                sql = sql.Replace("$into", intoBugId.ToString());
                sql = sql.Replace("$us", Convert.ToString(security.user.usid));

                int comment_id = Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));

                // update bug comments with info from old bug
                sql = @"update bug_posts
			set bp_comment = convert(nvarchar,bp_comment) + char(10) + bg_short_desc
			from bugs where bg_id = $from
			and bp_id = $bc";

                sql = sql.Replace("$from", fromBugId.ToString());
                sql = sql.Replace("$bc", Convert.ToString(comment_id));
                btnet.DbUtil.execute_nonquery(sql);

                // delete the from bug
                int from_bugid = Convert.ToInt32(fromBugId.ToString());
                Bug.delete_bug(from_bugid);

                // delete the from bug from the list, if there is a list
                DataView dv_bugs = (DataView)Session["bugs"];
                if (dv_bugs != null)
                {
                    // read through the list of bugs looking for the one that matches the from
                    int index = 0;
                    foreach (DataRowView drv in dv_bugs)
                    {
                        if (from_bugid == (int)drv[1])
                        {
                            dv_bugs.Delete(index);
                            break;
                        }
                        index++;
                    }
                }

                btnet.Bug.send_notifications(btnet.Bug.UPDATE, intoBugId, security);

                Response.Redirect("edit_bug.aspx?id=" + intoBugId);
            }
            /*This will be hit the first time the "Merge" button is clicked and will set some values for the 
              "Confirm Merge action */
            else
            {
                prev_from_bug.Value = from_bug.Value;
                prev_into_bug.Value = into_bug.Value;
                static_from_bug.InnerText = from_bug.Value;
                static_into_bug.InnerText = into_bug.Value;
                static_from_desc.InnerText = (string)fromBugDr["bg_short_desc"];
                static_into_desc.InnerText = (string)toBugDr["bg_short_desc"];
                from_bug.Style["display"] = "none";
                into_bug.Style["display"] = "none";
                static_from_bug.Style["display"] = "";
                static_into_bug.Style["display"] = "";
                static_from_desc.Style["display"] = "";
                static_into_desc.Style["display"] = "";
                submit.Value = "Confirm Merge";
            }
        }
    }
}
