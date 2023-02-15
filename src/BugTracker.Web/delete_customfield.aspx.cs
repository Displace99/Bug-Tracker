using btnet;
using BugTracker.Web.Services.CustomFields;
using System;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_customfield : Page
    {
        protected Security security;
        private CustomFieldService _customFieldService = new CustomFieldService(HttpContext.Current);

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            //security = new Security();
            Master.security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);
            Master.pageLink = "admin";

            Page.Title = string.Format("{0} - Delete Custom Field", Util.get_setting("AppTitle", "BugTracker.NET"));

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
                string columnName = _customFieldService.GetColumnName(id);

                confirm_href.InnerText = string.Format("confirm delete of {0}", columnName);

                row_id.Value = id.ToString();
            }
        }
    }
}
