using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class complete_registration : Page
    {
		void Page_Load(Object sender, EventArgs e)
		{

			Util.set_context(HttpContext.Current);
			Util.do_not_cache(Response);



			string guid = Request["id"];

			string sql = @"
declare @expiration datetime
set @expiration = dateadd(n,-$minutes,getdate())

select *,
	case when el_date < @expiration then 1 else 0 end [expired]
	from emailed_links
	where el_id = '$guid'

delete from emailed_links
	where el_date < dateadd(n,-240,getdate())";

			sql = sql.Replace("$minutes", Util.get_setting("RegistrationExpiration", "20"));
			sql = sql.Replace("$guid", guid.Replace("'", "''"));

			DataRow dr = btnet.DbUtil.get_datarow(sql);

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
				btnet.User.copy_user(
					(string)dr["el_username"],
					(string)dr["el_email"],
					(string)dr["el_firstname"],
					(string)dr["el_lastname"],
					"",
					(string)dr["el_salt"],
					(string)dr["el_password"],
					Util.get_setting("SelfRegisteredUserTemplate", "[error - missing user template]"),
					false);

				//  Delete the temp link
				sql = @"delete from emailed_links where el_id = '$guid'";
				sql = sql.Replace("$guid", guid.Replace("'", "''"));
				btnet.DbUtil.execute_nonquery(sql);

				msg.InnerHtml = "Your registration is complete.";
			}

		}
	}
}
