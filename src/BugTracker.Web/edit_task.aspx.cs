using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using btnet;

namespace BugTracker.Web
{
    public partial class edit_task : Page
    {
		int tsk_id;
		public int bugid;
		String sql;


		public Security security;

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

					sql = @"select * from bug_tasks where tsk_id = $tsk_id and tsk_bug = $bugid";
					sql = sql.Replace("$tsk_id", Convert.ToString(tsk_id));
					sql = sql.Replace("$bugid", Convert.ToString(bugid));
					DataRow dr = btnet.DbUtil.get_datarow(sql);

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

			sql = @"
				declare @project int
				declare @assigned_to int
				select @project = bg_project, @assigned_to = bg_assigned_to_user from bugs where bg_id = $bg_id";

			// Load the user dropdown, which changes per project
			// Only users explicitly allowed will be listed
			if (btnet.Util.get_setting("DefaultPermissionLevel", "2") == "0")
			{
				sql += @"

					/* users this project */ select us_id, case when $fullnames then us_lastname + ', ' + us_firstname else us_username end us_username
					from users
					inner join orgs on us_org = og_id
					where us_active = 1
					and og_can_be_assigned_to = 1
					and ($og_other_orgs_permission_level <> 0 or $og_id = og_id or og_external_user = 0)
					and us_id in
						(select pu_user from project_user_xref
							where pu_project = @project
							and pu_permission_level <> 0)
					order by us_username; ";
			}
			// Only users explictly DISallowed will be omitted
			else
			{
				sql += @"
					/* users this project */ select us_id, case when $fullnames then us_lastname + ', ' + us_firstname else us_username end us_username
					from users
					inner join orgs on us_org = og_id
					where us_active = 1
					and og_can_be_assigned_to = 1
					and ($og_other_orgs_permission_level <> 0 or $og_id = og_id or og_external_user = 0)
					and us_id not in
						(select pu_user from project_user_xref
							where pu_project = @project
							and pu_permission_level = 0)
					order by us_username; ";
			}

			sql += "\nselect st_id, st_name from statuses order by st_sort_seq, st_name";

			sql += "\nselect isnull(@assigned_to,0) ";

			sql = sql.Replace("$og_id", Convert.ToString(security.user.org));
			sql = sql.Replace("$og_other_orgs_permission_level", Convert.ToString(security.user.other_orgs_permission_level));
			sql = sql.Replace("$bg_id", Convert.ToString(bugid));

			if (Util.get_setting("UseFullNames", "0") == "0")
			{
				// false condition
				sql = sql.Replace("$fullnames", "0 = 1");
			}
			else
			{
				// true condition
				sql = sql.Replace("$fullnames", "1 = 1");
			}

			assigned_to.DataSource = new DataView((DataTable)btnet.DbUtil.get_dataset(sql).Tables[0]);
			assigned_to.DataTextField = "us_username";
			assigned_to.DataValueField = "us_id";
			assigned_to.DataBind();
			assigned_to.Items.Insert(0, new ListItem("[not assigned]", "0"));

			status.DataSource = new DataView((DataTable)btnet.DbUtil.get_dataset(sql).Tables[1]);
			status.DataTextField = "st_name";
			status.DataValueField = "st_id";
			status.DataBind();
			status.Items.Insert(0, new ListItem("[no status]", "0"));

			// by default, assign the entry to the same user to whom the bug is assigned to?
			// or should it be assigned to the logged in user?
			if (tsk_id == 0)
			{
				int default_assigned_to_user = (int)btnet.DbUtil.get_dataset(sql).Tables[2].Rows[0][0];
				ListItem li = assigned_to.Items.FindByValue(Convert.ToString(default_assigned_to_user));
				if (li != null)
				{
					li.Selected = true;
				}
			}
		}

		
		Boolean validate()
		{
			Boolean good = true;

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
				string err = btnet.Util.is_valid_decimal("Planned Duration", planned_duration.Value, 4, 2);

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
				string err = btnet.Util.is_valid_decimal("Actual Duration", actual_duration.Value, 4, 2);

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


		// This might not be right.   
		string format_date_hour_min(string date, string hour, string min)
		{
			if (!string.IsNullOrEmpty(date))
			{
				return btnet.Util.format_local_date_into_db_format(
					date
					+ " "
					+ hour
					+ ":"
					+ min
					+ ":00");
			}
			else
			{
				return "";
			}
		}

		
		string format_decimal_for_db(string s)
		{
			if (s == "")
				return "null";
			else
				return btnet.Util.format_local_decimal_into_db_format(s);
		}


		
		string format_number_for_db(string s)
		{
			if (s == "")
				return "null";
			else
				return s;
		}

		
		void on_update()
		{

			Boolean good = validate();

			if (good)
			{
				if (tsk_id == 0)  // insert new
				{
					sql = @"
						insert into bug_tasks (
						tsk_bug,
						tsk_created_user,
						tsk_created_date,
						tsk_last_updated_user,
						tsk_last_updated_date,
						tsk_assigned_to_user,
						tsk_planned_start_date,
						tsk_actual_start_date,
						tsk_planned_end_date,
						tsk_actual_end_date,
						tsk_planned_duration,
						tsk_actual_duration,
						tsk_duration_units,
						tsk_percent_complete,
						tsk_status,
						tsk_sort_sequence,
						tsk_description
						)
						values (
						$tsk_bug,
						$tsk_created_user,
						getdate(),
						$tsk_last_updated_user,
						getdate(),
						$tsk_assigned_to_user,
						'$tsk_planned_start_date',
						'$tsk_actual_start_date',
						'$tsk_planned_end_date',
						'$tsk_actual_end_date',
						$tsk_planned_duration,
						$tsk_actual_duration,
						N'$tsk_duration_units',
						$tsk_percent_complete,
						$tsk_status,
						$tsk_sort_sequence,
						N'$tsk_description'
						)

						declare @tsk_id int
						select @tsk_id = scope_identity()

						insert into bug_posts
						(bp_bug, bp_user, bp_date, bp_comment, bp_type)
						values($tsk_bug, $tsk_last_updated_user, getdate(), N'added task ' + convert(varchar, @tsk_id), 'update')";


					sql = sql.Replace("$tsk_created_user", Convert.ToString(security.user.usid));


				}
				else // edit existing
				{

					sql = @"
						update bug_tasks set
						tsk_last_updated_user = $tsk_last_updated_user,
						tsk_last_updated_date = getdate(),
						tsk_assigned_to_user = $tsk_assigned_to_user,
						tsk_planned_start_date = '$tsk_planned_start_date',
						tsk_actual_start_date = '$tsk_actual_start_date',
						tsk_planned_end_date = '$tsk_planned_end_date',
						tsk_actual_end_date = '$tsk_actual_end_date',
						tsk_planned_duration = $tsk_planned_duration,
						tsk_actual_duration = $tsk_actual_duration,
						tsk_duration_units = N'$tsk_duration_units',
						tsk_percent_complete = $tsk_percent_complete,
						tsk_status = $tsk_status,
						tsk_sort_sequence = $tsk_sort_sequence,
						tsk_description = N'$tsk_description'
						where tsk_id = $tsk_id
                
						insert into bug_posts
						(bp_bug, bp_user, bp_date, bp_comment, bp_type)
						values($tsk_bug, $tsk_last_updated_user, getdate(), N'updated task $tsk_id', 'update')";

					sql = sql.Replace("$tsk_id", Convert.ToString(tsk_id));

				}

				sql = sql.Replace("$tsk_bug", Convert.ToString(bugid));
				sql = sql.Replace("$tsk_last_updated_user", Convert.ToString(security.user.usid));

				sql = sql.Replace("$tsk_planned_start_date", format_date_hour_min(
					planned_start_date.Value,
					planned_start_hour.SelectedItem.Value,
					planned_start_min.SelectedItem.Value));

				sql = sql.Replace("$tsk_actual_start_date", format_date_hour_min(
					actual_start_date.Value,
					actual_start_hour.SelectedItem.Value,
					actual_start_min.SelectedItem.Value));

				sql = sql.Replace("$tsk_planned_end_date", format_date_hour_min(
					planned_end_date.Value,
					planned_end_hour.SelectedItem.Value,
					planned_end_min.SelectedItem.Value));

				sql = sql.Replace("$tsk_actual_end_date", format_date_hour_min(
					actual_end_date.Value,
					actual_end_hour.SelectedItem.Value,
					actual_end_min.SelectedItem.Value));

				sql = sql.Replace("$tsk_planned_duration", format_decimal_for_db(planned_duration.Value));
				sql = sql.Replace("$tsk_actual_duration", format_decimal_for_db(actual_duration.Value));
				sql = sql.Replace("$tsk_percent_complete", format_number_for_db(percent_complete.Value));
				sql = sql.Replace("$tsk_status", status.SelectedItem.Value);
				sql = sql.Replace("$tsk_sort_sequence", format_number_for_db(sort_sequence.Value));
				sql = sql.Replace("$tsk_assigned_to_user", assigned_to.SelectedItem.Value);
				sql = sql.Replace("$tsk_description", desc.Value.Replace("'", "''"));
				sql = sql.Replace("$tsk_duration_units", duration_units.SelectedItem.Value.Replace("'", "''"));

				DbUtil.execute_nonquery(sql);

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
