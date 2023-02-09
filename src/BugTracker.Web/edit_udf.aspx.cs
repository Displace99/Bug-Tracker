using btnet;
using BugTracker.Web.Models.UDF;
using BugTracker.Web.Services.UserDefinedFields;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class edit_udf : Page
    {
        int id = 0;
        String sql;

        protected Security security;

        private UserDefinedFieldService _udfsService = new UserDefinedFieldService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit user defined attribute value";

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

                    DataRow dr = _udfsService.GetFieldDetails(id);

                    // Fill in this form
                    name.Value = (string)dr["udf_name"];
                    sort_seq.Value = Convert.ToString((int)dr["udf_sort_seq"]);
                    default_selection.Checked = Convert.ToBoolean((int)dr["udf_default"]);
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
                UserDefinedField userField = new UserDefinedField
                {
                    Id = id,
                    Name = name.Value,
                    SortSequence = Convert.ToInt32(sort_seq.Value),
                    IsDefault = default_selection.Checked
                };

                if (id == 0)  // insert new
                {
                    _udfsService.CreateField(userField);
                }
                else // edit existing
                {

                    _udfsService.UpdateField(userField);

                }
                
                Server.Transfer("udfs.aspx");
            }
            else
            {
                if (id == 0)  // insert new
                {
                    msg.InnerText = "User defined attribute value was not created.";
                }
                else // edit existing
                {
                    msg.InnerText = "User defined attribute value was not updated.";
                }
            }
        }
    }
}
