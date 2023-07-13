using btnet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

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
    }
}