﻿using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

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

		public DataRow GetOrgDetailsById(int orgId)
		{
            string sql = @"select *,isnull(og_domain,'') og_domain2 from orgs where og_id = @orgId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@orgId", orgId);

            return DbUtil.get_datarow(cmd);
        }

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