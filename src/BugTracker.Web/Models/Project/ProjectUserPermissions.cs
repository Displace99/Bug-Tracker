using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.Project
{
    public class ProjectUserPermissions
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int PermissionLevel { get; set; }
    }
}