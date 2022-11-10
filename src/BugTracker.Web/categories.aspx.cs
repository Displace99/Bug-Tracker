using btnet;
using BugTracker.Web.Services.Category;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class categories : Page
    {
		protected DataSet ds;
		protected Security security;

		private CategoryService _categoryService = new CategoryService();

		void Page_Load(Object sender, EventArgs e)
		{
			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

			title.InnerText = String.Format("{0} - Categories", Util.get_setting("AppTitle", "BugTracker.NET"));

			ds = _categoryService.GetCategoryList();

		}
	}
}
