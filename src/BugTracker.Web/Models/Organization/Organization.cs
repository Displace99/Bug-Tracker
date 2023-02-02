using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.Organization
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool NonAdminsCanUse { get; set; }
        public bool ExternalUser { get; set; }
        public bool CanBeAssignedTo { get; set; }
        public bool IsActive { get; set; }
        
        //Site Wide Permissions - Org Wide
        public bool CanOnlySeeOwnReportedBugs { get; set; }
        public bool CanEditSQL { get; set; }
        public bool CanDeleteBug { get; set; }
        public bool CanEditDeleteComment { get; set; }
        public bool CanMergeBugs { get; set; }
        public bool CanMassEditBugs { get; set; }
        public bool CanUseReports { get; set;}
        public bool CanEditReports { get; set; }
        public bool CanViewTasks { get; set; }
        public bool CanEditTasks { get; set; }
        public bool CanSearch { get; set; }
        public bool CanAssignToInternalUsers { get; set; }

        //Field Permission Levels - Org Wide
        /// <summary>
        /// Permission level for bugs associated with other (or no) organizations
        /// </summary>
        public int OtherOrgPermission { get; set; }
        public int CategoryFieldPermission { get; set; }
        public int PriorityFieldPermission { get; set; }
        public int AssignedToFieldPermission { get; set; }
        public int StatusFieldPermission { get; set; }
        public int ProjectFieldPermission { get; set; }
        public int OrgFieldPermission { get; set; }
        public int UserDefinedFieldPermission { get; set; }
        public int TagFieldPermission { get; set; }

        /// <summary>
        /// This is for custom user created Org fields.
        /// </summary>
        public List<CustomFieldPermissions> CustomFieldPermissions { get; set; } = new List<CustomFieldPermissions>();
    }
}