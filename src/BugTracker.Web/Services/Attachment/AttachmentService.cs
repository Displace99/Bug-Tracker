using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services.Attachment
{
    public class AttachmentService
    {
        /// <summary>
        /// This checks to see if there is an attached file
        /// </summary>
        /// <param name="bugPostId">ID of the bug Post</param>
        /// <param name="bugId">ID of the bug</param>
        /// <returns></returns>
        public DataRow GetAttachmentInfo(int bugPostId, int bugId)
        {
            string sql = @"select bp_file, isnull(bp_content_type,'') [bp_content_type] from bug_posts where bp_id = @bugPostId and bp_bug = @bugId";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@bugPostId", bugPostId);
            cmd.Parameters.AddWithValue("@bugId", bugId);

            return DbUtil.get_datarow(cmd);
        }
    }
}