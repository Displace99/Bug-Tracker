using btnet;
using BugTracker.Web.Services.Query;
using Lucene.Net.Highlight;
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

		protected Security security;
		private QueryService _queryService = new QueryService();

		protected void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();

			security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

			titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
				+ "queries";

			int userId = security.user.usid;
			bool showAll = show_all.Checked;

            if (security.user.is_admin || security.user.can_edit_sql)
			{
				// allow admin to edit all queries
				ds = _queryService.GetAllUsersQueries(showAll, userId);
			}
			else
			{
				// allow editing for users' own queries
				ds = _queryService.GetQueriesByUserId(userId);
            }
		}
	}
}
