using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BugTracker.Web.Models.Comment
{
    public class AddEmailComment
    {
        public int BugId { get; set; }
        public int UserId { get; set; }

        public string Comment { get; set; }
        public string CommentSearch { get; set; }
        public string ContentType { get; set; }

        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailCC { get; set; }
        
        
    }
}