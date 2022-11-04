using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.Registration
{
    /// <summary>
    /// User who has registered, but not verified or completed registration process
    /// </summary>
    public class RegisteredUser
    {
        public string Id { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Action { get; set; }
        
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public string Salt { get; set; }
        public string Password { get; set; }
        
        
    }
}