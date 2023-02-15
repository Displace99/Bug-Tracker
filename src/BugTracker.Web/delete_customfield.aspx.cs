using btnet;
using BugTracker.Web.Services.CustomFields;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_customfield : Page
    {
        String sql;

        protected Security security;
        private CustomFieldService _customFieldService = new CustomFieldService(HttpContext.Current);

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "delete custom field";

            int id = 0;
            Int32.TryParse(Request["id"], out id);

            if (IsPostBack)
            {
                //Delete field
                _customFieldService.DeleteField(id);

                Response.Redirect("customfields.aspx");
            }
            else
            {
                //string id = Util.sanitize_integer(Request["id"]);

                sql = @"select sc.name
			from syscolumns sc
			inner join sysobjects so on sc.id = so.id
			left outer join sysobjects df on df.id = sc.cdefault
			where so.name = 'bugs'
			and sc.colorder = $id";

                sql = sql.Replace("$id", id.ToString());
                DataRow dr = btnet.DbUtil.get_datarow(sql);

                confirm_href.InnerText = "confirm delete of \""
                    + Convert.ToString(dr["name"])
                    + "\"";

                row_id.Value = id.ToString();
            }

        }
    }
}
