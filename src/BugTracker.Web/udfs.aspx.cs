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

            //Master.security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);
            //Master.pageLink = "admin";
            LoggedIn masterPage = Page.Master as LoggedIn;
            masterPage.security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);
            masterPage.pageLink = "admin";

            Page.Title = string.Format("{0} - User Defined Attribute Values", Util.get_setting("AppTitle", "BugTracker.NET"));

            ds = _udfService.GetFieldList();
        }
    }
}
