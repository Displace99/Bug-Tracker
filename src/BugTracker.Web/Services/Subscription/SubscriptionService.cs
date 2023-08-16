using btnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
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

        public void AddNewSubscriber(int bugId, int userId)
        {
            //TODO: Instead of deleting and then adding a new one, we should check to see if it
            //exists first before trying to delete or add.
            DeleteSubscription(bugId, userId);
            AddSubscription(bugId, userId);
        }

        public DataSet GetSubscriberList(int bugId, string session, bool isAdmin)
        {
            string sql;
            // show who is subscribed
            if (isAdmin)
            {
                sql = @"
					select
					'<a href=delete_subscriber.aspx?ses=$ses&bg_id=$bg&us_id=' + convert(varchar,us_id) + '>unsubscribe</a>'	[$no_sort_unsubscriber],
					us_username [user],
					us_lastname + ', ' + us_firstname [name],
					us_email [email],
					case when us_reported_notifications < 4 or us_assigned_notifications < 4 or us_subscribed_notifications < 4 then 'Y' else 'N' end [user is<br>filtering<br>notifications]
					from bug_subscriptions
					inner join users on bs_user = us_id
					where bs_bug = @bg
					and us_enable_notifications = 1
					and us_active = 1
					order by 1";

                sql = sql.Replace("$ses", session);
            }
            else
            {
                sql = @"
					select
					us_username [user],
					us_lastname + ', ' + us_firstname [name],
					us_email [email],
					case when us_reported_notifications < 4 or us_assigned_notifications < 4 or us_subscribed_notifications < 4 then 'Y' else 'N' end [user is<br>filtering<br>notifications]
					from bug_subscriptions
					inner join users on bs_user = us_id
					where bs_bug = @bg
					and us_enable_notifications = 1
					and us_active = 1
					order by 1";
            }

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bg", bugId);

            return DbUtil.get_dataset(cmd);
        }

        public DataView GetAvailableSubscribers(int bugId)
        {
            string sql = @"
				declare @project int;
				declare @org int;
				select @project = bg_project, @org = bg_org from bugs where bg_id = @bg;";

            // Only users explicitly allowed will be listed
            if (Util.get_setting("DefaultPermissionLevel", "2") == "0")
            {
                sql += @"select us_id, case when $fullnames then us_lastname + ', ' + us_firstname else us_username end us_username
					from users
					where us_active = 1
					and us_enable_notifications = 1
					and us_id in
						(select pu_user from project_user_xref
						where pu_project = @project
						and pu_permission_level <> 0)
					and us_id not in (
						select us_id
						from bug_subscriptions
						inner join users on bs_user = us_id
						where bs_bug = @bg
						and us_enable_notifications = 1
						and us_active = 1)
					and us_id not in (
						select us_id from users
						inner join orgs on us_org = og_id
						where us_org <> @org
						and og_other_orgs_permission_level = 0)

					order by us_username; ";
            }
            // Only users explictly DISallowed will be omitted
            else
            {
                sql += @"select us_id, case when $fullnames then us_lastname + ', ' + us_firstname else us_username end us_username
					from users
					where us_active = 1
					and us_enable_notifications = 1
					and us_id not in
						(select pu_user from project_user_xref
						where pu_project = @project
						and pu_permission_level = 0)
					and us_id not in (
						select us_id
						from bug_subscriptions
						inner join users on bs_user = us_id
						where bs_bug = @bg
						and us_enable_notifications = 1
						and us_active = 1)
					and us_id not in (
						select us_id from users
						inner join orgs on us_org = og_id
						where us_org <> @org
						and og_other_orgs_permission_level = 0)
					order by us_username; ";
            }

            if (Util.get_setting("UseFullNames", "0") == "0")
            {
                // false condition
                sql = sql.Replace("$fullnames", "0 = 1");
            }
            else
            {
                // true condition
                sql = sql.Replace("$fullnames", "1 = 1");
            }

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@bg", bugId);

            //Populating control
            return DbUtil.get_dataview(cmd);
        }


    }
}