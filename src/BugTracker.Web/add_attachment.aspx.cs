using System;
using System.Web;
using System.Web.UI;
using btnet;

namespace BugTracker.Web
{
    public partial class add_attachment : Page
    {
		public int bugid;
		public Security security;

		public void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


		///////////////////////////////////////////////////////////////////////
		public void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);
			security = new Security();
			security.check_security(HttpContext.Current, Security.ANY_USER_OK);

			titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
				+ "add attachment";

			string string_id = Util.sanitize_integer(Request.QueryString["id"]);

			if (string_id == null || string_id == "0")
			{
				write_msg("Invalid id.", false);
				Response.End();
				return;
			}
			else
			{
				bugid = Convert.ToInt32(string_id);
				int permission_level = Bug.get_bug_permission_level(bugid, security);
				if (permission_level == Security.PERMISSION_NONE
				|| permission_level == Security.PERMISSION_READONLY)
				{
					write_msg("You are not allowed to edit this item", false);
					Response.End();
					return;
				}
			}


			if (security.user.external_user || Util.get_setting("EnableInternalOnlyPosts", "0") == "0")
			{
				internal_only.Visible = false;
				internal_only_label.Visible = false;
			}

			if (IsPostBack)
			{
				on_update();
			}
		}


		///////////////////////////////////////////////////////////////////////
		void write_msg(string msg, bool rewrite_posts)
		{
            string script = "script"; // C# compiler doesn't like s c r i p t
            Response.Write("<html><" + script + ">");
            Response.Write("function foo() {");
            Response.Write("parent.set_msg('");
            Response.Write(msg);
            Response.Write("'); ");

            if (rewrite_posts)
            {
                Response.Write("parent.opener.rewrite_posts(" + Convert.ToString(bugid) + ")");
            }
            Response.Write("}</" + script + ">");
            Response.Write("<body onload='foo()'>");
            Response.Write("</body></html>");
            Response.End();
        }

		///////////////////////////////////////////////////////////////////////
		void on_update()
		{

			if (attached_file.PostedFile == null)
			{
				write_msg("Please select file", false);
				return;
			}

			string filename = System.IO.Path.GetFileName(attached_file.PostedFile.FileName);
			if (string.IsNullOrEmpty(filename))
			{
				write_msg("Please select file", false);
				return;
			}

			int max_upload_size = Convert.ToInt32(Util.get_setting("MaxUploadSize", "100000"));
			int content_length = attached_file.PostedFile.ContentLength;
			if (content_length > max_upload_size)
			{
				write_msg("File exceeds maximum allowed length of "
					+ Convert.ToString(max_upload_size)
					+ ".", false);
				return;
			}

			if (content_length == 0)
			{
				write_msg("No data was uploaded.", false);
				return;
			}

			try
			{
				Bug.insert_post_attachment(
					security,
					bugid,
					attached_file.PostedFile.InputStream,
					content_length,
					filename,
					desc.Value,
					attached_file.PostedFile.ContentType,
					-1, // parent
					internal_only.Checked,
					true);

				Response.Redirect("edit_bug.aspx?id=" + Convert.ToString(bugid));

			}
			catch (Exception ex)
			{
				//TODO: We shouldn't write this exception out as it's a security risk.
				//Should log it and provide a generic error message.
				write_msg("caught exception:" + ex.Message, false);
				return;
			}

		}
	}
}
