using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
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

        /// <summary>
        /// Gets the attachment for the bug or comment and writes it directly to the HttpResponse
        /// </summary>
        /// <param name="Response">HTTP Response to write back to</param>
        /// <param name="bugPostId">Comment Id</param>
        /// <param name="bugId">Bug Id</param>
        /// <param name="filename">The filename of the attachment</param>
        /// <param name="contentType">Content Type of attachment</param>
        /// <param name="download"></param>
        public void GetAttachment(HttpResponse Response, int bugPostId,int bugId, string filename, string contentType, bool download)
        {
            // First, try to find it in the bug_post_attachments table.
            string sql = @"select bpa_content
            from bug_post_attachments
            where bpa_post = @bp_id";

            bool foundInDatabase = false;
            String foundAtPath = null;
            using (SqlCommand cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@bp_id", bugPostId);

                // Use an SqlDataReader so that we can write out the blob data in chunks.

                using (SqlDataReader reader = btnet.DbUtil.execute_reader(cmd, CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess))
                {
                    if (reader.Read()) // Did we find the content in the database?
                    {
                        foundInDatabase = true;
                    }
                    else
                    {
                        // Otherwise, try to find the content in the UploadFolder.

                        string upload_folder = Util.get_upload_folder();
                        if (upload_folder != null)
                        {
                            StringBuilder path = new StringBuilder(upload_folder);
                            path.Append("\\");
                            path.Append(bugId);
                            path.Append("_");
                            path.Append(bugPostId);
                            path.Append("_");
                            path.Append(filename);

                            if (System.IO.File.Exists(path.ToString()))
                            {
                                foundAtPath = path.ToString();
                            }
                        }
                    }

                    // We must have found the content in the database or on the disk to proceed.

                    if (!foundInDatabase && foundAtPath == null)
                    {
                        Response.Write("File not found:<br>" + filename);
                        return;
                    }

                    // Write the ContentType header.

                    if (string.IsNullOrEmpty(contentType))
                    {
                        Response.ContentType = btnet.Util.filename_to_content_type(filename);
                    }
                    else
                    {
                        Response.ContentType = contentType;
                    }


                    if (download)
                    {
                        Response.AddHeader("content-disposition", "attachment; filename=\"" + filename + "\"");
                    }
                    else
                    {
                        Response.Cache.SetExpires(DateTime.Now.AddDays(3));
                        Response.AddHeader("content-disposition", "inline; filename=\"" + filename + "\"");
                    }

                    // Write the data.

                    if (foundInDatabase)
                    {
                        long totalRead = 0;
                        long dataLength = reader.GetBytes(0, 0, null, 0, 0);
                        byte[] buffer = new byte[16 * 1024];

                        while (totalRead < dataLength)
                        {
                            long bytesRead = reader.GetBytes(0, totalRead, buffer, 0, (int)Math.Min(dataLength - totalRead, buffer.Length));
                            totalRead += bytesRead;

                            Response.OutputStream.Write(buffer, 0, (int)bytesRead);
                        }
                    }
                    else if (foundAtPath != null)
                    {
                        if (Util.get_setting("UseTransmitFileInsteadOfWriteFile", "0") == "1")
                        {
                            Response.TransmitFile(foundAtPath);
                        }
                        else
                        {
                            Response.WriteFile(foundAtPath);
                        }
                    }
                    else
                    {
                        Response.Write("File not found:<br>" + filename);
                    }

                } // end using sql reader
            } // end using sql command
        }

        /// <summary>
        /// Returns the name of the file for the specific attachmentId
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        public string GetAttachmentFileName(int attachmentId)
        {
            string sql = @"select bp_file from bug_posts where bp_id = @bugPostId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bugPostId", attachmentId);

            string filename = (string)DbUtil.execute_scalar(cmd);

            return filename;
        }

        /// <summary>
        /// Returns attachment information
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        public DataRow GetAttachmentData(int attachmentId)
        {
            string sql = @"select bp_comment, bp_file, bp_hidden_from_external_users from bug_posts where bp_id = @bugPostId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bugPostId", attachmentId);

            return DbUtil.get_datarow(cmd);
        }

        /// <summary>
        /// Returns attachments that are a file for a given bug
        /// </summary>
        /// <param name="bugId"></param>
        /// <returns></returns>
        public DataSet GetAttachmentFileInfoByBugId(int bugId)
        {
            string sql = @"select bp_id, bp_file from bug_posts where bp_type = 'file' and bp_bug = @bugId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bugId", bugId);

            return DbUtil.get_dataset(cmd);
        }

        /// <summary>
        /// Updates the attachment info in the database
        /// </summary>
        /// <param name="bugPostId"></param>
        /// <param name="comment"></param>
        /// <param name="isInternal"></param>
        public void UpdateAttachment(int bugPostId, string comment, bool isInternal)
        {
            string sql = @"update bug_posts set
			bp_comment = @comment,
			bp_hidden_from_external_users = @isInternal
			where bp_id = @bugPostId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.Parameters.AddWithValue("@isInternal", isInternal); 
            cmd.Parameters.AddWithValue("@bugPostId", bugPostId);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Removes the attachment information from the database as well as the 
        /// file folder
        /// </summary>
        /// <param name="attachmentId">Id of the attachment to delete</param>
        /// <param name="bugId">Id of the Bug attachment is on</param>
        /// <param name="filename">Attachment Filename</param>
        public void DeleteAttachment(int attachmentId, int bugId, string filename)
        {
            DeleteAttachmentFromDatabase(attachmentId);
            DeleteFileFromFolder(attachmentId, bugId, filename);
        }

        /// <summary>
        /// Deletes any attachment from the Database
        /// </summary>
        /// <param name="attachmentId"></param>
        public void DeleteAttachmentFromDatabase(int attachmentId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete bug_post_attachments where bpa_post = @bugPostId");
            sql.AppendLine("delete bug_posts where bp_id = @bugPostId");

            SqlCommand cmd = new SqlCommand(sql.ToString());
            cmd.Parameters.AddWithValue("@bugPostId", attachmentId);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Deletes the actual attachment file from the file folder.
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <param name="bugId"></param>
        /// <param name="filename"></param>
        public void DeleteFileFromFolder(int attachmentId, int bugId, string filename)
        {
            // delete the file too
            string upload_folder = Util.get_upload_folder();
            if (upload_folder != null)
            {
                StringBuilder path = new StringBuilder(upload_folder);
                path.Append("\\");
                path.Append(bugId.ToString());
                path.Append("_");
                path.Append(attachmentId.ToString());
                path.Append("_");
                path.Append(filename);
                if (System.IO.File.Exists(path.ToString()))
                {
                    System.IO.File.Delete(path.ToString());
                }
            }
        }

        /// <summary>
        /// Deletes all attachments for a list of bug Ids
        /// </summary>
        /// <param name="bugIds">A list of bug Ids</param>
        public void MassDeleteAttachments(List<int> bugIds)
        {
            string paramNames = string.Join(",", bugIds.Select(n => "@prm" + n).ToArray());

            string sql = string.Format("SELECT bp_bug, bp_id, bp_file FROM bug_posts WHERE bp_type = 'file' AND bp_bug IN ({0})", paramNames);

            SqlCommand cmd = new SqlCommand(sql.ToString());
            foreach (int n in bugIds)
            {
                cmd.Parameters.AddWithValue("@prm" + n, n);
            }

            DataSet ds = DbUtil.get_dataset(cmd);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DeleteFileFromFolder((int)dr["bp_bug"], (int)dr["bp_id"], Convert.ToString(dr["bp_file"]));
            }
        }

        /// <summary>
        /// Moves an attachment from one bug to another. This is used when merging bugs
        /// </summary>
        /// <param name="fromBugId">Bug Id the attachment is currently on</param>
        /// <param name="intoBugId">Bug Id that you want the attachment moved to</param>
        public void MoveAttachmentToOtherBug(int fromBugId, int intoBugId)
        {
            string upload_folder = Util.get_upload_folder();
            if (upload_folder != null)
            {
                DataSet ds = GetAttachmentFileInfoByBugId(fromBugId);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    // create path
                    StringBuilder path = new StringBuilder(upload_folder);
                    path.Append("\\");
                    path.Append(fromBugId.ToString());
                    path.Append("_");
                    path.Append(Convert.ToString(dr["bp_id"]));
                    path.Append("_");
                    path.Append(Convert.ToString(dr["bp_file"]));
                    if (System.IO.File.Exists(path.ToString()))
                    {
                        StringBuilder path2 = new StringBuilder(upload_folder);
                        path2.Append("\\");
                        path2.Append(intoBugId.ToString());
                        path2.Append("_");
                        path2.Append(Convert.ToString(dr["bp_id"]));
                        path2.Append("_");
                        path2.Append(Convert.ToString(dr["bp_file"]));

                        System.IO.File.Move(path.ToString(), path2.ToString());
                    }
                }
            }
        }
    }
}