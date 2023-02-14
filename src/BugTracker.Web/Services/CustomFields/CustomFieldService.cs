using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;

namespace BugTracker.Web.Services.CustomFields
{
    public class CustomFieldService
    {
        private HttpContext _context;

        public CustomFieldService(HttpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of custom fields, which are extra columns on the Bug Table
        /// </summary>
        /// <returns></returns>
        public DataSet GetCustomFields()
        {
            DataSet ds = (DataSet)_context.Application["custom_columns_dataset"];

            if (ds != null)
            {
                return ds;
            }
            else
            {
                ds = btnet.DbUtil.get_dataset(@"
                    /* custom columns */ select sc.name, st.[name] [datatype], 
                    case when st.[name] = 'nvarchar' or st.[name] = 'nchar' then sc.length/2 else sc.length end as [length], 
                    sc.xprec, sc.xscale, sc.isnullable,
                    mm.text [default value], 
                    dflts.name [default name], 
                    isnull(ccm_dropdown_type,'') [dropdown type],
                    isnull(ccm_dropdown_vals,'') [vals],
                    isnull(ccm_sort_seq, sc.colorder) [column order],
                    sc.colorder
                    from syscolumns sc
                    inner join systypes st on st.xusertype = sc.xusertype
                    inner join sysobjects so on sc.id = so.id
                    left outer join syscomments mm on sc.cdefault = mm.id
                    left outer join custom_col_metadata on ccm_colorder = sc.colorder
                    left outer join sysobjects dflts on dflts.id = mm.id
                    where so.name = 'bugs'
                    and st.[name] <> 'sysname'
                    and sc.name not in ('rowguid',
                    'bg_id',
                    'bg_short_desc',
                    'bg_reported_user',
                    'bg_reported_date',
                    'bg_project',
                    'bg_org',
                    'bg_category',
                    'bg_priority',
                    'bg_status',
                    'bg_assigned_to_user',
                    'bg_last_updated_user',
                    'bg_last_updated_date',
                    'bg_user_defined_attribute',
                    'bg_project_custom_dropdown_value1',
                    'bg_project_custom_dropdown_value2',
                    'bg_project_custom_dropdown_value3',
                    'bg_tags')
                    order by sc.id, isnull(ccm_sort_seq,sc.colorder)");

                _context.Application["custom_columns_dataset"] = ds;
                return ds;
            }
        }
    }
}