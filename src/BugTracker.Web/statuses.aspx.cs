using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class statuses : Page
    {
        protected DataSet ds;

        protected Security security;

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);


            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "statuses";

            ds = btnet.DbUtil.get_dataset(
                @"select st_id [id],
		st_name [status],
		st_sort_seq [sort seq],
		st_style [css<br>class],
		case when st_default = 1 then 'Y' else 'N' end [default],
		st_id [hidden]
		from statuses order by st_sort_seq");

        }

    }
}
