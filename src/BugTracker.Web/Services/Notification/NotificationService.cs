using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services.Notification
{
    public class NotificationService
    {
        /// <summary>
        /// Deletes all notificaitons with a status of "not sent"
        /// </summary>
        public void DeleteNotifications()
        {
            string sql = @"delete from queued_notifications where qn_status = N'not sent'";
            DbUtil.execute_nonquery(sql);
        }

        /// <summary>
        /// Updates retry count on all notificaitons with a status of "not sent"
        /// </summary>
        public void UpdateNotifications()
        {
            string sql = @"update queued_notifications set qn_retries = 0 where qn_status = N'not sent'";
            DbUtil.execute_nonquery(sql);
        }

        /// <summary>
        /// Returns a DataSet of all notifications in the notification table
        /// </summary>
        /// <returns></returns>
        public DataSet GetQueuedNotifications()
        {
            return DbUtil.get_dataset(
                    @"select
		    qn_id [id],
		    qn_date_created [date created],
		    qn_to [to],
		    qn_bug [bug],
		    qn_status [status],
		    qn_retries [retries],
		    qn_last_exception [last error]
		    from queued_notifications
		    order by id;");
        }
    }
}