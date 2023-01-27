using btnet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace BugTracker.Web.Services.Bug
{
    public class BugSubscriptionService
    {
        public void DeleteSubscriptionsForUser(int userId)
        {
            string sql = "delete from bug_subscriptions where bs_user = @userId;";

            SqlCommand cmd = new SqlCommand(sql);

            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }

        public void AddAutoSubscribeBugs(int userId)
        {
            string sql = "insert into bug_subscriptions(bs_bug, bs_user) select bg_id, @userId from bugs;";

            SqlCommand cmd = new SqlCommand(sql);

            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }

        public void AddAutoSubscribeReportedBugs(int userId)
        {
            StringBuilder sql = new StringBuilder();

			sql.AppendLine("insert into bug_subscriptions (bs_bug, bs_user)");
			sql.AppendLine("select bg_id, @userId from bugs where bg_reported_user = @userId");
			sql.AppendLine("and bg_id not in (select bs_bug from bug_subscriptions where bs_user = @userId);");


            SqlCommand cmd = new SqlCommand(sql.ToString());

            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }

        public void AddAutoSubscribeOwnBugs(int userId)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("insert into bug_subscriptions(bs_bug, bs_user)");
            sql.AppendLine("select bg_id, @userId from bugs where bg_assigned_to_user = @userId");
            sql.AppendLine("and bg_id not in (select bs_bug from bug_subscriptions where bs_user = @userId);");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }

        public void AddAutoSubscribeProjects(int userId, List<int> projectIds)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("insert into bug_subscriptions(bs_bug, bs_user)");
            sql.AppendLine(string.Format("select bg_id, $us from bugs where bg_project in ({0})", string.Join(",", projectIds.Select(n => "@prm"+n).ToArray())));
			sql.AppendLine("and bg_id not in (select bs_bug from bug_subscriptions where bs_user = @userId);");

            SqlCommand cmd = new SqlCommand(sql.ToString());

            foreach (int n in projectIds)
            {
                cmd.Parameters.AddWithValue("@prm"+n, n);
            }
            cmd.Parameters.AddWithValue("@userId", userId);

            DbUtil.execute_nonquery(cmd);
        }
    }
}