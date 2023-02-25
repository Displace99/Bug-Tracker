namespace BugTracker.Web.Models.CustomFields
{
    public class NewCustomFieldVM
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string FieldLength { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public string DropDownType { get; set; }
        public string DropDownValues { get; set; }
        public int SortSequence { get; set; }
    }
}