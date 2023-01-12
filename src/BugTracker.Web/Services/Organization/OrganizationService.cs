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
        public DataSet GetOrganizationList()
        {
            string sql = "SELECT og_id, og_name FROM orgs ORDER BY og_name;";

            return DbUtil.get_dataset(sql);
        }
    }
}