using btnet;
using System;
using System.Collections.Generic;
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
    }
}