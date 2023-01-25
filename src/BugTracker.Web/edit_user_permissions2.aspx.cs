using btnet;
using BugTracker.Web.Services.Project;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BugTracker.Web
{
    public partial class edit_user_permissions2 : Page
    {
        String sql;

        protected Security security;

        private ProjectService _projectService = new ProjectService();

        void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


        ///////////////////////////////////////////////////////////////////////
        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit project per-user permissions";


            if (!IsPostBack)
            {
                int projectId = 0;
                string project_id_string = Util.sanitize_integer(Request["id"]);
                if (!string.IsNullOrEmpty(project_id_string))
                {
                    projectId = Convert.ToInt32(project_id_string);
                }

                if (Request["projects"] != null)
                {
                    back_href.InnerText = "back to projects";
                    back_href.HRef = "projects.aspx";
                }
                else
                {
                    back_href.InnerText = "back to project";
                    back_href.HRef = "edit_project.aspx?id=" + project_id_string;
                }

                DataSet ds = _projectService.GetProjectSettings(projectId);

                MyDataGrid.DataSource = ds.Tables[0].DefaultView;
                MyDataGrid.DataBind();

                titl.InnerText = "Project Permissions";
            }
            else
            {
                on_update();
            }

        }



        ///////////////////////////////////////////////////////////////////////
        void on_update()
        {

            // now update all the recs
            string sql_batch = "";
            RadioButton rb;
            string permission_level;

            foreach (DataGridItem dgi in MyDataGrid.Items)
            {
                sql = @" if exists (select * from project_user_xref where pu_user = $us and pu_project = $pj)
		            update project_user_xref set pu_permission_level = $pu
		            where pu_user = $us and pu_project = $pj
		         else
		            insert into project_user_xref (pu_user, pu_project, pu_permission_level)
		            values ($us, $pj, $pu); ";

                sql = sql.Replace("$pj", Util.sanitize_integer(Request["id"]));
                sql = sql.Replace("$us", Convert.ToString(dgi.Cells[1].Text));

                rb = (RadioButton)dgi.FindControl("none");
                if (rb.Checked)
                {
                    permission_level = "0";
                }
                else
                {
                    rb = (RadioButton)dgi.FindControl("readonly");
                    if (rb.Checked)
                    {
                        permission_level = "1";
                    }
                    else
                    {
                        rb = (RadioButton)dgi.FindControl("reporter");
                        if (rb.Checked)
                        {
                            permission_level = "3";
                        }
                        else
                        {
                            permission_level = "2";
                        }
                    }
                }



                sql = sql.Replace("$pu", permission_level);


                // add to the batch
                sql_batch += sql;

            }

            btnet.DbUtil.execute_nonquery(sql_batch);
            msg.InnerText = "Permissions have been updated.";

        }
    }
}
