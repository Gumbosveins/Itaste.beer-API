using Newtonsoft.Json;
using ReviewerAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ReviewerAPI.Helpers
{
    public class UnTapped
    {
        public async Task<UnTappedBeerSearch.Root> SearchUnTapped(string query)
        {
            Uri uri = new Uri(ConfigurationManager.AppSettings["untappedUrl"]);
            var requestObj = new
            {
                client_id = ConfigurationManager.AppSettings["untappedClientId"],
                client_secret = ConfigurationManager.AppSettings["untappedSecret"],
                q = query,
                limit = 50,
                sort = "highest_rated"
            };

            HttpHandler httpHandler = new HttpHandler();
            uri = httpHandler.CreateQueryFromObject(uri, "search/beer", requestObj);
            HttpResponseMessage response = await httpHandler.GetAsync(uri);

            return JsonConvert.DeserializeObject<UnTappedBeerSearch.Root>(Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync()));
        }

        public async Task<UnTappedBeerInfo.Root> GetBeerInfo(int beerId)
        {
            Uri uri = new Uri(ConfigurationManager.AppSettings["untappedUrl"]);
            var requestObj = new
            {
                client_id = ConfigurationManager.AppSettings["untappedClientId"],
                client_secret = ConfigurationManager.AppSettings["untappedSecret"],
            };

            HttpHandler httpHandler = new HttpHandler();
            uri = httpHandler.CreateQueryFromObject(uri, "beer/info/" + beerId, requestObj);
            HttpResponseMessage response = await httpHandler.GetAsync(uri);

            return JsonConvert.DeserializeObject<UnTappedBeerInfo.Root>(Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync()));
        }
    }
}