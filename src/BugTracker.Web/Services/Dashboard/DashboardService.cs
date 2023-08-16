using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Permissions;
using System.Text;
using System.Web;

namespace BugTracker.Web.Services.Dashboard
{
    public class DashboardService
    {
        /// <summary>
        /// Gets specific items for a users dashboard. 
        /// Each user can only have one dashboard
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetDashboardItems(int userId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select ds.*, rp_desc from dashboard_items ds inner join reports on rp_id = ds_report ");
            sql.AppendLine("where ds_user = @userId ");
            sql.AppendLine("order by ds_col, ds_row");

            SqlCommand cmd = new SqlCommand(sql.ToString());
            cmd.Parameters.AddWithValue("@userId", userId);

            return DbUtil.get_dataset(cmd);
        }

        /// <summary>
        /// Adds an item to the dashboard. An item is currently an existing report.
        /// </summary>
        /// <param name="userId">User Id of the person adding the item</param>
        /// <param name="reportId">Id of the report to add to the dashboard</param>
        /// <param name="chartType">Type of report item. i.e. BarChart, PieChart, Table, etc.</param>
        /// <param name="dashboardColumn">Which column is this being added to. Options are 1 or 2</param>
        public void AddDashboardItem(int userId, int reportId, string chartType, int dashboardColumn)
        {
            string sql = @"
            declare @last_row int
            set @last_row = -1

            select @last_row = max(ds_row) from dashboard_items
            where ds_user = @userId
            and ds_col = @columnNum

            if @last_row = -1 or @last_row is null
	            set @last_row = 1
            else
	            set @last_row = @last_row + 1

            insert into dashboard_items
            (ds_user, ds_report, ds_chart_type, ds_col, ds_row)
            values (@userId, @reportId, @chartType, @columnNum, @last_row)";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@reportId", reportId);
            cmd.Parameters.AddWithValue("@chartType", chartType);
            cmd.Parameters.AddWithValue("@columnNum", dashboardColumn);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Removes an item from the dashboard. 
        /// </summary>
        /// <param name="dashboardItemId">Id of the dashboard Item you want to remove</param>
        /// <param name="userId">Id of the user requesting the delete</param>
        public void DeleteDashboardItem(int dashboardItemId, int userId)
        {
            string sql = "delete from dashboard_items where ds_id = @dashboardItem and ds_user = @userId";
            
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@dashboardItem", dashboardItemId);
            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Moves a dashboard item up or down a row
        /// </summary>
        /// <param name="dashboardItemId">Id of the dashboard item to move</param>
        /// <param name="delta">If the item is moving up or down a row. -1 moves up, 1 moves down</param>
        /// <param name="userId">Id of the user permforming the move</param>
        public void MoveDashboardItem(int dashboardItemId, int delta, int userId)
        {
            string sql = @"
                    /* swap positions */
                    declare @other_row int
                    declare @this_row int
                    declare @col int

                    select @this_row = ds_row, @col = ds_col
                    from dashboard_items
                    where ds_id = @dashboardItemId and ds_user = @userId

                    set @other_row = @this_row + @delta

                    update dashboard_items
                    set ds_row = @this_row
                    where ds_user = @userId
                    and ds_col = @col
                    and ds_row = @other_row

                    update dashboard_items
                    set ds_row = @other_row
                    where ds_user = @userId
                    and ds_id = @dashboardItemId";

            SqlCommand cmd = new SqlCommand(sql);

            cmd.Parameters.AddWithValue("@dashboardItemId", dashboardItemId);
            cmd.Parameters.AddWithValue("@delta", delta);
            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }
    }
}