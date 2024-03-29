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
    public partial class delete_udf : Page
    {
        private UserDefinedFieldService _udfsService = new UserDefinedFieldService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Master.security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);
            Master.pageLink = "admin";

            int id = 0;
            Int32.TryParse(Request["id"], out id);

            if (IsPostBack)
            {
                //Delete field
                _udfsService.DeleteField(id);
                Server.Transfer("udfs.aspx");
            }
            else
            {
                Page.Title = string.Format("{0} - Delete User Defined Attribute Value", Util.get_setting("AppTitle", "BugTracker.NET"));

                var fieldDR = _udfsService.GetFieldDetails(id);
                string fieldName = fieldDR.IsNull("udf_name") ? string.Empty : fieldDR["udf_name"].ToString();

                bool hasRelatedEntities = _udfsService.DoesFieldHaveRelatedEntities(id);

                if (hasRelatedEntities)
                {
                    Response.Write(string.Format("You can't delete value {0} because some bugs still reference it.", fieldName));
                    Response.End();
                }
                else
                {
                    confirm_href.InnerText = string.Format("Confirm delete of {0}", fieldName);

                    row_id.Value = id.ToString();
                }
            }
        }
    }
}
