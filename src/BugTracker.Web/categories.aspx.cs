using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class categories : Page
    {
		protected DataSet ds;

		protected Security security;

		void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

			titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
				+ "categories";

			ds = btnet.DbUtil.get_dataset(
				@"select
		ct_id [id],
		ct_name [category],
		ct_sort_seq [sort seq],
		case when ct_default = 1 then 'Y' else 'N' end [default],
		ct_id [hidden]
		from categories order by ct_name");

		}
	}
}
