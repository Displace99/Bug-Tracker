using btnet;
using System;
using System.Collections.Generic;
using System.Data;
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

        public DataSet GetOrgListForNonAdmins()
        {
            StringBuilder sql = new StringBuilder();
            
			sql.AppendLine("SELECT og_id, og_name");
			sql.AppendLine("FROM orgs");
			sql.AppendLine("WHERE og_non_admins_can_use = 1");
            sql.AppendLine("ORDER BY og_name;");

            return DbUtil.get_dataset(sql.ToString());
        }
    }
}