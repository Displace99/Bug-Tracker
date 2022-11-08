using btnet;
using BugTracker.Web.Services.Project;
using System;
using System.Data;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class projects : Page
    {
		protected DataSet ds;
		protected Security security;

		private ProjectService _projectService = new ProjectService();

		void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

			title.InnerText = String.Format("{0} - Projects", Util.get_setting("AppTitle", "BugTracker.NET"));

			ds = _projectService.GetAllProjectList();
		}
	}
}
