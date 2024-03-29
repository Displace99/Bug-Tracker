using btnet;
using BugTracker.Web.Models.Comment;
using BugTracker.Web.Services.Comment;
using BugTracker.Web.Services.Email;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class send_email : Page
    {
        protected string sql;

        protected Security security;
        protected int project = -1;
        protected bool enable_internal_posts = false;

        private EmailService _emailService = new EmailService();
        private CommentService _commentService = new CommentService();

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

            titl.InnerText = string.Format("{0} - Send Email", Util.get_setting("AppTitle", "BugTracker.NET"));

            msg.InnerText = "";

            string string_bp_id = Request["bp_id"];
            string string_bg_id = Request["bg_id"];
            string request_to = Request["to"];
            string reply = Request["reply"];

            enable_internal_posts = (Util.get_setting("EnableInternalOnlyPosts", "0") == "1");

            if (!enable_internal_posts)
            {
                include_internal_posts.Visible = false;
                include_internal_posts_label.Visible = false;
            }

            if (!IsPostBack)
            {
                Session["email_addresses"] = null;

                DataRow dr = null;

                //This is true if you are forwarding a specific comment on a bug.
                //When would we forward a specific comment instead of just the entire bug?
                //This will be removed in a future update.
                if (string_bp_id != null)
                {
                    string_bp_id = Util.sanitize_integer(string_bp_id);
                    
                    int commentId = 0;
                    int.TryParse(string_bg_id, out commentId);

                    DataView dv = _emailService.GetBugInfoByCommentId(commentId, security.user.usid);

                    dr = null;
                    if (dv.Count > 0)
                    {
                        dv.RowFilter = "bp_id = " + string_bp_id;
                        if (dv.Count > 0)
                        {
                            dr = dv[0].Row;
                        }
                    }

                    int int_bg_id = (int)dr["bg_id"];
                    int permission_level = Bug.get_bug_permission_level(int_bg_id, security);
                    if (permission_level == Security.PERMISSION_NONE)
                    {
                        Response.Write("You are not allowed to view this item");
                        Response.End();
                    }

                    if ((int)dr["bp_hidden_from_external_users"] == 1)
                    {
                        if (security.user.external_user)
                        {
                            Response.Write("You are not allowed to view this post");
                            Response.End();
                        }
                    }

                    string_bg_id = Convert.ToString(dr["bg_id"]);
                    back_href.HRef = "edit_bug.aspx?id=" + string_bg_id;
                    bg_id.Value = string_bg_id;


                    to.Value = dr["bp_email_from"].ToString();


                    // Work around for a mysterious bug:
                    // http://sourceforge.net/tracker/?func=detail&aid=2815733&group_id=66812&atid=515837
                    if (Util.get_setting("StripDisplayNameFromEmailAddress", "0") == "1")
                    {
                        to.Value = Email.simplify_email_address(to.Value);
                    }

                    load_from_dropdown(dr, true); // list the project's email address first

                    if (reply != null && reply == "all")
                    {
                        Regex regex = new Regex("\n");
                        string[] lines = regex.Split((string)dr["bp_comment"]);
                        string cc_addrs = "";

                        int max = lines.Length < 5 ? lines.Length : 5;

                        // gather cc addresses, which might include the current user
                        for (int i = 0; i < max; i++)
                        {
                            if (lines[i].StartsWith("To:") || lines[i].StartsWith("Cc:"))
                            {
                                string cc_addr = lines[i].Substring(3, lines[i].Length - 3).Trim();

                                // don't cc yourself

                                if (cc_addr.IndexOf(from.SelectedItem.Value) == -1)
                                {
                                    if (cc_addrs != "")
                                    {
                                        cc_addrs += ",";
                                    }

                                    cc_addrs += cc_addr;
                                }
                            }
                        }

                        cc.Value = cc_addrs;
                    }

                    if (dr["us_signature"].ToString() != "")
                    {
                        if (security.user.use_fckeditor)
                        {
                            body.Value += "<br><br><br>";
                            body.Value += dr["us_signature"].ToString().Replace("\r\n", "<br>");
                            body.Value += "<br><br><br>";
                        }
                        else
                        {
                            body.Value += "\n\n\n";
                            body.Value += dr["us_signature"].ToString();
                            body.Value += "\n\n\n";
                        }
                    }


                    if (Request["quote"] != null)
                    {
                        Regex regex = new Regex("\n");
                        string[] lines = regex.Split((string)dr["bp_comment"]);

                        if (dr["bp_type"].ToString() == "received")
                        {
                            if (security.user.use_fckeditor)
                            {
                                body.Value += "<br><br><br>";
                                body.Value += "&#62;From: " + dr["bp_email_from"].ToString().Replace("<", "&#60;").Replace(">", "&#62;") + "<br>";
                            }
                            else
                            {
                                body.Value += "\n\n\n";
                                body.Value += ">From: " + dr["bp_email_from"] + "\n";
                            }
                        }

                        bool next_line_is_date = false;
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (i < 4 && (lines[i].IndexOf("To:") == 0 || lines[i].IndexOf("Cc:") == 0))
                            {
                                next_line_is_date = true;
                                if (security.user.use_fckeditor)
                                {
                                    body.Value += "&#62;" + lines[i].Replace("<", "&#60;").Replace(">", "&#62;") + "<br>";
                                }
                                else
                                {
                                    body.Value += ">" + lines[i] + "\n";
                                }
                            }
                            else if (next_line_is_date)
                            {
                                next_line_is_date = false;
                                if (security.user.use_fckeditor)
                                {
                                    body.Value += "&#62;Date: " + Convert.ToString(dr["bp_date"]) + "<br>&#62;<br>";
                                }
                                else
                                {
                                    body.Value += ">Date: " + Convert.ToString(dr["bp_date"]) + "\n>\n";
                                }
                            }
                            else
                            {
                                if (security.user.use_fckeditor)
                                {
                                    if (Convert.ToString(dr["bp_content_type"]) != "text/html")
                                    {
                                        body.Value += "&#62;" + lines[i].Replace("<", "&#60;").Replace(">", "&#62;") + "<br>";
                                    }
                                    else
                                    {
                                        if (i == 0)
                                        {
                                            body.Value += "<hr>";
                                        }

                                        body.Value += lines[i];
                                    }
                                }
                                else
                                {
                                    body.Value += ">" + lines[i] + "\n";
                                }
                            }
                        }
                    }

                    if (reply == "forward")
                    {
                        to.Value = "";
                        dv.RowFilter = "bp_type = 'file'";
                        foreach (DataRowView drv in dv)
                        {
                            attachments_label.InnerText = "Select attachments to forward:";
                            lstAttachments.Items.Add(new ListItem(drv["bp_file"].ToString(), drv["bp_id"].ToString()));
                        }

                    }

                }
                else if (string_bg_id != null) //This is true when we are sending an email for a bug
                {

                    string_bg_id = Util.sanitize_integer(string_bg_id);
                    int bugId = 0;
                    int.TryParse(string_bg_id, out bugId);

                    int permission_level = Bug.get_bug_permission_level(Convert.ToInt32(string_bg_id), security);
                    if (permission_level == Security.PERMISSION_NONE
                    || permission_level == Security.PERMISSION_READONLY)
                    {
                        Response.Write("You are not allowed to edit this item");
                        Response.End();
                    }

                    dr = _emailService.GetBugInfoByBugId(bugId, security.user.usid);

                    load_from_dropdown(dr, false); // list the user's email first, then the project

                    back_href.HRef = "edit_bug.aspx?id=" + string_bg_id;
                    bg_id.Value = string_bg_id;

                    if (request_to != null)
                    {
                        to.Value = request_to;
                    }

                    // Work around for a mysterious bug:
                    // http://sourceforge.net/tracker/?func=detail&aid=2815733&group_id=66812&atid=515837
                    if (Util.get_setting("StripDisplayNameFromEmailAddress", "0") == "1")
                    {
                        to.Value = Email.simplify_email_address(to.Value);
                    }

                    if (dr["us_signature"].ToString() != "")
                    {
                        if (security.user.use_fckeditor)
                        {
                            body.Value += "<br><br><br>";
                            body.Value += dr["us_signature"].ToString().Replace("\r\n", "<br>");
                        }
                        else
                        {
                            body.Value += "\n\n\n";
                            body.Value += dr["us_signature"].ToString();
                        }
                    }


                }

                short_desc.Value = (string)dr["bg_short_desc"];

                if (string_bp_id != null || string_bg_id != null)
                {

                    subject.Value = (string)dr["bg_short_desc"]
                        + "  (" + Util.get_setting("TrackingIdString", "DO NOT EDIT THIS:")
                        + bg_id.Value
                        + ")";

                    // for determining which users to show in "address book"
                    project = (int)dr["bg_project"];

                }
            }
            else
            {
                on_update();
            }
        }

        void load_from_dropdown(DataRow dr, bool project_first)
        {
            // format from dropdown
            string project_email = dr["pj_pop3_email_from"].ToString();
            string us_email = dr["us_email"].ToString();
            string us_firstname = dr["us_firstname"].ToString();
            string us_lastname = dr["us_lastname"].ToString();

            if (project_first)
            {
                if (project_email != "")
                {
                    from.Items.Add(new ListItem(project_email));
                    if (us_firstname != "" && us_lastname != "")
                    {
                        from.Items.Add(new ListItem("\"" + us_firstname + " " + us_lastname + "\" <" + project_email + ">"));
                    }
                }

                if (us_email != "")
                {
                    from.Items.Add(new ListItem(us_email));
                    if (us_firstname != "" && us_lastname != "")
                    {
                        from.Items.Add(new ListItem("\"" + us_firstname + " " + us_lastname + "\" <" + us_email + ">"));
                    }
                }
            }
            else
            {
                if (us_email != "")
                {
                    from.Items.Add(new ListItem(us_email));
                    if (us_firstname != "" && us_lastname != "")
                    {
                        from.Items.Add(new ListItem("\"" + us_firstname + " " + us_lastname + "\" <" + us_email + ">"));
                    }
                }

                if (project_email != "")
                {
                    from.Items.Add(new ListItem(project_email));
                    if (us_firstname != "" && us_lastname != "")
                    {
                        from.Items.Add(new ListItem("\"" + us_firstname + " " + us_lastname + "\" <" + project_email + ">"));
                    }
                }
            }

            if (from.Items.Count == 0)
            {
                from.Items.Add(new ListItem("[none]"));
            }

        }

        bool validate()
        {
            bool good = true;

            if (to.Value == "")
            {
                good = false;
                to_err.InnerText = "\"To\" is required.";
            }
            else
            {
                try
                {
                    System.Net.Mail.MailMessage dummy_msg = new System.Net.Mail.MailMessage();
                    Email.add_addresses_to_email(dummy_msg, to.Value, Email.AddrType.to);
                    to_err.InnerText = "";
                }
                catch
                {
                    good = false;
                    to_err.InnerText = "\"To\" is not in a valid format. Separate multiple addresses with commas.";
                }
            }

            if (cc.Value != "")
            {
                try
                {
                    System.Net.Mail.MailMessage dummy_msg = new System.Net.Mail.MailMessage();
                    Email.add_addresses_to_email(dummy_msg, cc.Value, Email.AddrType.cc);
                    cc_err.InnerText = "";
                }
                catch
                {
                    good = false;
                    cc_err.InnerText = "\"CC\" is not in a valid format. Separate multiple addresses with commas.";
                }
            }

            if (from.SelectedItem.Value == "[none]")
            {
                good = false;
                from_err.InnerText = "\"From\" is required.  Use \"settings\" to fix.";
            }
            else
            {
                from_err.InnerText = "";
            }

            if (subject.Value == "")
            {
                good = false;
                subject_err.InnerText = "\"Subject\" is required.";
            }
            else
            {
                subject_err.InnerText = "";
            }

            msg.InnerText = "Email was not sent.";

            return good;
        }


        string get_bug_text(int bugid)
        {
            // Get bug html
            DataRow bug_dr = Bug.get_bug_datarow(bugid, security);

            // Create a fake response and let the code
            // write the html to that response
            System.IO.StringWriter writer = new System.IO.StringWriter();
            HttpResponse my_response = new HttpResponse(writer);
            PrintBug.print_bug(my_response,
                bug_dr,
                security,
                true,  // include style
                false, // images_inline
                true,  // history_inline
                include_internal_posts.Checked); // internal_posts

            return writer.ToString();
        }


        void on_update()
        {

            if (!validate()) return;

            AddEmailComment emailComment = new AddEmailComment();
            emailComment.BugId = int.Parse(bg_id.Value);
            emailComment.UserId = security.user.usid;
            if (security.user.use_fckeditor)
            {
                string adjusted_body = "Subject: " + subject.Value + "<br><br>";
                adjusted_body += Util.strip_dangerous_tags(body.Value);

                emailComment.Comment = adjusted_body.Replace("'", "&#39;");
                emailComment.CommentSearch = adjusted_body.Replace("'", "''");
                emailComment.ContentType = "text/html";
            }
            else
            {
                string adjusted_body = "Subject: " + subject.Value + "\n\n";
                adjusted_body += HttpUtility.HtmlDecode(body.Value);
                adjusted_body = adjusted_body.Replace("'", "''");

                emailComment.Comment = adjusted_body;
                emailComment.CommentSearch = adjusted_body;
                emailComment.ContentType = "text/plain";
            }

            emailComment.EmailFrom = from.SelectedItem.Value.Replace("'", "''");
            emailComment.EmailTo = to.Value.Replace("'", "''");
            emailComment.EmailCC = cc.Value.Replace("'", "''");

            int comment_id = _commentService.AddEmailSentComment(emailComment);

            int[] attachments = handle_attachments(comment_id);

            string body_text;
            BtnetMailFormat format;
            BtnetMailPriority priority;

            switch (prior.SelectedItem.Value)
            {
                case "High":
                    priority = BtnetMailPriority.High;
                    break;
                case "Low":
                    priority = BtnetMailPriority.Low;
                    break;
                default:
                    priority = BtnetMailPriority.Normal;
                    break;
            }

            if (include_bug.Checked)
            {

                // white space isn't handled well, I guess.
                if (security.user.use_fckeditor)
                {
                    body_text = body.Value;
                    body_text += "<br><br>";
                }
                else
                {
                    body_text = body.Value.Replace("\n", "<br>");
                    body_text = body_text.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                    body_text = body_text.Replace("  ", "&nbsp; ");
                }
                body_text += "<hr>" + get_bug_text(Convert.ToInt32(bg_id.Value));

                format = BtnetMailFormat.Html;
            }
            else
            {
                if (security.user.use_fckeditor)
                {
                    body_text = body.Value;
                    format = BtnetMailFormat.Html;
                }
                else
                {
                    body_text = HttpUtility.HtmlDecode(body.Value);
                    format = BtnetMailFormat.Text;
                }
            }

            string result = Email.send_email( // 9 args
                to.Value,
                from.SelectedItem.Value,
                cc.Value,
                subject.Value,
                body_text,
                format,
                priority,
                attachments,
                return_receipt.Checked);

            Bug.send_notifications(btnet.Bug.UPDATE, Convert.ToInt32(bg_id.Value), security);
            WhatsNew.add_news(Convert.ToInt32(bg_id.Value), short_desc.Value, "email sent", security);

            if (result == "")
            {
                Response.Redirect("edit_bug.aspx?id=" + bg_id.Value);
            }
            else
            {
                msg.InnerText = result;
            }

        }


        //This adds attachments to the email.
        //For security and efficency we should remove this.
        int[] handle_attachments(int comment_id)
        {
            ArrayList attachments = new ArrayList();

            string filename = System.IO.Path.GetFileName(attached_file.PostedFile.FileName);
            if (filename != "")
            {
                //add attachment
                int max_upload_size = Convert.ToInt32(Util.get_setting("MaxUploadSize", "100000"));
                int content_length = attached_file.PostedFile.ContentLength;
                if (content_length > max_upload_size)
                {
                    msg.InnerText = "File exceeds maximum allowed length of "
                    + Convert.ToString(max_upload_size)
                    + ".";
                    return null;
                }

                if (content_length == 0)
                {
                    msg.InnerText = "No data was uploaded.";
                    return null;
                }

                int bp_id = Bug.insert_post_attachment(
                    security,
                    Convert.ToInt32(bg_id.Value),
                    attached_file.PostedFile.InputStream,
                    content_length,
                    filename,
                    "email attachment",
                    attached_file.PostedFile.ContentType,
                    comment_id,
                    false, false);

                attachments.Add(bp_id);
            }

            //attachments to forward

            foreach (ListItem item_attachment in lstAttachments.Items)
            {
                if (item_attachment.Selected)
                {
                    int bp_id = Convert.ToInt32(item_attachment.Value);

                    Bug.insert_post_attachment_copy(security, Convert.ToInt32(bg_id.Value), bp_id, "email attachment", comment_id, false, false);
                    attachments.Add(bp_id);
                }
            }

            return (int[])attachments.ToArray(typeof(int));
        }
    }
}
