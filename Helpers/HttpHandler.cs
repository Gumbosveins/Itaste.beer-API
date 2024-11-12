using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace ItbApi.Helpers
{
    public class HttpHandler
    {
        public async Task<HttpResponseMessage> GetAsync(Uri url)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Accept", "application/json");
            using (HttpClient client = CreateHttpClientWithHeaders(new HttpClient(), headers))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                return await client.GetAsync(url);
            }
        }

        private static HttpClient CreateHttpClientWithHeaders(HttpClient client, IDictionary<string, string> headers)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            return client;
        }

        public Uri CreateQueryFromObject<T>(Uri baseUri, string pasePath, T obj)
        {
            Type type = typeof(T);
            string query = pasePath + "?";
            List<PropertyInfo> propertyInfos = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).ToList();
            foreach (PropertyInfo prop in propertyInfos)
            {
                object propValue = type.GetProperty(prop.Name).GetValue(obj, null);
                if (propValue == null)
                {
                    continue;
                }

                query += $"{prop.Name}={propValue}";
                if (propertyInfos.Last() != prop)
                {
                    query += "&";
                }
            }

            return new Uri(baseUri, query);
        }
    }
}