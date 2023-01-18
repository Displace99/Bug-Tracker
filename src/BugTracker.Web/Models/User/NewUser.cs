using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.User
{
    public class NewUser
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int BugsPerPage { get; set; }
        public bool UseFckEditor { get; set; }
        public bool EnablePopups { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool EnableNotifications { get; set; }
        public bool SendToSelf { get; set; }
        public int ReportedNotifications { get; set; }
        public int AssignedNotifications { get; set; }
        public int SubscribedNotifications { get; set; }
        public bool AutoSubscribe { get; set; }
        public bool AutoSubscribeOwn { get; set; }
        public bool AutoSubscribeReported { get; set; }
        public int DefaultQueryId { get; set; }
        public int OrginizationId { get; set; }
        public string Signature { get; set; }
        public int ForcedProjectId { get; set; }
        public int CreatedById { get; set; }
    }
}