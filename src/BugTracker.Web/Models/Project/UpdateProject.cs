using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.Project
{
    public class UpdateProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int DefaultUserId { get; set; }
        public bool IsAutoAssign { get; set; }
        public bool IsAutoSubscribe { get; set; }
        public bool IsDefaultSelection { get; set; }
        
        public bool EnablePop3 { get; set; }
        public string Pop3UserName { get; set; }
        public string Pop3Password { get; set; }
        public string Pop3EmailFrom { get; set; }
        
        public bool EnableDropdown1 { get; set; }
        public string Dropdown1Label { get; set; }
        public string Dropdown1Values { get; set; }
        
        public bool EnableDropdown2 { get; set; }
        public string Dropdown2Label { get; set; }
        public string Dropdown2Values { get; set; }
        
        public bool EnableDropdown3 { get; set; }
        public string Dropdown3Label { get; set; }
        public string Dropdown3Values { get; set; }
    }
}