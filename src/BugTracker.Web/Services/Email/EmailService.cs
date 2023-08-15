using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services.Email
{
    public class EmailService
    {
		/// <summary>
		/// Returns Bug and Project information based on the comment Id. 
		/// This is used when we email a specific comment to someone
		/// </summary>
		/// <param name="commentId">Id of comment </param>
		/// <param name="userId">Id of user who is trying to send email</param>
		/// <returns></returns>
        public DataView GetBugInfoByCommentId(int commentId, int userId)
        {
            string sql = @"select
				bp_parent,
                bp_file,
                bp_id,
				bg_id,
				bg_short_desc,
				bp_email_from,
				bp_comment,
				bp_email_from,
				bp_date,
				bp_type,
                bp_content_type,
				bg_project,
                bp_hidden_from_external_users,
				isnull(us_signature,'') [us_signature],
				isnull(pj_pop3_email_from,'') [pj_pop3_email_from],
				isnull(us_email,'') [us_email],
				isnull(us_firstname,'') [us_firstname],
				isnull(us_lastname,'') [us_lastname]				
				from bug_posts
				inner join bugs on bp_bug = bg_id
				inner join users on us_id = @userId
				left outer join projects on bg_project = pj_id
				where bp_id = @commentId
				or (bp_parent = @commentId and bp_type='file')";

			SqlCommand cmd = new SqlCommand(sql);
			cmd.Parameters.AddWithValue("@userId", userId);
			cmd.Parameters.AddWithValue("@commentId", commentId);

            return DbUtil.get_dataview(cmd);
        }

		public DataRow GetBugInfoByBugId(int bugId, int userId)
		{
            string sql = @"select
				bg_short_desc,
				bg_project,
				isnull(us_signature,'') [us_signature],
				isnull(us_email,'') [us_email],
				isnull(us_firstname,'') [us_firstname],
				isnull(us_lastname,'') [us_lastname],
				isnull(pj_pop3_email_from,'') [pj_pop3_email_from]
				from bugs
				inner join users on us_id = @userId
				left outer join projects on bg_project = pj_id
				where bg_id = @bugId";

			SqlCommand cmd = new SqlCommand(sql);
			cmd.Parameters.AddWithValue("@userId", userId);
			cmd.Parameters.AddWithValue("@bugId", bugId);
            
            return DbUtil.get_datarow(cmd);
        }
    }
}