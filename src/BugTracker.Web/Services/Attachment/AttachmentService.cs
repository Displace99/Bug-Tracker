using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
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
    }
}