using ItbApi.Models;
using ItbApi.Settings;
using Newtonsoft.Json;
using System.Text;

namespace ItbApi.Helpers
{
    public class UnTapped(UntappedSettings settings) : IUnTapped
    {
        public UntappedSettings Settings { get; } = settings;

        public async Task<UnTappedBeerSearch.Root> SearchUnTapped(string query)
        {
            Uri uri = new Uri(Settings.Url);
            var requestObj = new
            {
                client_id = Settings.ClientId,
                client_secret = Settings.Secret,
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
            Uri uri = new Uri(Settings.Url);
            var requestObj = new
            {
                client_id = Settings.ClientId,
                client_secret = Settings.Secret,
            };

            HttpHandler httpHandler = new HttpHandler();
            uri = httpHandler.CreateQueryFromObject(uri, "beer/info/" + beerId, requestObj);
            HttpResponseMessage response = await httpHandler.GetAsync(uri);
            var str = await response.Content.ReadAsByteArrayAsync();
            try
            {
                 return JsonConvert.DeserializeObject<UnTappedBeerInfo.Root>(Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync()));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}