using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewerAPI.Models
{
    public class UnTappedBeerInfo
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

        public class Stats
        {
            public int total_count { get; set; }
            public int monthly_count { get; set; }
            public int total_user_count { get; set; }
            public int user_count { get; set; }
        }

        public class Contact
        {
            public string twitter { get; set; }
            public string facebook { get; set; }
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
        }

        public class Photo
        {
            public string photo_img_sm { get; set; }
            public string photo_img_md { get; set; }
            public string photo_img_lg { get; set; }
            public string photo_img_og { get; set; }
        }

        public class Beer2
        {
            public int bid { get; set; }
            public string beer_name { get; set; }
            public string beer_label { get; set; }
            public double beer_abv { get; set; }
            public int beer_ibu { get; set; }
            public string beer_slug { get; set; }
            public string beer_description { get; set; }
            public int is_in_production { get; set; }
            public int beer_style_id { get; set; }
            public string beer_style { get; set; }
            public int auth_rating { get; set; }
            public bool wish_list { get; set; }
            public int beer_active { get; set; }
        }

        public class Contact2
        {
            public string twitter { get; set; }
            public string facebook { get; set; }
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

        public class User
        {
            public int uid { get; set; }
            public string user_name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string user_avatar { get; set; }
            public string relationship { get; set; }
            public int is_private { get; set; }
        }

        public class Item
        {
            public int photo_id { get; set; }
            public Photo photo { get; set; }
            public string created_at { get; set; }
            public int checkin_id { get; set; }
            public Beer2 beer { get; set; }
            public Brewery2 brewery { get; set; }
            public User user { get; set; }
            public List<object> venue { get; set; }
        }

        public class Media
        {
            public int count { get; set; }
            public List<Item> items { get; set; }
        }

        public class User2
        {
            public int uid { get; set; }
            public string user_name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string location { get; set; }
            public string url { get; set; }
            public int is_supporter { get; set; }
            public string bio { get; set; }
            public object relationship { get; set; }
            public string user_avatar { get; set; }
            public int is_private { get; set; }
            public object contact { get; set; }
        }

        public class Beer3
        {
            public int bid { get; set; }
            public string beer_name { get; set; }
            public string beer_label { get; set; }
            public double beer_abv { get; set; }
            public int beer_ibu { get; set; }
            public string beer_slug { get; set; }
            public string beer_description { get; set; }
            public int is_in_production { get; set; }
            public int beer_style_id { get; set; }
            public string beer_style { get; set; }
            public int auth_rating { get; set; }
            public bool wish_list { get; set; }
            public int beer_active { get; set; }
        }

        public class Contact3
        {
            public string twitter { get; set; }
            public string facebook { get; set; }
            public string url { get; set; }
        }

        public class Location3
        {
            public string brewery_city { get; set; }
            public string brewery_state { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Brewery3
        {
            public int brewery_id { get; set; }
            public string brewery_name { get; set; }
            public string brewery_slug { get; set; }
            public string brewery_label { get; set; }
            public string country_name { get; set; }
            public Contact3 contact { get; set; }
            public Location3 location { get; set; }
            public int brewery_active { get; set; }
        }

        public class Comments
        {
            public int total_count { get; set; }
            public int count { get; set; }
            public List<object> items { get; set; }
        }

        public class Toasts
        {
            public int total_count { get; set; }
            public int count { get; set; }
            public object auth_toast { get; set; }
            public List<object> items { get; set; }
        }

        public class Media2
        {
            public int count { get; set; }
            public List<object> items { get; set; }
        }

        public class Source
        {
            public string app_name { get; set; }
            public string app_website { get; set; }
        }

        public class Badges
        {
            public int count { get; set; }
            public List<object> items { get; set; }
        }

        public class Item2
        {
            public int checkin_id { get; set; }
            public string created_at { get; set; }
            public string checkin_comment { get; set; }
            public double rating_score { get; set; }
            public User2 user { get; set; }
            public Beer3 beer { get; set; }
            public Brewery3 brewery { get; set; }
            public object venue { get; set; }
            public Comments comments { get; set; }
            public Toasts toasts { get; set; }
            public Media2 media { get; set; }
            public Source source { get; set; }
            public Badges badges { get; set; }
        }

        public class Checkins
        {
            public int count { get; set; }
            public List<Item2> items { get; set; }
        }

        public class Beer4
        {
            public int bid { get; set; }
            public string beer_name { get; set; }
            public double beer_abv { get; set; }
            public int beer_ibu { get; set; }
            public string beer_slug { get; set; }
            public string beer_style { get; set; }
            public string beer_label { get; set; }
            public int auth_rating { get; set; }
            public bool wish_list { get; set; }
        }

        public class Contact4
        {
            public string twitter { get; set; }
            public string facebook { get; set; }
            public string instagram { get; set; }
            public string url { get; set; }
        }

        public class Location4
        {
            public string brewery_city { get; set; }
            public string brewery_state { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Brewery4
        {
            public int brewery_id { get; set; }
            public string brewery_name { get; set; }
            public string brewery_slug { get; set; }
            public string brewery_label { get; set; }
            public string country_name { get; set; }
            public Contact4 contact { get; set; }
            public Location4 location { get; set; }
            public int brewery_active { get; set; }
        }

        public class Friends
        {
            public List<object> items { get; set; }
            public int count { get; set; }
        }

        public class Item3
        {
            public double rating_score { get; set; }
            public Beer4 beer { get; set; }
            public Brewery4 brewery { get; set; }
            public Friends friends { get; set; }
        }

        public class Similar
        {
            public int count { get; set; }
            public List<Item3> items { get; set; }
        }

        public class Friends2
        {
            public List<object> items { get; set; }
            public int count { get; set; }
        }

        public class Vintages
        {
            public int count { get; set; }
            public List<object> items { get; set; }
        }

        public class Beer
        {
            public int bid { get; set; }
            public string beer_name { get; set; }
            public string beer_label { get; set; }
            public string beer_label_hd { get; set; }
            public double beer_abv { get; set; }
            public int beer_ibu { get; set; }
            public string beer_description { get; set; }
            public string beer_style { get; set; }
            public int is_in_production { get; set; }
            public string beer_slug { get; set; }
            public int is_homebrew { get; set; }
            public string created_at { get; set; }
            public int rating_count { get; set; }
            public double rating_score { get; set; }
            public Stats stats { get; set; }
            public Brewery brewery { get; set; }
            public int auth_rating { get; set; }
            public bool wish_list { get; set; }
            public Media media { get; set; }
            public Checkins checkins { get; set; }
            public Similar similar { get; set; }
            public Friends2 friends { get; set; }
            public double weighted_rating_score { get; set; }
            public Vintages vintages { get; set; }
        }

        public class Response
        {
            public Beer beer { get; set; }
        }

        public class Root
        {
            public Meta meta { get; set; }
            public List<object> notifications { get; set; }
            public Response response { get; set; }
        }
    }
}