using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services.UserDefinedFields
{
    public class UserDefinedFieldService
    {
        public DataSet GetFieldList()
        {
			string sql = @"select udf_id [id], udf_name [user defined attribute value], udf_sort_seq [sort seq],
		        case when udf_default = 1 then 'Y' else 'N' end [default],
		        udf_id [hidden]
		        from user_defined_attribute order by udf_name";

            return DbUtil.get_dataset(sql);
        }
    }
}