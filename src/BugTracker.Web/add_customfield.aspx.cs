using btnet;
using BugTracker.Web.Models.CustomFields;
using BugTracker.Web.Services.CustomFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class add_customfield : Page
    {
        private CustomFieldService _customFieldService = new CustomFieldService(HttpContext.Current);

        void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            Master.security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);
            Master.pageLink = "admin";

            msg.InnerText = "";

            if (!IsPostBack)
            {
                datatype.Items.Insert(0, new ListItem("char", "char"));
                datatype.Items.Insert(0, new ListItem("datetime", "datetime"));
                datatype.Items.Insert(0, new ListItem("decimal", "decimal"));
                datatype.Items.Insert(0, new ListItem("int", "int"));
                datatype.Items.Insert(0, new ListItem("nchar", "nchar"));
                datatype.Items.Insert(0, new ListItem("nvarchar", "nvarchar"));
                datatype.Items.Insert(0, new ListItem("varchar", "varchar"));

                dropdown_type.Items.Insert(0, new ListItem("not a dropdown", ""));
                dropdown_type.Items.Insert(1, new ListItem("normal", "normal"));
                dropdown_type.Items.Insert(2, new ListItem("users", "users"));

                sort_seq.Value = "1";
            }
            else
            {
                UpdateField();
            }

        }


        bool ValidateForm()
        {
            //Clear error messages
            name_err.InnerText = "";
            length_err.InnerText = "";
            sort_seq_err.InnerText = "";
            default_err.InnerText = "";
            vals_err.InnerText = "";
            datatype_err.InnerText = "";
            required_err.InnerText = "";

            bool good = true;

            if (string.IsNullOrEmpty(name.Value))
            {
                good = false;
                name_err.InnerText = "Field name is required.";
            }
            else
            {
                if (name.Value.ToLower() == "url")
                {
                    good = false;
                    name_err.InnerText = "Field name of \"URL\" causes problems with ASP.NET.";
                }
                else if (name.Value.Contains("'")
                || name.Value.Contains("\\")
                || name.Value.Contains("/")
                || name.Value.Contains("\"")
                || name.Value.Contains("<")
                || name.Value.Contains(">"))
                {
                    good = false;
                    name_err.InnerText = "Some special characters like quotes, slashes are not allowed.";
                }
            }

            if (string.IsNullOrEmpty(length.Value))
            {
                if (datatype.SelectedItem.Value == "int"
                || datatype.SelectedItem.Value == "datetime")
                {
                    // ok
                }
                else
                {
                    good = false;
                    length_err.InnerText = "Length or Precision is required for this datatype.";
                }
            }
            else
            {
                if (datatype.SelectedItem.Value == "int"
                || datatype.SelectedItem.Value == "datetime")
                {
                    good = false;
                    length_err.InnerText = "Length or Precision not allowed for this datatype.";
                }
            }

            if (required.Checked)
            {
                if (string.IsNullOrEmpty(default_text.Value))
                {
                    good = false;
                    default_err.InnerText = "If \"Required\" is checked, then Default is required.";
                }

                if (dropdown_type.SelectedItem.Value != "")
                {
                    good = false;
                    required_err.InnerText = "Checking \"Required\" is not compatible with a normal or users dropdown";
                }

            }

            if (dropdown_type.SelectedItem.Value == "normal")
            {
                if (string.IsNullOrEmpty(vals.Value))
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
                    else
                    {
                        if (datatype.SelectedItem.Value == "int"
                        || datatype.SelectedItem.Value == "decimal"
                        || datatype.SelectedItem.Value == "datetime")
                        {
                            good = false;
                            datatype_err.InnerText = "For a normal dropdown datatype must be char, varchar, nchar, or nvarchar.";
                        }
                    }
                }
            }
            else if (dropdown_type.SelectedItem.Value == "users")
            {
                if (datatype.SelectedItem.Value != "int")
                {
                    good = false;
                    datatype_err.InnerText = "For a users dropdown datatype must be int.";
                }
            }

            if (dropdown_type.SelectedItem.Value != "normal")
            {
                if (vals.Value != "")
                {
                    good = false;
                    vals_err.InnerText = "Dropdown values are only used for dropdown of type \"normal\".";
                }
            }

            if (string.IsNullOrEmpty(sort_seq.Value))
            {
                good = false;
                sort_seq_err.InnerText = "Sort Sequence is required.";
            }
            else
            {
                if (!Util.is_int(sort_seq.Value))
                {
                    good = false;
                    sort_seq_err.InnerText = "Sort Sequence must be an integer.";
                }
            }

            return good;
        }


        void UpdateField()
        {
            bool isValid = ValidateForm();

            if (isValid)
            {
                NewCustomFieldVM customFieldVM= new NewCustomFieldVM 
                {
                    Name = name.Value,
                    DataType = datatype.SelectedItem.Value,
                    FieldLength = length.Value,
                    DefaultValue = default_text.Value,
                    IsRequired = required.Checked,
                    DropDownValues = vals.Value,
                    SortSequence = Convert.ToInt32(sort_seq.Value),
                    DropDownType = dropdown_type.SelectedItem.Value
                };

                bool fieldAdded = _customFieldService.AddCustomField(customFieldVM);

                if (fieldAdded)
                {
                    Server.Transfer("customfields.aspx");
                }
                else
                {
                    msg.InnerText = "There was an error creating the custom field";
                }
            }
            else
            {
                msg.InnerText = "Custom field was not created.";
            }
        }
    }
}
