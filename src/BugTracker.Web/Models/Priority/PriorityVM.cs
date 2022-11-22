using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.Priority
{
    public class PriorityVM
    {
        public int PriorityId { get; set; }
        public string Name { get; set; }
        public string SortSequence { get; set; }
        public string BackgroundColor { get; set; }
        public string Style { get; set; }
        public bool IsDefault { get; set; }
    }
}