﻿using btnet;
using BugTracker.Web.Models.CustomFields;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml.Linq;

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

        /// <summary>
        /// Returns the details of a specific custom field
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DataRow GetFieldDetails(int Id)
        {
            string sql = @"
                select sc.name,
                isnull(ccm_dropdown_vals,'') [vals],
                isnull(ccm_dropdown_type,'') [dropdown_type],
                isnull(ccm_sort_seq, sc.colorder) [column order],
                mm.text [default value], dflts.name [default name]
                from syscolumns sc
                inner join sysobjects so on sc.id = so.id
                left outer join custom_col_metadata ccm on ccm_colorder = sc.colorder
                left outer join syscomments mm on sc.cdefault = mm.id
                left outer join sysobjects dflts on dflts.id = mm.id
                where so.name = 'bugs'
                and sc.colorder = @Id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Id", Id);

            return DbUtil.get_datarow(cmd);
        }

        public void DeleteField(int Id)
        {
            DataRow columnDR = GetColumns(Id);

            string columnName = (string)columnDR["column_name"];
            string constraintName = columnDR["default_constraint_name"].ToString();

            // if there is a default, delete it
            if (constraintName != "")
            {
                DeleteColumnConstraint(constraintName);
            }

            DeleteColumn(columnName);
            DeleteRow(Id);
        }

        /// <summary>
        /// Gets a column name from the column Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Name of the column</returns>
        public string GetColumnName(int Id)
        {
            string sql = @"select sc.name
			from syscolumns sc
			inner join sysobjects so on sc.id = so.id
			left outer join sysobjects df on df.id = sc.cdefault
			where so.name = 'bugs'
			and sc.colorder = @Id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Id", Id);
            
            DataRow dr = DbUtil.get_datarow(cmd);

            string columnName = string.Empty;

            if (dr != null)
            {
                columnName = Convert.ToString(dr["name"]);
            }

            return columnName;
        }

        public void UpdateCustomFieldMetaData(CustomField customField)
        {
            //I don't think the insert will ever be hit, as this sql will only be called on update,
            //and in order to update you have to have info in the table already.
            string sql = @"declare @count int
			    select @count = count(1) from custom_col_metadata
			    where ccm_colorder = @colOrder

			    if @count = 0
				    insert into custom_col_metadata
				    (ccm_colorder, ccm_dropdown_vals, ccm_sort_seq, ccm_dropdown_type)
				    values(@colOrder, @dropdownValues, @sortSeq, @dropdownType)
			    else
				    update custom_col_metadata
				    set ccm_dropdown_vals = @dropdownValues,
				    ccm_sort_seq = @sortSeq
				    where ccm_colorder = @colOrder";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@colOrder", customField.Id);
            cmd.Parameters.AddWithValue("@dropdownValues", customField.DropdownValues);
            cmd.Parameters.AddWithValue("@sortSeq", customField.SortSequence);
            cmd.Parameters.AddWithValue("@dropdownType", customField.DropdownType);

            DbUtil.execute_nonquery(cmd);

            if (customField.DefaultValue != customField.OldDefaultValue)
            {
                if (customField.OldDefaultValue != "")
                {
                    sql = "alter table bugs drop constraint [" + customField.OldDefaultValue.Replace("'", "''") + "]";
                    DbUtil.execute_nonquery(sql);
                }

                if (customField.DefaultValue != "")
                {
                    sql = "alter table bugs add constraint [" + Guid.NewGuid().ToString() + "] default " + customField.DefaultValue.Replace("'", "''") + " for [" + customField.Name + "]";
                    DbUtil.execute_nonquery(sql);
                }
            }

            _context.Application["custom_columns_dataset"] = null;
        }

        /// <summary>
        /// Adds a custom field to the database.
        /// </summary>
        /// <param name="customField"></param>
        /// <returns></returns>
        public bool AddCustomField(NewCustomFieldVM customField)
        {
            //Because this is altering tables and columns we are unable to parameterize this and will need to 
            //stay with the string replace.
            string sql = @"
                    alter table orgs add [og_$nm_field_permission_level] int null
                    alter table bugs add [$nm] $dt $ln $null $df";

            sql = sql.Replace("$nm", customField.Name);
            sql = sql.Replace("$dt", customField.DataType);

            if (customField.FieldLength != "")
            {
                if (customField.FieldLength.StartsWith("("))
                {
                    sql = sql.Replace("$ln", customField.FieldLength);
                }
                else
                {
                    sql = sql.Replace("$ln", "(" + customField.FieldLength + ")");
                }
            }
            else
            {
                sql = sql.Replace("$ln", "");
            }

            if (customField.DefaultValue != "")
            {
                //Why do we have to wrap the default value in parenthesis?
                if (customField.DefaultValue.StartsWith("("))
                {
                    sql = sql.Replace("$df", "DEFAULT " + customField.DefaultValue);
                }
                else
                {
                    sql = sql.Replace("$df", "DEFAULT (" + customField.DefaultValue + ")");
                }
            }
            else
            {
                sql = sql.Replace("$df", "");
            }

            if (customField.IsRequired)
            {
                sql = sql.Replace("$null", "NOT NULL");
            }
            else
            {
                sql = sql.Replace("$null", "NULL");
            }

            bool alter_table_worked = false;
            try
            {
                DbUtil.execute_nonquery(sql);
                alter_table_worked = true;
            }
            catch
            {
                alter_table_worked = false;
            }

            if (alter_table_worked)
            {
                sql = @"declare @colorder int

				select @colorder = sc.colorder
				from syscolumns sc
				inner join sysobjects so on sc.id = so.id
				where so.name = 'bugs'
				and sc.name = '$nm'

				insert into custom_col_metadata
				(ccm_colorder, ccm_dropdown_vals, ccm_sort_seq, ccm_dropdown_type)
				values(@colorder, N'$v', $ss, '$dt')";


                sql = sql.Replace("$nm", customField.Name);
                sql = sql.Replace("$v", customField.DropDownValues.Replace("'", "''"));
                sql = sql.Replace("$ss", customField.SortSequence.ToString());
                sql = sql.Replace("$dt", customField.DropDownType.Replace("'", "''"));

                DbUtil.execute_nonquery(sql);
            }
            _context.Application["custom_columns_dataset"] = null;
            return alter_table_worked;
        }

        //****** Specific Methods for delete column workflow ******
        #region Delete Helper Methods
        private DataRow GetColumns(int Id)
        {
            string sql = @"select sc.name [column_name], df.name [default_constraint_name]
			from syscolumns sc
			inner join sysobjects so on sc.id = so.id
			left outer join sysobjects df on df.id = sc.cdefault
			where so.name = 'bugs'
			and sc.colorder = @Id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Id", Id);
            
            return DbUtil.get_datarow(cmd);
        }

        private void DeleteColumnConstraint(string constraintName)
        {
            string sql = @"alter table bugs drop constraint [$df]";
            sql = sql.Replace("$df", constraintName);
            DbUtil.execute_nonquery(sql);
        }

        private void DeleteColumn(string columnName)
        {
            string sql = @"
                alter table orgs drop column [og_$nm_field_permission_level]
                alter table bugs drop column [$nm]";

            //We have to keep the text replace
            //Trying to parameterize the column name doesn't work, but also doesn't throw any errors.
            sql = sql.Replace("$nm",columnName);
            DbUtil.execute_nonquery(sql);
        }

        private void DeleteRow(int Id)
        {
            //delete row from custom column table
            string sql = @"delete from custom_col_metadata where ccm_colorder = @Id";
            
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Id", Id);

            _context.Application["custom_columns_dataset"] = null;
            DbUtil.execute_nonquery(cmd);
        }
        #endregion
    }
}