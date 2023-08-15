using btnet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Services.Subscription
{
    public class SubscriptionService
    {
        /// <summary>
        /// Adds the user as subscribed to the bug
        /// </summary>
        /// <param name="bugId">Bug Id to subscribe to</param>
        /// <param name="userId">User to subscribe</param>
        public void AddSubscription(int bugId, int userId)
        {
            string sql = "INSERT INTO bug_subscriptions (bs_bug, bs_user) VALUES (@bg, @us)";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bg", bugId);
            cmd.Parameters.AddWithValue("@us", userId);

            DbUtil.execute_nonquery(cmd);
        }

        /// <summary>
        /// Removes a user from a subscribed bug
        /// </summary>
        /// <param name="bugId">Bug Id to subscribe to</param>
        /// <param name="userId">User to subscribe</param>
        public void DeleteSubscription(int bugId, int userId)
        {
            string sql = "DELETE FROM bug_subscriptions WHERE bs_bug = @bg AND bs_user = @us";
            
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bg", bugId);
            cmd.Parameters.AddWithValue("@us", userId);

            DbUtil.execute_nonquery(cmd);
        }


    }
}