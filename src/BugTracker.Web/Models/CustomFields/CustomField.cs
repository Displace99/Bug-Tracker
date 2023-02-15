using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace BugTracker.Web.Models.CustomFields
{
    public class CustomField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DropdownValues { get; set; }
        public int SortSequence { get; set; }
        public string DefaultValue { get; set; }
        public string OldDefaultValue { get; set; }
        public string DefaultName { get; set; }
    }
}