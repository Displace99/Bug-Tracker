using btnet;
using BugTracker.Web.Models.Organization;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace BugTracker.Web.Services.Organization
{
	public class OrganizationService
    {
        /// <summary>
        /// Returns a list of all Organizations in the system
        /// </summary>
        /// <returns></returns>
        public DataSet GetOrganizationDropDownList()
        {
            string sql = "SELECT og_id, og_name FROM orgs ORDER BY og_name;";

            return DbUtil.get_dataset(sql);
        }

		public string GetOrgNameById(int orgId)
		{
			string sql = "SELECT og_name FROM orgs WHERE og_id = @orgId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@orgId", orgId);

			DataRow dr = DbUtil.get_datarow(cmd);

			return dr["og_name"].ToString();
        }

		/// <summary>
		/// Gets details for a specific org
		/// </summary>
		/// <param name="orgId"></param>
		/// <returns></returns>
		public DataRow GetOrgDetailsById(int orgId)
		{
            string sql = @"select *,isnull(og_domain,'') og_domain2 from orgs where og_id = @orgId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@orgId", orgId);

            return DbUtil.get_datarow(cmd);
        }

		/// <summary>
		/// Gets an list of organizations for non admins
		/// </summary>
		/// <returns></returns>
        public DataSet GetOrgListForNonAdmins()
        {
            StringBuilder sql = new StringBuilder();
            
			sql.AppendLine("SELECT og_id, og_name");
			sql.AppendLine("FROM orgs");
			sql.AppendLine("WHERE og_non_admins_can_use = 1");
            sql.AppendLine("ORDER BY og_name;");

            return DbUtil.get_dataset(sql.ToString());
        }

		/// <summary>
		/// Returns a DataSet of all organizations in the system, including details.
		/// </summary>
		/// <returns>DataSet</returns>
        public DataSet GetOrganizationList()
        {
            string sql = @"select og_id [id],
			'<a href=edit_org.aspx?id=' + convert(varchar,og_id) + '>edit</a>' [$no_sort_edit],
			'<a href=delete_org.aspx?id=' + convert(varchar,og_id) + '>delete</a>' [$no_sort_delete],
			og_name[desc],
			case when og_active = 1 then 'Y' else 'N' end [active],
			case when og_can_search = 1 then 'Y' else 'N' end [can<br>search],
			case when og_non_admins_can_use = 1 then 'Y' else 'N' end [non-admin<br>can use],
			case when og_can_only_see_own_reported = 1 then 'Y' else 'N' end [can see<br>only own bugs],
			case
				when og_other_orgs_permission_level = 0 then 'None'
				when og_other_orgs_permission_level = 1 then 'Read Only'
				else 'Add/Edit' end [other orgs<br>permission<br>level],
			case when og_external_user = 1 then 'Y' else 'N' end [external],
			case when og_can_be_assigned_to = 1 then 'Y' else 'N' end [can<br>be assigned to],
			case
				when og_status_field_permission_level = 0 then 'None'
				when og_status_field_permission_level = 1 then 'Read Only'
				else 'Add/Edit' end [status<br>permission<br>level],
			case
				when og_assigned_to_field_permission_level = 0 then 'None'
				when og_assigned_to_field_permission_level = 1 then 'Read Only'
				else 'Add/Edit' end [assigned to<br>permission<br>level],
			case
				when og_priority_field_permission_level = 0 then 'None'
				when og_priority_field_permission_level = 1 then 'Read Only'
				else 'Add/Edit' end [priority<br>permission<br>level],
			isnull(og_domain,'')[domain]
			from orgs order by og_name";

			return DbUtil.get_dataset(sql);
        }

		/// <summary>
		/// Gets an org list based on users permission level
		/// </summary>
		/// <param name="orgPermissionLevel"></param>
		/// <param name="userOrgId"></param>
		/// <returns></returns>
		public DataView GetOrganizationListByPermission(int orgPermissionLevel, int userOrgId)
		{
			string sql = string.Empty;
			SqlCommand cmd = new SqlCommand();

            if (orgPermissionLevel != 0)
            {
                sql = "select og_id, og_name from orgs order by og_name;";
            }
            else
            {
                sql = "select og_id, og_name from orgs where og_id = @orgId order by og_name;";
				cmd.Parameters.AddWithValue("@orgId", userOrgId);
            }

			cmd.CommandText = sql;
			return DbUtil.get_dataview(cmd);
        }

		/// <summary>
		/// Adds a new Organization to the system
		/// </summary>
		/// <param name="org"></param>
		public void CreateOrganization(Org org)
		{
            string sql = @"
			insert into orgs
				(og_name,
				og_domain,
				og_active,
				og_non_admins_can_use,
				og_external_user,
				og_can_edit_sql,
				og_can_delete_bug,
				og_can_edit_and_delete_posts,
				og_can_merge_bugs,
				og_can_mass_edit_bugs,
				og_can_use_reports,
				og_can_edit_reports,
				og_can_be_assigned_to,
				og_can_view_tasks,
				og_can_edit_tasks,
				og_can_search,
				og_can_only_see_own_reported,
				og_can_assign_to_internal_users,
				og_other_orgs_permission_level,
				og_project_field_permission_level,
				og_org_field_permission_level,
				og_category_field_permission_level,
				og_tags_field_permission_level,
				og_priority_field_permission_level,
				og_status_field_permission_level,
				og_assigned_to_field_permission_level,
				og_udf_field_permission_level)
				values (
				@name, 
				@domain,
				@active,
				@non_admins_can_use,
				@external_user,
				@can_edit_sql,
				@can_delete_bug,
				@can_edit_and_delete_posts,
				@can_merge_bugs,
				@can_mass_edit_bugs,
				@can_use_reports,
				@can_edit_reports,
				@can_be_assigned_to,
				@can_view_tasks,
				@can_edit_tasks,
				@can_search,
				@can_only_see_own_reported,
				@can_assign_to_internal_users,
				@other_orgs,
				@flp_project,
				@flp_org,
				@flp_category,
				@flp_tags,
				@flp_priority,
				@flp_status,
				@flp_assigned_to,
				@flp_udf);
				select scope_identity()";

			SqlCommand cmd = new SqlCommand(sql);
			cmd.Parameters.AddWithValue("@name", org.Name);
			cmd.Parameters.AddWithValue("@domain", org.Domain);
			cmd.Parameters.AddWithValue("@active", org.IsActive);
			cmd.Parameters.AddWithValue("@non_admins_can_use", org.NonAdminsCanUse);
			cmd.Parameters.AddWithValue("@external_user", org.ExternalUser);
			cmd.Parameters.AddWithValue("@can_edit_sql", org.CanEditSQL);
			cmd.Parameters.AddWithValue("@can_delete_bug", org.CanDeleteBug);
			cmd.Parameters.AddWithValue("@can_edit_and_delete_posts", org.CanEditDeleteComment);
			cmd.Parameters.AddWithValue("@can_merge_bugs", org.CanMergeBugs);
			cmd.Parameters.AddWithValue("@can_mass_edit_bugs", org.CanMassEditBugs);
			cmd.Parameters.AddWithValue("@can_use_reports", org.CanUseReports);
			cmd.Parameters.AddWithValue("@can_edit_reports", org.CanEditReports);
			cmd.Parameters.AddWithValue("@can_be_assigned_to", org.CanBeAssignedTo);
			cmd.Parameters.AddWithValue("@can_view_tasks", org.CanViewTasks);
			cmd.Parameters.AddWithValue("@can_edit_tasks", org.CanEditTasks);
			cmd.Parameters.AddWithValue("@can_search", org.CanSearch);
			cmd.Parameters.AddWithValue("@can_only_see_own_reported", org.CanOnlySeeOwnReportedBugs);
			cmd.Parameters.AddWithValue("@can_assign_to_internal_users", org.CanAssignToInternalUsers);
			cmd.Parameters.AddWithValue("@other_orgs", org.OtherOrgPermission);
			cmd.Parameters.AddWithValue("@flp_project", org.ProjectFieldPermission);
			cmd.Parameters.AddWithValue("@flp_org", org.OrgFieldPermission);
			cmd.Parameters.AddWithValue("@flp_category", org.CategoryFieldPermission);
			cmd.Parameters.AddWithValue("@flp_tags", org.TagFieldPermission);
			cmd.Parameters.AddWithValue("@flp_priority", org.PriorityFieldPermission);
			cmd.Parameters.AddWithValue("@flp_status", org.StatusFieldPermission);
			cmd.Parameters.AddWithValue("@flp_assigned_to", org.AssignedToFieldPermission);
			cmd.Parameters.AddWithValue("@flp_udf", org.UserDefinedFieldPermission);

			org.Id = Convert.ToInt32(DbUtil.execute_scalar(cmd));

			//Updates any custom fields
			UpdateCustomFields(org);
        }

		/// <summary>
		/// Updates an existing Org
		/// </summary>
		/// <param name="org"></param>
		public void UpdateOrganizaton(Org org)
		{
            string sql = @"
				update orgs set
					og_name = @name,
					og_domain = @domain,
					og_active = @active,
					og_non_admins_can_use = @non_admins_can_use,
					og_external_user = @external_user,
					og_can_edit_sql = @can_edit_sql,
					og_can_delete_bug = @can_delete_bug,
					og_can_edit_and_delete_posts = @can_edit_and_delete_posts,
					og_can_merge_bugs = @can_merge_bugs,
					og_can_mass_edit_bugs = @can_mass_edit_bugs,
					og_can_use_reports = @can_use_reports,
					og_can_edit_reports = @can_edit_reports,
					og_can_be_assigned_to = @can_be_assigned_to,
					og_can_view_tasks = @can_view_tasks,
					og_can_edit_tasks = @can_edit_tasks,
					og_can_search = @can_search,
					og_can_only_see_own_reported = @can_only_see_own_reported,
					og_can_assign_to_internal_users = @can_assign_to_internal_users,
					og_other_orgs_permission_level = @other_orgs,
					og_project_field_permission_level = @flp_project,
					og_org_field_permission_level = @flp_org,
					og_category_field_permission_level = @flp_category,
					og_tags_field_permission_level = @flp_tags,
					og_priority_field_permission_level = @flp_priority,
					og_status_field_permission_level = @flp_status,
					og_assigned_to_field_permission_level = @flp_assigned_to,
					og_udf_field_permission_level = @flp_udf
					where og_id = @ogId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@name", org.Name);
            cmd.Parameters.AddWithValue("@domain", org.Domain);
            cmd.Parameters.AddWithValue("@active", org.IsActive);
            cmd.Parameters.AddWithValue("@non_admins_can_use", org.NonAdminsCanUse);
            cmd.Parameters.AddWithValue("@external_user", org.ExternalUser);
            cmd.Parameters.AddWithValue("@can_edit_sql", org.CanEditSQL);
            cmd.Parameters.AddWithValue("@can_delete_bug", org.CanDeleteBug);
            cmd.Parameters.AddWithValue("@can_edit_and_delete_posts", org.CanEditDeleteComment);
            cmd.Parameters.AddWithValue("@can_merge_bugs", org.CanMergeBugs);
            cmd.Parameters.AddWithValue("@can_mass_edit_bugs", org.CanMassEditBugs);
            cmd.Parameters.AddWithValue("@can_use_reports", org.CanUseReports);
            cmd.Parameters.AddWithValue("@can_edit_reports", org.CanEditReports);
            cmd.Parameters.AddWithValue("@can_be_assigned_to", org.CanBeAssignedTo);
            cmd.Parameters.AddWithValue("@can_view_tasks", org.CanViewTasks);
            cmd.Parameters.AddWithValue("@can_edit_tasks", org.CanEditTasks);
            cmd.Parameters.AddWithValue("@can_search", org.CanSearch);
            cmd.Parameters.AddWithValue("@can_only_see_own_reported", org.CanOnlySeeOwnReportedBugs);
            cmd.Parameters.AddWithValue("@can_assign_to_internal_users", org.CanAssignToInternalUsers);
            cmd.Parameters.AddWithValue("@other_orgs", org.OtherOrgPermission);
            cmd.Parameters.AddWithValue("@flp_project", org.ProjectFieldPermission);
            cmd.Parameters.AddWithValue("@flp_org", org.OrgFieldPermission);
            cmd.Parameters.AddWithValue("@flp_category", org.CategoryFieldPermission);
            cmd.Parameters.AddWithValue("@flp_tags", org.TagFieldPermission);
            cmd.Parameters.AddWithValue("@flp_priority", org.PriorityFieldPermission);
            cmd.Parameters.AddWithValue("@flp_status", org.StatusFieldPermission);
            cmd.Parameters.AddWithValue("@flp_assigned_to", org.AssignedToFieldPermission);
            cmd.Parameters.AddWithValue("@flp_udf", org.UserDefinedFieldPermission);
			cmd.Parameters.AddWithValue("@ogId", org.Id);

			DbUtil.execute_nonquery(cmd);

			UpdateCustomFields(org);
        }

		/// <summary>
		/// Updates custom fields for organziations
		/// </summary>
		/// <param name="org"></param>
		public void UpdateCustomFields(Org org)
		{
            //Due to the fluid nature of custom fields, it's easier if we update them separately from the rest of the fields.
            if (org.CustomFieldPermissions.Count > 0) 
			{
				//For some reason trying to parameterize the field name and permisson level doesn't work. We will want to ultimatly replace this 
				//with a more stable solution.
				string sql = string.Format("update orgs set {0} WHERE og_id = @ogId",
					string.Join(",", org.CustomFieldPermissions.Select(n => n.FieldName + " = " + n.PermissionLevel).ToArray()));

				SqlCommand cmd = new SqlCommand(sql);

				cmd.Parameters.AddWithValue("@ogId", org.Id);

				DbUtil.execute_nonquery(cmd);
            }
        }

		/// <summary>
		/// Deletes an organization from the database. THIS IS PERMANENT!
		/// </summary>
		/// <param name="orgId"></param>
		public void DeleteOrganization(int orgId)
		{
            string sql = @"delete orgs where og_id = @orgId";

			SqlCommand cmd = new SqlCommand(sql);
			cmd.Parameters.AddWithValue("@orgId", orgId);

            DbUtil.execute_nonquery(cmd);
        }

		/// <summary>
		/// Checks to see if the specific Org has any bugs or queries attached to it.
		/// Used when attempting to delete an org
		/// </summary>
		/// <param name="orgId"></param>
		/// <returns></returns>
		public bool DoesOrgHaveRelatedEntities(int orgId)
		{
            string sql = @"declare @cnt int
			select @cnt = count(1) from users where us_org = @orgId;
			select @cnt = @cnt + count(1) from queries where qu_org = @orgId;
			select @cnt = @cnt + count(1) from bugs where bg_org = @orgId;
			select og_name, @cnt [cnt] from orgs where og_id = @orgId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@orgId", orgId);

            DataRow dr = DbUtil.get_datarow(cmd);

			return (int)dr["cnt"] > 0;
        }
    }
}