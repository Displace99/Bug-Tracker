using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models
{
    public class ProjectBugVM
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Organization { get; set; }
        public string Category { get; set; }
        public string ReportedBy { get; set; }
        public DateTime ReportedDate { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}