namespace AppModel
{
    public class SearchDetail
    {
        //Bool for ExportButton to set to true/false
        public bool ExportResults { get; set; }

        //Bool for showing/hiding advanced search section
        public bool ShowAdvanced { get; set; }

        public string OrderBy { get; set; }

        public int PageNumber { get; set; }
    }
}
