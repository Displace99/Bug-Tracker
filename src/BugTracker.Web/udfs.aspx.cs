using btnet;
using BugTracker.Web.Services.UserDefinedFields;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class udfs : Page
    {
        protected DataSet ds;
        protected Security security;

        private UserDefinedFieldService _udfService = new UserDefinedFieldService();

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            title.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "user defined attribute values";

            ds = _udfService.GetFieldList();
        }
    }
}
