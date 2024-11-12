namespace ItbApi.Models.ViewModels
{
    public class BeerVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lableSM { get; set; }
        public string lableMed { get; set; }
        public decimal IBU { get; set; }
        public decimal percent { get; set; }
        public string description { get; set; }
        public decimal untappedRating { get; set; }
        public BreweryVM brewery { get; set; }
        public string type { get; set; }

        public BeerVM(Beverage beer)
        {
            id = beer.Id;
            name = beer.Name;
            lableSM = beer.LableSm;
            lableMed = beer.LableMed;
            IBU = beer.IBU;
            percent = beer.AlcoholPercentage;
            description = beer.Description;
            untappedRating = beer.UtappedRating;
            brewery = new BreweryVM(beer.Brewery);
            type = beer.BeverageType.Name;
        }
        public BeerVM(UnTappedBeerSearch.Item beer)
        {
            id = beer.beer.bid;
            name = beer.beer.beer_name;
            lableSM = beer.beer.beer_label;
            IBU = beer.beer.beer_ibu;
            percent = Convert.ToDecimal(beer.beer.beer_abv);
            description = beer.beer.beer_description;
            untappedRating = -1;
            brewery = new BreweryVM(beer.brewery);
            type = beer.beer.beer_style;
            
        }
    }
}