using btnet;
using BugTracker.Web.Models.Status;
using BugTracker.Web.Services.Status;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class edit_status : Page
    {
        int id = 0;
        String sql;

        protected Security security;
        private StatusService _statusService = new StatusService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit status";

            msg.InnerText = "";

            Int32.TryParse(Request.QueryString["id"], out id);

            if (!IsPostBack)
            {
                // add or edit?
                if (id == 0)
                {
                    sub.Value = "Create";
                }
                else
                {
                    sub.Value = "Update";

                    // Get this entry's data from the db and fill in the form
                    DataRow dr = _statusService.GetStatusById(id);

                    // Fill in this form
                    name.Value = (string)dr["st_name"];
                    sort_seq.Value = Convert.ToString((int)dr["st_sort_seq"]);
                    style.Value = (string)dr["st_style"];
                    default_selection.Checked = Convert.ToBoolean((int)dr["st_default"]);
                }
            }
            else
            {
                on_update();
            }
        }

        Boolean validate()
        {

            Boolean good = true;
            if (name.Value == "")
            {
                good = false;
                name_err.InnerText = "Description is required.";
            }
            else
            {
                name_err.InnerText = "";
            }

            if (sort_seq.Value == "")
            {
                good = false;
                sort_seq_err.InnerText = "Sort Sequence is required.";
            }
            else
            {
                sort_seq_err.InnerText = "";
            }

            if (!Util.is_int(sort_seq.Value))
            {
                good = false;
                sort_seq_err.InnerText = "Sort Sequence must be an integer.";
            }
            else
            {
                sort_seq_err.InnerText = "";
            }


            return good;
        }

        ///////////////////////////////////////////////////////////////////////
        void on_update()
        {

            Boolean good = validate();

            if (good)
            {
                StatusVM status = new StatusVM
                {
                    Id = id,
                    Name = name.Value,
                    SortSequence = Convert.ToInt32(sort_seq.Value),
                    Style = style.Value,
                    IsDefault = default_selection.Checked
                };

                if (id == 0)  // insert new
                {
                    _statusService.CreateStatus(status);
                }
                else // edit existing
                {
                    _statusService.UpdateStatus(status);
                }
                
                Server.Transfer("statuses.aspx");
            }
            else
            {
                if (id == 0)  // insert new
                {
                    msg.InnerText = "Status was not created.";
                }
                else // edit existing
                {
                    msg.InnerText = "Status was not updated.";
                }

            }

        }
    }
}
