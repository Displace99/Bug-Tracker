using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace BugTracker.Web.Services.Status
{
    public class StatusService
    {
        public DataSet GetStatusList()
        {
            string sql = @"select st_id [id], st_name [status], st_sort_seq [sort seq], st_style [css<br>class],
		        case when st_default = 1 then 'Y' else 'N' end [default],
		        st_id [hidden]
		        from statuses order by st_sort_seq";

            return DbUtil.get_dataset(sql);
        }
    }
}