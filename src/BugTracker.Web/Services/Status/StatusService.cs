using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;
using System.Text;
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

        public DataRow GetStatusById(int Id)
        {
            string sql = @"select st_name, st_sort_seq, st_style, st_default from statuses where st_id = @Id";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Id", Id);

            return DbUtil.get_datarow(cmd);
        }

        public void DeleteStatus(int Id)
        {
            string sql = @"delete statuses where st_id = @statusId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@statusId", Id);
            
            DbUtil.execute_nonquery(cmd);
        }

        public int GetBugCountByStatus(int Id)
        {
            string sql = "select count(1)[cnt] from bugs where bg_status = @statusId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@statusId", Id);

            DataRow dr = DbUtil.get_datarow(cmd);

            return (int)dr["cnt"];
        }
    }
}