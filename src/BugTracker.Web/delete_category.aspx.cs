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
    public partial class delete_category : Page
    {
		protected Security security;
		private CategoryService _categoryService = new CategoryService();

		void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

		protected void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

			int categoryId = 0;
			int.TryParse(Request["id"], out categoryId);

			if (IsPostBack)
			{
				_categoryService.DeleteCategory(categoryId);
				Server.Transfer("categories.aspx");
			}
			else
			{
				titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
					+ "delete category";
				
				if (categoryId == 0)
				{
					Response.Write("Category Id not found");
					Response.End();
				}

				int bugCount = _categoryService.GetBugCountByCategory(categoryId);
				var categoryDr = _categoryService.GetCategoryById(categoryId);
				string categoryName = (string)categoryDr[0];

				if (bugCount > 0)
				{
					Response.Write(string.Format("You can't delete category {0} because some bugs still reference it", categoryName));
					Response.End();
				}
				else
				{
					Response.Write(string.Format("Confirm deletion of {0}", categoryName));
					
					//sets a hidden field with category Id
					row_id.Value = categoryId.ToString();
				}
			}

		}
	}
}
