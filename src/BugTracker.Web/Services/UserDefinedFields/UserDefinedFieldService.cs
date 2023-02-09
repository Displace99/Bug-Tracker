using btnet;
using BugTracker.Web.Models.UDF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services.UserDefinedFields
{
    public class UserDefinedFieldService
    {
        /// <summary>
        /// Get a list of User Defined Fields
        /// </summary>
        /// <returns></returns>
        public DataSet GetFieldList()
        {
			string sql = @"select udf_id [id], udf_name [user defined attribute value], udf_sort_seq [sort seq],
		        case when udf_default = 1 then 'Y' else 'N' end [default],
		        udf_id [hidden]
		        from user_defined_attribute order by udf_name";

            return DbUtil.get_dataset(sql);
        }

        /// <summary>
        /// Returns details of a specific user defined field
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataRow GetFieldDetails(int id)
        {
            string sql = "select udf_name, udf_sort_seq, udf_default from user_defined_attribute where udf_id = @fieldId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@fieldId", id);

            return DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Creates a new field value for the User Defined Field
        /// </summary>
        /// <param name="field"></param>
        public void CreateField(UserDefinedField field)
        {
            string sql = "insert into user_defined_attribute (udf_name, udf_sort_seq, udf_default) values (@name, @sortSequence, @isDefault)";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@name", field.Name);
            cmd.Parameters.AddWithValue("@sortSequence", field.SortSequence);
            cmd.Parameters.AddWithValue("@isDefault", field.IsDefault);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Updates an existing field value
        /// </summary>
        /// <param name="field"></param>
        public void UpdateField(UserDefinedField field)
        {
            string sql = @"update user_defined_attribute set
				udf_name = @name,
				udf_sort_seq = @sortSequence,
				udf_default = @isDefault
				where udf_id = @id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@id", field.Id);
            cmd.Parameters.AddWithValue("@name", field.Name);
            cmd.Parameters.AddWithValue("@sortSequence", field.SortSequence);
            cmd.Parameters.AddWithValue("@isDefault", field.IsDefault);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// returns 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DoesFieldHaveRelatedEntities(int id)
        {
            string sql = @"declare @cnt int
			select count(1) [cnt] from bugs where bg_user_defined_attribute = @fieldId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@fieldId", id);

            var dr = DbUtil.get_datarow(cmd);
            
            return (int)dr["cnt"] > 0;
        }

        /// <summary>
        /// Delete a User Defined field
        /// </summary>
        /// <param name="id"></param>
        public void DeleteField(int id)
        {
            string sql = @"delete user_defined_attribute where udf_id = @fieldId";
            
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@fieldId", id);
            
            DbUtil.execute_nonquery(cmd);
        }
    }
}