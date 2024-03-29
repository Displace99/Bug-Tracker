﻿using btnet;
using BugTracker.Web.Models.Category;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

        public DataView GetCategoryListForSearch()
        {
            string sql = "select ct_id, ct_name from categories order by ct_sort_seq, ct_name";

            return DbUtil.get_dataview(sql);
        }

        public DataRow GetCategoryById(int Id)
        {
            string sql = @"select ct_name, ct_sort_seq, ct_default from categories where ct_id = @Id";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Id", Id);
            
            return DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="category">Category Model</param>
        public void CreateCategory(CategoryVM category)
        {
            string sql = "insert into categories (ct_name, ct_sort_seq, ct_default) values (@Name, @SortSeq, @IsDefault)";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Name", category.Name);
            cmd.Parameters.AddWithValue("@SortSeq", category.SortSequence);
            cmd.Parameters.AddWithValue("@IsDefault", category.IsDefault);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <param name="category">Category Model</param>
        public void UpdateCategory(CategoryVM category)
        {
            string sql = "update categories set ct_name = @Name, ct_sort_seq = @SortSeq, ct_default = @IsDefault where ct_id = @Id";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Name", category.Name);
            cmd.Parameters.AddWithValue("@SortSeq", category.SortSequence);
            cmd.Parameters.AddWithValue("@IsDefault", category.IsDefault);
            cmd.Parameters.AddWithValue("@Id", category.Id);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Deletes a Category from the database
        /// </summary>
        /// <param name="Id">Id of the category to delete</param>
        public void DeleteCategory(int Id)
        {
            string sql = "delete categories where ct_id = @Id";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Id", Id);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Gets a count of bugs tied to a specific category.
        /// Used when attempting to delete a category
        /// </summary>
        /// <param name="categoryId">Category Id to get count for</param>
        /// <returns>Count of bugs tied to category</returns>
        public int GetBugCountByCategory(int categoryId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("declare @cnt int");
            sql.AppendLine("select @cnt = count(1) from bugs where bg_category = @categoryId");
            sql.AppendLine("select ct_name, @cnt [cnt] from categories where ct_id = @categoryId");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("@categoryId", categoryId);

            DataRow dr = DbUtil.get_datarow(cmd);

            return (int)dr["cnt"];
        }
    }
}