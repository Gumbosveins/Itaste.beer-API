namespace ItbApi.Models.ViewModels
{
    public class ReviewTypeForCreateVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public bool isNew { get; set; }
        public ReviewTypeForCreateVM(ReviewType t)   
        {
            id = t.Id;
            name = t.Name;
            desc = "";
            isNew = false;
        }

        public ReviewTypeForCreateVM()
        {
            // TODO: Complete member initialization
        }
    }
}