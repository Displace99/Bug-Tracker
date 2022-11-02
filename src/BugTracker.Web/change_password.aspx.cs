using btnet;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class change_password : Page
    {
		void Page_Load(Object sender, EventArgs e)
		{

			Util.set_context(HttpContext.Current);
			Util.do_not_cache(Response);

			if (!IsPostBack)
			{
				titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
					+ "change password";
			}
			else
			{
				msg.InnerHtml = "";

				if (string.IsNullOrEmpty(password.Value))
				{
					msg.InnerHtml = "Enter your password twice.";
				}
				else if (password.Value != confirm.Value)
				{
					msg.InnerHtml = "Re-entered password doesn't match password.";
				}
				else if (!Util.check_password_strength(password.Value))
				{
					msg.InnerHtml = "Password is not difficult enough to guess.";
					msg.InnerHtml += "<br>Avoid common words.";
					msg.InnerHtml += "<br>Try using a mixture of lowercase, uppercase, digits, and special characters.";
				}
				else
				{


					string guid = Request["id"];

					if (string.IsNullOrEmpty(guid))
					{
						msg.InnerHtml = "The change password link is invalid. Please request a new Password Reset.";
						return;
					}

					StringBuilder sqlText = new StringBuilder();
					sqlText.AppendLine("declare @expiration datetime");
					sqlText.AppendLine("set @expiration = dateadd(n,-@minutes,getdate())");
					sqlText.AppendLine("select *, case when el_date < @expiration then 1 else 0 end[expired] from emailed_links	where el_id = @linkId");
					
					//TODO: This should be a separate process. Nightly job or something. Or we delete the row upon successful completion of the password reset.
					sqlText.AppendLine("delete from emailed_links where el_date < dateadd(n,-240,getdate())");

					int expirationMinutes = Int32.Parse(Util.get_setting("RegistrationExpiration", "20"));

					SqlCommand cmd = new SqlCommand();
					cmd.CommandText = sqlText.ToString();
					cmd.Parameters.AddWithValue("@minutes", expirationMinutes);
					cmd.Parameters.AddWithValue("@linkId", guid);

					DataRow dr = DbUtil.get_datarow(cmd);

					// DataRow dr = btnet.DbUtil.get_datarow(sql);

					if (dr == null)
					{
						msg.InnerHtml = "The link you clicked on is expired or invalid.<br>Please start over again.";
					}
					else if ((int)dr["expired"] == 1)
					{
						msg.InnerHtml = "The link you clicked has expired.<br>Please start over again.";
					}
					else
					{
						Util.UpdateUserPassword((int)dr["el_user_id"], password.Value);
						msg.InnerHtml = "Your password has been changed.";
					}

				}
			}
		}
	}
}
