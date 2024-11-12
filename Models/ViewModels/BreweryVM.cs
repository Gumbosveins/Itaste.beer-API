namespace ItbApi.Models.ViewModels
{
    public class BreweryVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lable { get; set; }
        public BreweryVM()
        {

        }
        public BreweryVM(Brewery b)
        {
            id = b.Id;
            name = b.Name;
            lable = b.Logo;
        }

        public BreweryVM(UnTappedBeerSearch.Brewery b)
        {
            id = b.brewery_id;
            name = b.brewery_name;
            lable = b.brewery_label;
        }
    }
}