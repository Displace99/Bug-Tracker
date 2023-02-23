using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class edit_project : Page
    {
        protected int id;
        String sql;

        protected Security security;

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit project";

            msg.InnerText = "";

            string var = Request.QueryString["id"];
            if (var == null)
            {
                id = 0;
            }
            else
            {
                id = Convert.ToInt32(var);
            }

            if (!IsPostBack)
            {

                default_user.DataSource =
                    btnet.DbUtil.get_dataview("select us_id, us_username from users order by us_username");
                default_user.DataTextField = "us_username";
                default_user.DataValueField = "us_id";
                default_user.DataBind();
                default_user.Items.Insert(0, new ListItem("", "0"));

                // add or edit?
                if (id == 0)
                {
                    sub.Value = "Create";
                    active.Checked = true;
                }
                else
                {
                    sub.Value = "Update";

                    // Get this entry's data from the db and fill in the form

                    sql = @"select
			pj_name,
			pj_active,
			isnull(pj_default_user,0) [pj_default_user],
			pj_default,
			isnull(pj_auto_assign_default_user,0) [pj_auto_assign_default_user],
			isnull(pj_auto_subscribe_default_user,0) [pj_auto_subscribe_default_user],
			isnull(pj_enable_pop3,0) [pj_enable_pop3],
			isnull(pj_pop3_username,'') [pj_pop3_username],
			isnull(pj_pop3_email_from,'') [pj_pop3_email_from],
			isnull(pj_description,'') [pj_description],
			isnull(pj_enable_custom_dropdown1,0) [pj_enable_custom_dropdown1],
			isnull(pj_enable_custom_dropdown2,0) [pj_enable_custom_dropdown2],
			isnull(pj_enable_custom_dropdown3,0) [pj_enable_custom_dropdown3],
			isnull(pj_custom_dropdown_label1,'') [pj_custom_dropdown_label1],
			isnull(pj_custom_dropdown_label2,'') [pj_custom_dropdown_label2],
			isnull(pj_custom_dropdown_label3,'') [pj_custom_dropdown_label3],
			isnull(pj_custom_dropdown_values1,'') [pj_custom_dropdown_values1],
			isnull(pj_custom_dropdown_values2,'') [pj_custom_dropdown_values2],
			isnull(pj_custom_dropdown_values3,'') [pj_custom_dropdown_values3]
			from projects
			where pj_id = $1";
                    sql = sql.Replace("$1", Convert.ToString(id));
                    DataRow dr = btnet.DbUtil.get_datarow(sql);

                    // Fill in this form
                    name.Value = (string)dr["pj_name"];
                    active.Checked = Convert.ToBoolean((int)dr["pj_active"]);
                    auto_assign.Checked = Convert.ToBoolean((int)dr["pj_auto_assign_default_user"]);
                    auto_subscribe.Checked = Convert.ToBoolean((int)dr["pj_auto_subscribe_default_user"]);
                    default_selection.Checked = Convert.ToBoolean((int)dr["pj_default"]);
                    enable_pop3.Checked = Convert.ToBoolean((int)dr["pj_enable_pop3"]);
                    pop3_username.Value = (string)dr["pj_pop3_username"];
                    pop3_email_from.Value = (string)dr["pj_pop3_email_from"];

                    enable_custom_dropdown1.Checked = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown1"]);
                    enable_custom_dropdown2.Checked = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown2"]);
                    enable_custom_dropdown3.Checked = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown3"]);

                    custom_dropdown_label1.Value = (string)dr["pj_custom_dropdown_label1"];
                    custom_dropdown_label2.Value = (string)dr["pj_custom_dropdown_label2"];
                    custom_dropdown_label3.Value = (string)dr["pj_custom_dropdown_label3"];

                    custom_dropdown_values1.Value = (string)dr["pj_custom_dropdown_values1"];
                    custom_dropdown_values2.Value = (string)dr["pj_custom_dropdown_values2"];
                    custom_dropdown_values3.Value = (string)dr["pj_custom_dropdown_values3"];

                    desc.Value = (string)dr["pj_description"];

                    foreach (ListItem li in default_user.Items)
                    {
                        if (Convert.ToInt32(li.Value) == (int)dr["pj_default_user"])
                        {
                            li.Selected = true;
                            break;
                        }
                    }

                    permissions_href.HRef = "edit_user_permissions2.aspx?id=" + Convert.ToString(id)
                        + "&label=" + HttpUtility.UrlEncode(name.Value);

                }
            }
            else
            {
                on_update();
            }
        }


        bool ValidateForm()
        {
            bool good = true;

            //Clear error messages 
            name_err.InnerText = "";
            custom_dropdown_values1_err.InnerText = "";
            custom_dropdown_values2_err.InnerText = "";
            custom_dropdown_values3_err.InnerText = "";

            if (name.Value == "")
            {
                good = false;
                name_err.InnerText = "Description is required.";
            }

            string vals_error_string = "";
            bool errors_with_custom_dropdowns = false;
            vals_error_string = btnet.Util.validate_dropdown_values(custom_dropdown_values1.Value);
            if (!string.IsNullOrEmpty(vals_error_string))
            {
                good = false;
                custom_dropdown_values1_err.InnerText = vals_error_string;
                errors_with_custom_dropdowns = true;
            }

            vals_error_string = btnet.Util.validate_dropdown_values(custom_dropdown_values2.Value);
            if (!string.IsNullOrEmpty(vals_error_string))
            {
                good = false;
                custom_dropdown_values2_err.InnerText = vals_error_string;
                errors_with_custom_dropdowns = true;
            }

            vals_error_string = btnet.Util.validate_dropdown_values(custom_dropdown_values3.Value);
            if (!string.IsNullOrEmpty(vals_error_string))
            {
                good = false;
                custom_dropdown_values3_err.InnerText = vals_error_string;
                errors_with_custom_dropdowns = true;
            }

            if (errors_with_custom_dropdowns)
            {
                msg.InnerText += "Custom fields have errors.  ";
            }

            return good;
        }


        void on_update()
        {
            bool isValid = ValidateForm();

            if (isValid)
            {
                if (id == 0)  // insert new
                {
                    sql = @"insert into projects
			(pj_name, pj_active, pj_default_user, pj_default, pj_auto_assign_default_user, pj_auto_subscribe_default_user,
			pj_enable_pop3,
			pj_pop3_username,
			pj_pop3_password,
			pj_pop3_email_from,
			pj_description,
			pj_enable_custom_dropdown1,
			pj_enable_custom_dropdown2,
			pj_enable_custom_dropdown3,
			pj_custom_dropdown_label1,
			pj_custom_dropdown_label2,
			pj_custom_dropdown_label3,
			pj_custom_dropdown_values1,
			pj_custom_dropdown_values2,
			pj_custom_dropdown_values3)
			values (N'$name', $active, $defaultuser, $defaultsel, $autoasg, $autosub,
			$enablepop, N'$popuser',N'$poppass',N'$popfrom',
			N'$desc', 
			$ecd1,$ecd2,$ecd3,
			N'$cdl1',N'$cdl2',N'$cdl3',
			N'$cdv1',N'$cdv2',N'$cdv3')";

                    sql = sql.Replace("$poppass", pop3_password.Value.Replace("'", "''"));

                }
                else // edit existing
                {

                    sql = @"update projects set
				pj_name = N'$name',
				$POP3_PASSWORD
				pj_active = $active,
				pj_default_user = $defaultuser,
				pj_default = $defaultsel,
				pj_auto_assign_default_user = $autoasg,
				pj_auto_subscribe_default_user = $autosub,
				pj_enable_pop3 = $enablepop,
				pj_pop3_username = N'$popuser',
				pj_pop3_email_from = N'$popfrom',
				pj_description = N'$desc',
				pj_enable_custom_dropdown1 = $ecd1,
				pj_enable_custom_dropdown2 = $ecd2,
				pj_enable_custom_dropdown3 = $ecd3,
				pj_custom_dropdown_label1 = N'$cdl1',
				pj_custom_dropdown_label2 = N'$cdl2',
				pj_custom_dropdown_label3 = N'$cdl3',
				pj_custom_dropdown_values1 = N'$cdv1',
				pj_custom_dropdown_values2 = N'$cdv2',
				pj_custom_dropdown_values3 = N'$cdv3'
				where pj_id = $id";
                    sql = sql.Replace("$id", Convert.ToString(id));

                    if (pop3_password.Value != "")
                    {
                        sql = sql.Replace("$POP3_PASSWORD", "pj_pop3_password = N'" + pop3_password.Value.Replace("'", "''") + "',");
                    }
                    else
                    {
                        sql = sql.Replace("$POP3_PASSWORD", "");
                    }

                }

                sql = sql.Replace("$name", name.Value.Replace("'", "''"));
                sql = sql.Replace("$active", Util.bool_to_string(active.Checked));
                sql = sql.Replace("$defaultuser", default_user.SelectedItem.Value);
                sql = sql.Replace("$autoasg", Util.bool_to_string(auto_assign.Checked));
                sql = sql.Replace("$autosub", Util.bool_to_string(auto_subscribe.Checked));
                sql = sql.Replace("$defaultsel", Util.bool_to_string(default_selection.Checked));
                sql = sql.Replace("$enablepop", Util.bool_to_string(enable_pop3.Checked));
                sql = sql.Replace("$popuser", pop3_username.Value.Replace("'", "''"));
                sql = sql.Replace("$popfrom", pop3_email_from.Value.Replace("'", "''"));

                sql = sql.Replace("$desc", desc.Value.Replace("'", "''"));

                sql = sql.Replace("$ecd1", Util.bool_to_string(enable_custom_dropdown1.Checked));
                sql = sql.Replace("$ecd2", Util.bool_to_string(enable_custom_dropdown2.Checked));
                sql = sql.Replace("$ecd3", Util.bool_to_string(enable_custom_dropdown3.Checked));


                sql = sql.Replace("$cdl1", custom_dropdown_label1.Value.Replace("'", "''"));
                sql = sql.Replace("$cdl2", custom_dropdown_label2.Value.Replace("'", "''"));
                sql = sql.Replace("$cdl3", custom_dropdown_label3.Value.Replace("'", "''"));

                sql = sql.Replace("$cdv1", custom_dropdown_values1.Value.Replace("'", "''"));
                sql = sql.Replace("$cdv2", custom_dropdown_values2.Value.Replace("'", "''"));
                sql = sql.Replace("$cdv3", custom_dropdown_values3.Value.Replace("'", "''"));

                btnet.DbUtil.execute_nonquery(sql);
                Server.Transfer("projects.aspx");

            }
            else
            {
                if (id == 0)  // insert new
                {
                    msg.InnerText += "Project was not created.";
                }
                else // edit existing
                {
                    msg.InnerText += "Project was not updated.";
                }
            }
        }
    }
}
