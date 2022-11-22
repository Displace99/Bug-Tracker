using btnet;
using BugTracker.Web.Models.Priority;
using BugTracker.Web.Services.Priority;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class edit_priority : Page
    {
        private int id;

        protected Security security;

        private PriorityService _priorityService = new PriorityService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit priority";

            msg.InnerText = "";

            string requestId = Request.QueryString["id"];
            if (requestId == null)
            {
                id = 0;
            }
            else
            {
                id = Convert.ToInt32(requestId);
            }

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

                    DataRow dr = _priorityService.GetPriorityById(id);

                    name.Value = (string)dr["pr_name"];
                    sort_seq.Value = Convert.ToString((int)dr["pr_sort_seq"]);
                    color.Value = (string)dr["pr_background_color"];
                    style.Value = (string)dr["pr_style"];
                    default_selection.Checked = Convert.ToBoolean((int)dr["pr_default"]);
                }
            }
            else
            {
                bool isValid = ValidateForm();
                if (isValid)
                {
                    UpdatePriority();
                } 
            }
        }

        /// <summary>
        /// Validates the forms
        /// </summary>
        /// <returns>true if valid, otherwise false</returns>
        private bool ValidateForm()
        {
            bool valid = true;
            if (name.Value == "")
            {
                valid = false;
                name_err.InnerText = "Description is required.";
            }
            else
            {
                name_err.InnerText = "";
            }

            if (sort_seq.Value == "")
            {
                valid = false;
                sort_seq_err.InnerText = "Sort Sequence is required.";
            }
            else
            {
                sort_seq_err.InnerText = "";
            }

            if (!Util.is_int(sort_seq.Value))
            {
                valid = false;
                sort_seq_err.InnerText = "Sort Sequence must be an integer.";
            }
            else
            {
                sort_seq_err.InnerText = "";
            }

            if (color.Value == "")
            {
                valid = false;
                color_err.InnerText = "Background Color in #FFFFFF format is required.";
            }
            else
            {
                color_err.InnerText = "";
            }

            return valid;
        }

        
        void UpdatePriority()
        {
            PriorityVM model = new PriorityVM
            {
                PriorityId = id,
                Name = name.Value,
                SortSequence = sort_seq.Value,
                BackgroundColor = color.Value,
                Style = style.Value,
                IsDefault = default_selection.Checked
            };
            
            if (id == 0)  // insert new
            {
                _priorityService.CreatePriority(model);
            }
            else // edit existing
            {
                _priorityService.UpdatePriority(model);
            }
                
            Server.Transfer("priorities.aspx");
        }
    }
}
