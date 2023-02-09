using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.UDF
{
    public class UserDefinedField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortSequence { get; set; }
        public bool IsDefault { get; set; }
    }
}