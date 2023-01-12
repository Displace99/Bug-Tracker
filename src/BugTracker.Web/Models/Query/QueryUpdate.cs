using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.Query
{
    public class QueryUpdate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SqlText { get; set; }
        public int SelectedUserId { get; set; }
        public int SelectedOrgId { get; set; }
    }
}