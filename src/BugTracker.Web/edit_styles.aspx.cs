using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class edit_styles : Page
    {
        protected DataSet ds;

        protected Security security;

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            ds = btnet.DbUtil.get_dataset(
                @"select
			'<a target=_blank href=edit_priority.aspx?id=' + convert(varchar,pr_id) + '>' + pr_name + '</a>' [priority],
			'<a target=_blank href=edit_status.aspx?id=' + convert(varchar,st_id) + '>' + st_name + '</a>' [status],
			isnull(pr_style,'') [priority CSS class],
			isnull(st_style,'') [status CSS class],
			isnull(pr_style + st_style,'datad') [combo CSS class - priority + status ],
			'<span class=''' + isnull(pr_style,'') + isnull(st_style,'')  +'''>The quick brown fox</span>' [text sample]
			from priorities, statuses /* intentioanl cartesian join */
			order by pr_sort_seq, st_sort_seq;

			select distinct isnull(pr_style + st_style,'datad')
			from priorities, statuses;");

        }
    }
}
