using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.User
{
    public class PasswordResetUser
    {
        public string Id { get; set; }
        public DateTime SentDate { get; set; }
        public string Action { get; set; }

        public int UserId { get; set; }
        public string Email { get; set; }
    }
}