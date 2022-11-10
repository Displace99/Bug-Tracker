using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services.Category
{
    public class CategoryService
    {
        /// <summary>
        /// Returns a list of all categories in the system
        /// </summary>
        /// <returns>DataSet of all categories</returns>
        public DataSet GetCategoryList()
        {
            string sql = @"select ct_id [id], ct_name [category], ct_sort_seq [sort seq],
		        case when ct_default = 1 then 'Y' else 'N' end [default], ct_id [hidden]
		        from categories order by ct_name";
            
            return DbUtil.get_dataset(sql);
        }
    }
}