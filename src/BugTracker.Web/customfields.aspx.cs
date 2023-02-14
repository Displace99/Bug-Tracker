using btnet;
using BugTracker.Web.Services.CustomFields;
using System;
using System.Data;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class customfields : Page
    {
        protected DataSet ds;

        protected Security security;
        private CustomFieldService _customFieldService = new CustomFieldService(HttpContext.Current);

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "custom fields";

            ds = _customFieldService.GetCustomFields();
        }
    }
}
