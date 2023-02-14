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

        private CustomFieldService _customFieldService = new CustomFieldService(HttpContext.Current);

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            Master.security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);
            Master.pageLink = "admin";

            Page.Title = string.Format("{0} - Custom Fields", Util.get_setting("AppTitle", "BugTracker.NET"));

            ds = _customFieldService.GetCustomFields();
        }
    }
}
