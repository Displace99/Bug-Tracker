using btnet;
using BugTracker.Web.Models.Query;
using BugTracker.Web.Services;
using BugTracker.Web.Services.Organization;
using BugTracker.Web.Services.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class edit_query : Page
    {
        int id;
        string sql;

        protected Security security;

        private QueryService _queryService = new QueryService();
        private OrganizationService _orgService = new OrganizationService();
        private UserService _userService = new UserService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();

            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            title.InnerText = string.Format("{0} - Edit Query", Util.get_setting("AppTitle", "BugTracker.NET"));

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

                if (security.user.is_admin || security.user.can_edit_sql)
                {
                    // these guys can do everything
                    vis_everybody.Checked = true;

                    DataSet orgListDS = _orgService.GetOrganizationList();
                    DataSet userListDS = _userService.GetUserList();

                    org.DataSource = orgListDS.Tables[0].DefaultView;
                    org.DataTextField = "og_name";
                    org.DataValueField = "og_id";
                    org.DataBind();
                    org.Items.Insert(0, new ListItem("[select org]", "0"));

                    user.DataSource = userListDS.Tables[0].DefaultView;
                    user.DataTextField = "us_username";
                    user.DataValueField = "us_id";
                    user.DataBind();
                    user.Items.Insert(0, new ListItem("[select user]", "0"));
                }
                else
                {
                    sql_text.Visible = false;
                    sql_text_label.Visible = false;
                    explanation.Visible = false;

                    vis_everybody.Enabled = false;
                    vis_org.Enabled = false;
                    vis_user.Checked = true;
                    org.Enabled = false;
                    user.Enabled = false;

                    org.Visible = false;
                    user.Visible = false;
                    vis_everybody.Visible = false;
                    vis_org.Visible = false;
                    vis_user.Visible = false;
                    visibility_label.Visible = false;
                }


                // add or edit?
                if (id == 0)
                {
                    sub.Value = "Create";
                    sql_text.Value = HttpUtility.HtmlDecode(Request.Form["sql_text"]); // if coming from search.aspx

                }
                else
                {
                    sub.Value = "Update";

                    // Get this entry's data from the db and fill in the form
                    DataRow dr = _queryService.GetQueryById(id);

                    if ((int)dr["qu_user"] != security.user.usid)
                    {
                        if (security.user.is_admin || security.user.can_edit_sql)
                        {
                            // these guys can do everything
                        }
                        else
                        {
                            Response.Write("You are not allowed to edit this query");
                            Response.End();
                        }
                    }

                    // Fill in this form
                    desc.Value = (string)dr["qu_desc"];

                    sql_text.Value = (string)dr["qu_sql"];

                    if ((int)dr["qu_user"] == 0 && (int)dr["qu_org"] == 0)
                    {
                        vis_everybody.Checked = true;
                    }
                    else if ((int)dr["qu_user"] != 0)
                    {
                        vis_user.Checked = true;
                        foreach (ListItem li in user.Items)
                        {
                            if (Convert.ToInt32(li.Value) == (int)dr["qu_user"])
                            {
                                li.Selected = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        vis_org.Checked = true;
                        foreach (ListItem li in org.Items)
                        {
                            if (Convert.ToInt32(li.Value) == (int)dr["qu_org"])
                            {
                                li.Selected = true;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                UpdateQuery();
            }

        }

       
        bool ValidateForm()
        {

            bool good = true;

            if (desc.Value == "")
            {
                good = false;
                desc_err.InnerText = "Description is required.";
            }
            else
            {
                desc_err.InnerText = "";
            }


            if (security.user.is_admin || security.user.can_edit_sql)
            {
                if (vis_org.Checked)
                {
                    if (org.SelectedIndex < 1)
                    {
                        good = false;
                        org_err.InnerText = "You must select a org.";
                    }
                    else
                    {
                        org_err.InnerText = "";
                    }
                }
                else if (vis_user.Checked)
                {
                    if (user.SelectedIndex < 1)
                    {
                        good = false;
                        user_err.InnerText = "You must select a user.";
                    }
                    else
                    {
                        user_err.InnerText = "";
                    }
                }
                else
                {
                    org_err.InnerText = "";
                }
            }

            bool isQueryNameUnique = false;

            if (id == 0)
            {
                isQueryNameUnique = _queryService.IsQueryUnique(desc.Value);
            }
            else
            {
                isQueryNameUnique = _queryService.IsQueryUnique(desc.Value, id);
            }

            if (!isQueryNameUnique)
            {
                desc_err.InnerText = "A query with this name already exists.   Choose another name.";
                msg.InnerText = "Query was not created.";
                good = false;
            }

            return good;
        }

        /// <summary>
        /// Saves Query to the Database. Updates existing query, otherwise creates a new one
        /// </summary>
        void UpdateQuery()
        {

            bool good = ValidateForm();

            if (good)
            {
                QueryUpdate query = new QueryUpdate();

                query.Id = id;
                query.Title = desc.Value; ;
                query.SqlText = sql_text.Value;

                if (security.user.is_admin || security.user.can_edit_sql)
                {
                    if (vis_everybody.Checked)
                    {
                        query.SelectedUserId = 0;
                        query.SelectedOrgId = 0;
                    }
                    else if (vis_user.Checked)
                    {
                        query.SelectedUserId = int.Parse(user.SelectedItem.Value);
                        query.SelectedOrgId = 0;
                    }
                    else
                    {
                        query.SelectedUserId = 0;
                        query.SelectedOrgId = int.Parse(org.SelectedItem.Value);
                    }
                }
                else
                {
                    query.SelectedUserId = security.user.usid;
                    query.SelectedOrgId = 0;
                }

                if(id == 0)
                {
                    _queryService.CreateQuery(query);
                }
                else
                {
                    _queryService.UpdateQuery(query);
                }

                Server.Transfer("queries.aspx");
            }
            else
            {
                if (id == 0)  // insert new
                {
                    msg.InnerText = "Query was not created.";
                }
                else // edit existing
                {
                    msg.InnerText = "Query was not updated.";
                }

            }

        }
    }
}
