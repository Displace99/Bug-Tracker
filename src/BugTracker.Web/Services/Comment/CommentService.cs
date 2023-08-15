using btnet;
using BugTracker.Web.Models.Comment;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BugTracker.Web.Services.Comment
{
    public class CommentService
    {
        /// <summary>
        /// Adds a comment to the database
        /// </summary>
        /// <param name="comment">what you want the comment to say</param>
        /// <param name="userId">Id of the person adding the comment</param>
        /// <param name="bugId">Id of the bug the comment is attached to</param>
        /// <returns>Returns the id of the newly added comment</returns>
        public int AddComment(string comment, int userId, int bugId)
        {
            string sql = @"insert into bug_posts
			(bp_bug, bp_user, bp_date, bp_type, bp_comment, bp_comment_search)
			values(@bugId,@userId,getdate(), 'comment', @comment, @comment)
			select scope_identity()";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bugId", bugId);
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.Parameters.AddWithValue("@userId", userId);

            int commentId = Convert.ToInt32(DbUtil.execute_scalar(cmd));

            return commentId;
        }

        /// <summary>
        /// This updates the comments when we merge bugs
        /// </summary>
        /// <param name="fromBugId">Id of the bug we were coming from</param>
        /// <param name="commentId">Id of the comment</param>
        public void UpdateMergeComments(int fromBugId, int commentId)
        {
            string sql = @"update bug_posts
			set bp_comment = convert(nvarchar,bp_comment) + char(10) + bg_short_desc
			from bugs where bg_id = @fromBugId
			and bp_id = @commentId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@fromBugId", fromBugId);
            cmd.Parameters.AddWithValue("@commentId", commentId);

            DbUtil.execute_nonquery(cmd);
        }

        public int AddEmailSentComment(AddEmailComment emailComment)
        {
            string sql = @"
                insert into bug_posts
	                (bp_bug, bp_user, bp_date, bp_comment, bp_comment_search, bp_email_from, bp_email_to, bp_type, bp_content_type, bp_email_cc)
	                values(@bugId, @userId, getdate(), @comment, @commentSearch, @emailFrom,  @emailTo, 'sent', @contentType, @emailCC);
                select scope_identity()
                update bugs set
	                bg_last_updated_user = @userId,
	                bg_last_updated_date = getdate()
	                where bg_id = @bugId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bugId", emailComment.BugId);
            cmd.Parameters.AddWithValue("@userId", emailComment.UserId);
            cmd.Parameters.AddWithValue("@comment", emailComment.Comment);
            cmd.Parameters.AddWithValue("@commentSearch", emailComment.CommentSearch);
            cmd.Parameters.AddWithValue("@emailFrom", emailComment.EmailFrom);
            cmd.Parameters.AddWithValue("@emailTo", emailComment.EmailTo);
            cmd.Parameters.AddWithValue("@contentType", emailComment.ContentType);
            cmd.Parameters.AddWithValue("@emailCC", emailComment.EmailCC);

            return Convert.ToInt32(DbUtil.execute_scalar(cmd));
        }
    }
}