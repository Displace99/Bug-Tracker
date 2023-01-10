using btnet;
using BugTracker.Web.Services.Priority;
using BugTracker.Web.Services.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class delete_query : Page
    {
        private string sql;
        protected Security security;

        private QueryService _queryService = new QueryService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            int queryId = 0;
            int.TryParse(Request["id"], out queryId);

            if (IsPostBack)
            {
                //Delete query
                _queryService.DeleteQuery(queryId);

                Server.Transfer("queries.aspx");
            }
            else
            {
                title.InnerText = string.Format("{0} - Delete Query", Util.get_setting("AppTitle", "BugTracker.NET"));

                var queryDr = _queryService.GetQueryById(queryId);
                int queryUserId = (int)queryDr["qu_user"];
                string queryDescription = queryDr["qu_desc"].ToString();

                if (queryUserId != security.user.usid)
                {
                    if (security.user.is_admin || security.user.can_edit_sql)
                    {
                        // can do anything
                    }
                    else
                    {
                        Response.Write("You are not allowed to delete this item");
                        Response.End();
                    }
                }

                confirm_href.InnerText = string.Format("Confirm delete of query: {0}", queryDescription);

                row_id.Value = queryId.ToString();

            }
        }
    }
}
