using btnet;
using BugTracker.Web.Models.CustomFields;
using BugTracker.Web.Services.CustomFields;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class edit_customfield : Page
    {
        int id;
        String sql;

        protected Security security;
        private CustomFieldService _customFieldService = new CustomFieldService(HttpContext.Current);

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit custom column metadata";

            msg.InnerText = "";

            id = Convert.ToInt32(Util.sanitize_integer(Request["id"]));

            if (!IsPostBack)
            {
                DataRow dr = _customFieldService.GetFieldDetails(id);
                
                name.InnerText = (string)dr["name"];
                dropdown_type.Value = Convert.ToString(dr["dropdown_type"]);

                if (dropdown_type.Value == "normal")
                {
                    // show the dropdown vals
                }
                else
                {
                    vals.Visible = false;
                    vals_label.Visible = false;
                }

                // Fill in this form
                vals.Value = (string)dr["vals"];
                sort_seq.Value = Convert.ToString(dr["column order"]);
                default_value.Value = Convert.ToString(dr["default value"]);
                hidden_default_value.Value = default_value.Value; // to test if it changed
                hidden_default_name.Value = Convert.ToString(dr["default name"]);
            }
            else
            {
                UpdateField();
            }
        }

        bool ValidateForm()
        {
            bool good = true;

            sort_seq_err.InnerText = "";
            vals_err.InnerText = "";

            if (sort_seq.Value == "")
            {
                good = false;
                sort_seq_err.InnerText = "Sort Sequence is required.";
            }

            if (!Util.is_int(sort_seq.Value))
            {
                good = false;
                sort_seq_err.InnerText = "Sort Sequence must be an integer.";
            }

            if (dropdown_type.Value == "normal")
            {
                if (vals.Value == "")
                {
                    good = false;
                    vals_err.InnerText = "Dropdown values are required for dropdown type of \"normal\".";
                }
                else
                {
                    string vals_error_string = btnet.Util.validate_dropdown_values(vals.Value);
                    if (!string.IsNullOrEmpty(vals_error_string))
                    {
                        good = false;
                        vals_err.InnerText = vals_error_string;
                    }
                }
            }

            return good;
        }

        void UpdateField()
        {
            bool isValid = ValidateForm();

            if (isValid)
            {

                sql = @"declare @count int
                select @count = count(1) from custom_col_metadata
                where ccm_colorder = $co

                if @count = 0
                	insert into custom_col_metadata
                	(ccm_colorder, ccm_dropdown_vals, ccm_sort_seq, ccm_dropdown_type)
                	values($co, N'$v', $ss, '$dt')
                else
                	update custom_col_metadata
                	set ccm_dropdown_vals = N'$v',
                	ccm_sort_seq = $ss
                	where ccm_colorder = $co";

                sql = sql.Replace("$co", Convert.ToString(id));
                sql = sql.Replace("$v", vals.Value.Replace("'", "''"));
                sql = sql.Replace("$ss", sort_seq.Value);

                btnet.DbUtil.execute_nonquery(sql);
                Application["custom_columns_dataset"] = null;

                if (default_value.Value != hidden_default_value.Value)
                {
                    if (hidden_default_name.Value != "")
                    {
                        sql = "alter table bugs drop constraint [" + hidden_default_name.Value.Replace("'", "''") + "]";
                        btnet.DbUtil.execute_nonquery(sql);
                        Application["custom_columns_dataset"] = null;
                    }

                    if (default_value.Value != "")
                    {
                        sql = "alter table bugs add constraint [" + System.Guid.NewGuid().ToString() + "] default " + default_value.Value.Replace("'", "''") + " for [" + name.InnerText + "]";
                        btnet.DbUtil.execute_nonquery(sql);
                        Application["custom_columns_dataset"] = null;
                    }
                }
                CustomField customField = new CustomField 
                {
                    Id = id,
                    Name = name.InnerText,
                    DropdownValues = vals.Value,
                    SortSequence = Convert.ToInt32(sort_seq.Value),
                    DefaultValue = default_value.Value,
                    OldDefaultValue = hidden_default_value.Value,
                    DefaultName = hidden_default_name.Value
                };


                Server.Transfer("customfields.aspx");
            }
            else
            {
                msg.InnerText = "dropdown values were not updated.";
            }
        }
    }
}
