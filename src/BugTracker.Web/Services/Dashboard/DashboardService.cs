using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;

namespace BugTracker.Web.Services.Dashboard
{
    public class DashboardService
    {
        public DataSet GetDashboardItems(int userId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select ds.*, rp_desc from dashboard_items ds inner join reports on rp_id = ds_report");
            sql.AppendLine("where ds_user = @userId");
            sql.AppendLine("order by ds_col, ds_row");

            SqlCommand cmd = new SqlCommand(sql.ToString());
            cmd.Parameters.AddWithValue("@userId", userId);

            return DbUtil.get_dataset(cmd);
        }
    }
}