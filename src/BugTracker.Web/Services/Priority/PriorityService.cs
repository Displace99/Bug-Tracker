using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace BugTracker.Web.Services.Priority
{
    public class PriorityService
    {
        public DataSet GetPriorityList()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select pr_id [id], pr_name [description], pr_sort_seq [sort seq], ");
            sql.AppendLine("'<div style=''background:' + pr_background_color + ';''>' + pr_background_color + '</div>' [background<br>color], ");
            sql.AppendLine("pr_style [css<br>class], case when pr_default = 1 then 'Y' else 'N' end [default], pr_id [hidden]");
            sql.AppendLine("from priorities");

            DataSet priorityDS = DbUtil.get_dataset(sql.ToString());

            return priorityDS;
        }
    }
}