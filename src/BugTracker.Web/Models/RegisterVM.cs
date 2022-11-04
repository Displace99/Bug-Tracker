using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models
{
    public class RegisterVM
    {
        /// <summary>
        /// User name that the user is registering with
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// Password that the user is registering with
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// Email the user is registering with
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// First name the user is registering with
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name the user is registering with
        /// </summary>
        public string LastName { get; set; }
    }
}