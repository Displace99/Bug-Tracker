using btnet;
using BugTracker.Web.Models.Category;
using BugTracker.Web.Services.Category;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class edit_category : Page
    {
		
		String sql;

		protected Security security;

		private CategoryService _categoryService = new CategoryService();

		void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


		void Page_Load(Object sender, EventArgs e)
		{
			int id;
			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.MUST_BE_ADMIN);

			titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
				+ "edit category";

			msg.InnerText = "";

			string requestId = Request.QueryString["id"];
			
			//If query string is null then it is a New request
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

					DataRow dr = _categoryService.GetCategoryById(id);

					// Fill in this form
					name.Value = (string)dr[0];
					sort_seq.Value = Convert.ToString((int)dr[1]);
					default_selection.Checked = Convert.ToBoolean((int)dr["ct_default"]);
				}
			}
			else
			{
                if (ValidateForm())
                {
					CategoryVM model = new CategoryVM
					{
						Id = id,
						Name = name.Value,
						SortSequence = int.Parse(sort_seq.Value),
						IsDefault = default_selection.Checked
					};

					if(id == 0)
                    {
						_categoryService.CreateCategory(model);
                    }
                    else
                    {
						_categoryService.UpdateCategory(model);
                    }

					Server.Transfer("categories.aspx");
				}
                else
                {
					msg.InnerText = "Please correct errors in form and resubmit";
                }
			}
		}


		/// <summary>
		/// Validates the Form
		/// </summary>
		/// <returns>True if valid, otherwise false</returns>
		private bool ValidateForm()
		{
			bool valid = true;

			//Resets error messages
			name_err.InnerText = "";
			sort_seq_err.InnerText = "";
			sort_seq_err.InnerText = "";

			if (name.Value == "")
			{
				valid = false;
				name_err.InnerText = "Description is required.";
			}

			if (sort_seq.Value == "")
			{
				valid = false;
				sort_seq_err.InnerText = "Sort Sequence is required.";
			}

			if (!Util.is_int(sort_seq.Value))
			{
				valid = false;
				sort_seq_err.InnerText = "Sort Sequence must be an integer.";
			}

			return valid;
		}

	}
}
