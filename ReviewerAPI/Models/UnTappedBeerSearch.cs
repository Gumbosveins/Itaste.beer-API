using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewerAPI.Models
{
    public class UnTappedBeerSearch
    {
        public class ResponseTime
        {
            public double time { get; set; }
            public string measure { get; set; }
        }

        public class InitTime
        {
            public double time { get; set; }
            public string measure { get; set; }
        }

        public class Meta
        {
            public int code { get; set; }
            public ResponseTime response_time { get; set; }
            public InitTime init_time { get; set; }
        }

        public class InProduction
        {
            public string order { get; set; }
        }

        public class Popularity
        {
            public string order { get; set; }
        }

        public class Sort
        {
            public InProduction in_production { get; set; }
            public Popularity popularity { get; set; }
        }

        public class QueryString
        {
            public string query { get; set; }
            public List<string> fields { get; set; }
        }

        public class Query2
        {
            public QueryString query_string { get; set; }
        }

        public class Query
        {
            public Sort sort { get; set; }
            public int size { get; set; }
            public int from { get; set; }
            public Query2 query { get; set; }
        }

        public class Beer
        {
            public int bid { get; set; }
            public string beer_name { get; set; }
            public string beer_label { get; set; }
            public double beer_abv { get; set; }
            public string beer_slug { get; set; }
            public int beer_ibu { get; set; }
            public string beer_description { get; set; }
            public string created_at { get; set; }
            public string beer_style { get; set; }
            public int auth_rating { get; set; }
            public bool wish_list { get; set; }
            public int in_production { get; set; }
        }

        public class Contact
        {
            public string twitter { get; set; }
            public string facebook { get; set; }
            public string instagram { get; set; }
            public string url { get; set; }
        }

        public class Location
        {
            public string brewery_city { get; set; }
            public string brewery_state { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Brewery
        {
            public int brewery_id { get; set; }
            public string brewery_name { get; set; }
            public string brewery_slug { get; set; }
            public string brewery_label { get; set; }
            public string country_name { get; set; }
            public Contact contact { get; set; }
            public Location location { get; set; }
            public int brewery_active { get; set; }
        }

        public class Item
        {
            public int checkin_count { get; set; }
            public bool have_had { get; set; }
            public int your_count { get; set; }
            public Beer beer { get; set; }
            public Brewery brewery { get; set; }
        }

        public class Beers
        {
            public int count { get; set; }
            public List<Item> items { get; set; }
        }

        public class Beer2
        {
            public int bid { get; set; }
            public string beer_name { get; set; }
            public string beer_label { get; set; }
            public double beer_abv { get; set; }
            public string beer_slug { get; set; }
            public int beer_ibu { get; set; }
            public string beer_description { get; set; }
            public string created_at { get; set; }
            public string beer_style { get; set; }
            public int auth_rating { get; set; }
            public bool wish_list { get; set; }
            public int in_production { get; set; }
        }

        public class Contact2
        {
            public string twitter { get; set; }
            public string facebook { get; set; }
            public string instagram { get; set; }
            public string url { get; set; }
        }

        public class Location2
        {
            public string brewery_city { get; set; }
            public string brewery_state { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Brewery2
        {
            public int brewery_id { get; set; }
            public string brewery_name { get; set; }
            public string brewery_slug { get; set; }
            public string brewery_label { get; set; }
            public string country_name { get; set; }
            public Contact2 contact { get; set; }
            public Location2 location { get; set; }
            public int brewery_active { get; set; }
        }

        public class Item2
        {
            public int checkin_count { get; set; }
            public bool have_had { get; set; }
            public int your_count { get; set; }
            public Beer2 beer { get; set; }
            public Brewery2 brewery { get; set; }
        }

        public class Homebrew
        {
            public int count { get; set; }
            public List<Item2> items { get; set; }
        }

        public class Location3
        {
            public string brewery_city { get; set; }
            public string brewery_state { get; set; }
            public int lat { get; set; }
            public int lng { get; set; }
        }

        public class Brewery3
        {
            public int brewery_id { get; set; }
            public int beer_count { get; set; }
            public string brewery_name { get; set; }
            public string brewery_slug { get; set; }
            public string brewery_label { get; set; }
            public string country_name { get; set; }
            public Location3 location { get; set; }
        }

        public class Item3
        {
            public Brewery3 brewery { get; set; }
        }

        public class Breweries
        {
            public List<Item3> items { get; set; }
            public int count { get; set; }
        }

        public class Response
        {
            public Query query { get; set; }
            public string message { get; set; }
            public bool brewery_id { get; set; }
            public string search_type { get; set; }
            public int type_id { get; set; }
            public int search_version { get; set; }
            public int found { get; set; }
            public int offset { get; set; }
            public int limit { get; set; }
            public string term { get; set; }
            public string parsed_term { get; set; }
            public Beers beers { get; set; }
            public Homebrew homebrew { get; set; }
            public Breweries breweries { get; set; }
        }

        public class Root
        {
            public Meta meta { get; set; }
            public List<object> notifications { get; set; }
            public Response response { get; set; }
        }
    }
}