using BugTracker.Web.Models.Organization;
using BugTracker.Web.Models.Priority;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Lucene.Net.Index.CheckIndex;
using System.Web.UI.WebControls;
using btnet;
using System.Data;

namespace BugTracker.Web.Models.Search
{
    public class ExtendedSearch
    {
        public Security Security { get; set; }
        public DataSet CustomColumns { get; set; }
        public Dictionary<int, BtnetProject> MapProjects { get; set; }


        public List<ListItem> ReportedByList { get; set; }
        public List<ListItem> AssignedToList { get; set; }
        public List<ListItem> ProjectList { get; set; }
        public List<ListItem> ProjectCustomDD1List { get; set; }
        public List<ListItem> ProjectCustomDD2List { get; set; }
        public List<ListItem> ProjectCustomDD3List { get; set; }
        public List<ListItem> OrgList { get; set; }
        public List<ListItem> CategoryList { get; set; }
        public List<ListItem> PriorityList { get; set; }
        public List<ListItem> StatusList { get; set; }
        public List<ListItem> UDFList { get; set; }

        public string DescriptionContainsFilter { get; set; }
        public string CommentsContainsFilter { get; set; }

        public bool UseFullNames { get; set; }
        public bool IsExternalUser { get; set; }
        public string CommentSinceDateString { get; set; }
        public string FromDateString { get; set; }
        public string ToDateString { get; set;}
        public string LastUpdatedFromDateString { get; set; }
        public string LastUpdatedToDateString { get; set; }

    }
}