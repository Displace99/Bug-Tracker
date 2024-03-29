using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using btnet;
using BugTracker.Web.Models.Task;
using BugTracker.Web.Services;
using BugTracker.Web.Services.Bug;

namespace BugTracker.Web
{
    public partial class edit_task : Page
    {
		int tsk_id;
		public int bugid;

		public Security security;
		private TaskService _taskService = new TaskService();
		private UserService _userService = new UserService();

		public void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


		public void Page_Load(Object sender, EventArgs e)
		{

			Util.do_not_cache(Response);

			security = new Security();
			security.check_security(HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

			msg.InnerText = "";

			string string_bugid = btnet.Util.sanitize_integer(Request["bugid"]);
			bugid = Convert.ToInt32(string_bugid);

			int permission_level = Bug.get_bug_permission_level(bugid, security);

			if (permission_level != Security.PERMISSION_ALL)
			{
				Response.Write("You are not allowed to edit tasks for this item");
				Response.End();
			}

			if (security.user.is_admin || security.user.can_edit_tasks)
			{
				// allowed	
			}
			else
			{
				Response.Write("You are not allowed to edit tasks");
				Response.End();
			}


			string string_tsk_id = btnet.Util.sanitize_integer(Request["id"]);
			tsk_id_static.InnerHtml = string_tsk_id;
			tsk_id = Convert.ToInt32(string_tsk_id);

			if (!IsPostBack)
			{

				titl.InnerText = btnet.Util.get_setting("AppTitle", "BugTracker.NET") + " - "
					+ "edit task";

				bugid_label.InnerHtml = btnet.Util.capitalize_first_letter(btnet.Util.get_setting("SingularBugLabel", "bug")) + " ID:";
				bugid_static.InnerHtml = Convert.ToString(bugid);

				load_users_dropdowns(bugid);

				if (Util.get_setting("ShowTaskAssignedTo", "1") == "0")
				{
					assigned_to_tr.Visible = false;
				}

				if (Util.get_setting("ShowTaskPlannedStartDate", "1") == "0")
				{
					planned_start_date_tr.Visible = false;
				}
				if (Util.get_setting("ShowTaskActualStartDate", "1") == "0")
				{
					actual_start_date_tr.Visible = false;
				}

				if (Util.get_setting("ShowTaskPlannedEndDate", "1") == "0")
				{
					planned_end_date_tr.Visible = false;
				}
				if (Util.get_setting("ShowTaskActualEndDate", "1") == "0")
				{
					actual_end_date_tr.Visible = false;
				}

				if (Util.get_setting("ShowTaskPlannedDuration", "1") == "0")
				{
					planned_duration_tr.Visible = false;
				}
				if (Util.get_setting("ShowTaskActualDuration", "1") == "0")
				{
					actual_duration_tr.Visible = false;
				}


				if (Util.get_setting("ShowTaskDurationUnits", "1") == "0")
				{
					duration_units_tr.Visible = false;
				}

				if (Util.get_setting("ShowTaskPercentComplete", "1") == "0")
				{
					percent_complete_tr.Visible = false;
				}

				if (Util.get_setting("ShowTaskStatus", "1") == "0")
				{
					status_tr.Visible = false;
				}

				if (Util.get_setting("ShowTaskSortSequence", "1") == "0")
				{
					sort_sequence_tr.Visible = false;
				}


				// add or edit?
				if (tsk_id == 0)
				{
					tsk_id_tr.Visible = false;
					sub.Value = "Create";

					string default_duration_units = btnet.Util.get_setting("TaskDefaultDurationUnits", "hours");
					duration_units.Items.FindByText(default_duration_units).Selected = true;

					string default_hour = btnet.Util.get_setting("TaskDefaultHour", "09");
					planned_start_hour.Items.FindByText(default_hour).Selected = true;
					actual_start_hour.Items.FindByText(default_hour).Selected = true;
					planned_end_hour.Items.FindByText(default_hour).Selected = true;
					actual_end_hour.Items.FindByText(default_hour).Selected = true;

					string default_status = btnet.Util.get_setting("TaskDefaultStatus", "[no status]");
					status.Items.FindByText(default_status).Selected = true;
				}
				else
				{

					// Get this entry's data from the db and fill in the form
					DataRow dr = _taskService.GetTaskById(tsk_id, bugid);

					assigned_to.Items.FindByValue(Convert.ToString(dr["tsk_assigned_to_user"])).Selected = true;

					duration_units.Items.FindByText(Convert.ToString(dr["tsk_duration_units"])).Selected = true;

					status.Items.FindByValue(Convert.ToString(dr["tsk_status"])).Selected = true;

					planned_duration.Value = btnet.Util.format_db_value(dr["tsk_planned_duration"]);
					actual_duration.Value = btnet.Util.format_db_value(dr["tsk_actual_duration"]);
					percent_complete.Value = Convert.ToString(dr["tsk_percent_complete"]);
					sort_sequence.Value = Convert.ToString(dr["tsk_sort_sequence"]);
					desc.Value = Convert.ToString(dr["tsk_description"]);

					load_date_hour_min(
						planned_start_date,
						planned_start_hour,
						planned_start_min,
						dr["tsk_planned_start_date"]);

					load_date_hour_min(
						actual_start_date,
						actual_start_hour,
						actual_start_min,
						dr["tsk_actual_start_date"]);

					load_date_hour_min(
						planned_end_date,
						planned_end_hour,
						planned_end_min,
						dr["tsk_planned_end_date"]);

					load_date_hour_min(
						actual_end_date,
						actual_end_hour,
						actual_end_min,
						dr["tsk_actual_end_date"]);

					sub.Value = "Update";
				}
			}
			else
			{
				on_update();
			}
		}

		
		void load_date_hour_min(
			HtmlInputText date_control,
			DropDownList hour_control,
			DropDownList min_control,
			object date)
		{

			if (Convert.IsDBNull(date))
			{
				date_control.Value = "";
			}
			else
			{
				DateTime dt = Convert.ToDateTime(date);
				string temp_date = dt.Year.ToString("0000") + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00");
				date_control.Value = btnet.Util.format_db_date_and_time(Convert.ToDateTime(temp_date));
				hour_control.Items.FindByValue(dt.Hour.ToString("00")).Selected = true;
				min_control.Items.FindByValue(dt.Minute.ToString("00")).Selected = true;
			}
		}

		
		void load_users_dropdowns(int bugid)
		{
			// What's selected now?   Save it before we refresh the dropdown.
			string current_value = "";

			if (IsPostBack)
			{
				current_value = assigned_to.SelectedItem.Value;
			}

			DataSet userInfo = _userService.GetUserListForTasks(bugid, security.user.org, security.user.other_orgs_permission_level);

			assigned_to.DataSource = new DataView(userInfo.Tables[0]);
			assigned_to.DataTextField = "us_username";
			assigned_to.DataValueField = "us_id";
			assigned_to.DataBind();
			assigned_to.Items.Insert(0, new ListItem("[not assigned]", "0"));

			status.DataSource = new DataView(userInfo.Tables[1]);
			status.DataTextField = "st_name";
			status.DataValueField = "st_id";
			status.DataBind();
			status.Items.Insert(0, new ListItem("[no status]", "0"));

			// by default, assign the entry to the same user to whom the bug is assigned to?
			// or should it be assigned to the logged in user?
			if (tsk_id == 0)
			{
				int default_assigned_to_user = (int)userInfo.Tables[2].Rows[0][0];
				ListItem li = assigned_to.Items.FindByValue(Convert.ToString(default_assigned_to_user));
				if (li != null)
				{
					li.Selected = true;
				}
			}
		}

		
		bool ValidateForm()
		{
			bool good = true;

			if (sort_sequence.Value != "")
			{
				if (!Util.is_int(sort_sequence.Value))
				{
					good = false;
					sort_sequence_err.InnerText = "Sort Sequence must be an integer.";
				}
				else
				{
					sort_sequence_err.InnerText = "";
				}
			}
			else
			{
				sort_sequence_err.InnerText = "";
			}

			if (percent_complete.Value != "")
			{
				if (!Util.is_int(percent_complete.Value))
				{
					good = false;
					percent_complete_err.InnerText = "Percent Complete must be from 0 to 100.";
				}
				else
				{
					int percent_complete_int = Convert.ToInt32(percent_complete.Value);
					if (percent_complete_int >= 0 && percent_complete_int <= 100)
					{
						// good
						percent_complete_err.InnerText = "";
					}
					else
					{
						good = false;
						percent_complete_err.InnerText = "Percent Complete must be from 0 to 100.";
					}

				}
			}
			else
			{
				percent_complete_err.InnerText = "";
			}

			if (planned_duration.Value != "")
			{
				string err = Util.is_valid_decimal("Planned Duration", planned_duration.Value, 4, 2);

				if (err != "")
				{
					good = false;
					planned_duration_err.InnerText = err;
				}
				else
				{
					planned_duration_err.InnerText = "";
				}
			}
			else
			{
				planned_duration_err.InnerText = "";
			}

			if (actual_duration.Value != "")
			{
				string err = Util.is_valid_decimal("Actual Duration", actual_duration.Value, 4, 2);

				if (err != "")
				{
					good = false;
					actual_duration_err.InnerText = err;
				}
				else
				{
					actual_duration_err.InnerText = "";
				}
			}
			else
			{
				actual_duration_err.InnerText = "";
			}

			return good;
		}


		private DateTime? ConvertStringToDate(string date, string hour, string min)
		{
			DateTime convertedDate = new DateTime();
			bool isDateGood = false;

			if (!string.IsNullOrEmpty(date))
			{
				string dateString = String.Format("{0} {1}:{2}:00", date, hour, min);

				isDateGood = DateTime.TryParse(dateString, out convertedDate);
			}

			if (isDateGood)
				return convertedDate;
			else
				return null;
        }

		private decimal? ConvertStringToDecimal(string value)
		{
			decimal convertedDecimal;

			bool isValueGood = decimal.TryParse(value, out convertedDecimal);

			if (isValueGood)
				return convertedDecimal;
			else 
				return null;
        }

        private int? ConvertStringToInt(string value)
        {
            int convertedInt;

            bool isValueGood = int.TryParse(value, out convertedInt);

            if (isValueGood)
                return convertedInt;
            else
                return null;
        }
		
		void on_update()
		{
			bool good = ValidateForm();

			if (good)
			{
                EditTask taskModel = new EditTask();
                taskModel.TaskId = tsk_id; //Only for edits
                taskModel.BugId = bugid;
                taskModel.CreatedBy = security.user.usid; //Only for new tasks
                taskModel.LastUpdatedBy = security.user.usid;
				taskModel.AssignedTo = ConvertStringToInt(assigned_to.SelectedItem.Value);
                taskModel.PlannedStartDate = ConvertStringToDate(planned_start_date.Value, planned_start_hour.SelectedItem.Value, planned_start_min.SelectedItem.Value);
                taskModel.ActualStartDate = ConvertStringToDate(actual_start_date.Value, actual_start_hour.SelectedItem.Value, actual_start_min.SelectedItem.Value);
                taskModel.PlannedEndDate = ConvertStringToDate(planned_end_date.Value, planned_end_hour.SelectedItem.Value, planned_end_min.SelectedItem.Value);
                taskModel.ActualEndDate = ConvertStringToDate(actual_end_date.Value, actual_end_hour.SelectedItem.Value, actual_end_min.SelectedItem.Value);
                taskModel.PlannedDuration = ConvertStringToDecimal(planned_duration.Value);
                taskModel.ActualDuration = ConvertStringToDecimal(actual_duration.Value);
                taskModel.PercentComplete = ConvertStringToInt(percent_complete.Value);
                taskModel.Status = int.Parse(status.SelectedItem.Value);
                taskModel.SortSequence = ConvertStringToInt(sort_sequence.Value);
                taskModel.Description = desc.Value;
                taskModel.DurationUnits = duration_units.SelectedItem.Value;

                if (tsk_id == 0)  // insert new
				{
					_taskService.InsertTask(taskModel);
				}
				else // edit existing
				{
                    _taskService.UpdateTask(taskModel);
				}

                Bug.send_notifications(Bug.UPDATE, bugid, security);

				Response.Redirect("tasks.aspx?bugid=" + Convert.ToString(bugid));
			}
			else
			{
				if (tsk_id == 0)  // insert new
				{
					msg.InnerText = "Task was not created.";
				}
				else // edit existing
				{
					msg.InnerText = "Task was not updated.";
				}
			}
		}
	}
}
