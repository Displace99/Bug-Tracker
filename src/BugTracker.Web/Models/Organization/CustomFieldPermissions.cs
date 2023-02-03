using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.Organization
{
    /// <summary>
    /// Represents a custom field and permission level. 
    /// When a custom field is added to an org, a new column gets added to the Org table.
    /// </summary>
    public class CustomFieldPermissions
    {
        public string FieldName { get; set; }
        public int? PermissionLevel { get; set; } 
    }
}