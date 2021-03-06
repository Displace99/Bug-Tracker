using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class queries : Page
    {
		public DataSet ds;

		public Security security;

		protected void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();

			security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

			titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
				+ "queries";

			string sql = "";

			if (security.user.is_admin || security.user.can_edit_sql)
			{
				// allow admin to edit all queries

				sql = @"select
					qu_desc [query],
					case
						when isnull(qu_user,0) = 0 and isnull(qu_org,0) is null then 'everybody'
						when isnull(qu_user,0) <> 0 then 'user:' + us_username
						when isnull(qu_org,0) <> 0 then 'org:' + og_name
						else ' '
						end [visibility],
					'<a href=bugs.aspx?qu_id=' + convert(varchar,qu_id) + '>view list</a>' [view list],
					'<a target=_blank href=print_bugs.aspx?qu_id=' + convert(varchar,qu_id) + '>print list</a>' [print list],
					'<a target=_blank href=print_bugs.aspx?format=excel&qu_id=' + convert(varchar,qu_id) + '>export as excel</a>' [export as excel],
					'<a target=_blank href=print_bugs2.aspx?qu_id=' + convert(varchar,qu_id) + '>print detail</a>' [print list<br>with detail],
					'<a href=edit_query.aspx?id=' + convert(varchar,qu_id) + '>edit</a>' [edit],
					'<a href=delete_query.aspx?id=' + convert(varchar,qu_id) + '>delete</a>' [delete]
					from queries
					left outer join users on qu_user = us_id
					left outer join orgs on qu_org = og_id
					where 1 = $all /* all */
					or isnull(qu_user,0) = $us
					or isnull(qu_user,0) = 0
					order by qu_desc";

				sql = sql.Replace("$all", show_all.Checked ? "1" : "0");
			}
			else
			{
				// allow editing for users' own queries

				sql = @"select
					qu_desc [query],
					'<a href=bugs.aspx?qu_id=' + convert(varchar,qu_id) + '>view list</a>' [view list],
					'<a target=_blank href=print_bugs.aspx?qu_id=' + convert(varchar,qu_id) + '>print list</a>' [print list],
					'<a target=_blank href=print_bugs.aspx?format=excel&qu_id=' + convert(varchar,qu_id) + '>export as excel</a>' [export as excel],
					'<a target=_blank href=print_bugs2.aspx?qu_id=' + convert(varchar,qu_id) + '>print detail</a>' [print list<br>with detail],
					'<a href=edit_query.aspx?id=' + convert(varchar,qu_id) + '>rename</a>' [rename],
					'<a href=delete_query.aspx?id=' + convert(varchar,qu_id) + '>delete</a>' [delete]
					from queries
					inner join users on qu_user = us_id
					where isnull(qu_user,0) = $us
					order by qu_desc";
			}

			sql = sql.Replace("$us", Convert.ToString(security.user.usid));
			ds = btnet.DbUtil.get_dataset(sql);

		}
	}
}
