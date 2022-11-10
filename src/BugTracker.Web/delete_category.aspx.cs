using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_category : Page
    {
		String sql;

		protected Security security;

		void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

		///////////////////////////////////////////////////////////////////////
		protected void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

			if (IsPostBack)
			{
				sql = @"delete categories where ct_id = $1";
				sql = sql.Replace("$1", Util.sanitize_integer(row_id.Value));
				btnet.DbUtil.execute_nonquery(sql);
				Server.Transfer("categories.aspx");
			}
			else
			{
				titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
					+ "delete category";

				string id = Util.sanitize_integer(Request["id"]);

				sql = @"declare @cnt int
			select @cnt = count(1) from bugs where bg_category = $1
			select ct_name, @cnt [cnt] from categories where ct_id = $1";
				sql = sql.Replace("$1", id);

				DataRow dr = btnet.DbUtil.get_datarow(sql);

				if ((int)dr["cnt"] > 0)
				{
					Response.Write("You can't delete category \""
						+ Convert.ToString(dr["ct_name"])
						+ "\" because some bugs still reference it.");
					Response.End();
				}
				else
				{
					confirm_href.InnerText = "confirm delete of \""
						+ Convert.ToString(dr["ct_name"])
						+ "\"";

					row_id.Value = id;
				}
			}

		}
	}
}
