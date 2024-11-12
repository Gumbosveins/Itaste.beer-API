using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace ItbApi.Helpers
{
    /// <summary>
    /// Utilities to for calling rest webservices
    /// </summary>
    public class RestUtil
    {
        //private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Calls a http webservice, only supports GET and POST as of now
        /// </summary>
        /// <typeparam name="T">Type of response</typeparam>
        /// <param name="url">Url of service</param>
        /// <param name="httpRequestType">GET or POST</param>
        /// <param name="requestObj">The request object</param>
        /// <returns></returns>
        public static T RestRequest<T>(string url, HttpRequestType httpRequestType, object requestObj = null)
        {
            HttpWebRequest request;
            if (httpRequestType == HttpRequestType.GET && requestObj == null)
                request = GetGetRequest(url);
            else if (httpRequestType == HttpRequestType.POST)
                request = GetPostRequest(url, requestObj);
            else if (httpRequestType == HttpRequestType.GET)
                request = GetGetRequest(url, requestObj);
            else
                throw new ApplicationException("httpRequest type is unknown");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream receiveStream = response.GetResponseStream();

            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string responseText = readStream.ReadToEnd();
            response.Close();
            readStream.Close();

            T serialized = JsonConvert.DeserializeObject<T>(responseText);

            return serialized;
        }

        public static string RestRequestWithStringResponse(string url, HttpRequestType httpRequestType, object requestObj)
        {
            HttpWebRequest request;
            if (httpRequestType == HttpRequestType.GET && requestObj == null)
                request = GetGetRequest(url);
            else if (httpRequestType == HttpRequestType.POST)
                request = GetPostRequest(url, requestObj);
            else if (httpRequestType == HttpRequestType.GET)
                request = GetGetRequest(url, requestObj);
            else
                throw new ApplicationException("httpRequest type is unknown");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream receiveStream = response.GetResponseStream();

            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string responseText = readStream.ReadToEnd();
            response.Close();
            readStream.Close();

            return responseText;
        }

        /// <summary>
        /// Calls a http webservice, only supports GET and POST as of now
        /// </summary>
        /// <typeparam name="T">Type of response</typeparam>
        /// <param name="url">Url of service</param>
        /// <param name="httpRequestType">GET or POST</param>
        /// <param name="requestObj">The request object</param>
        /// <returns></returns>
        public static string RestRequestXML(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            string content = "";
            using (StreamReader reader = new StreamReader(dataStream))
            {
                try
                {
                    content = reader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error reading document");
                }
            }
            return content;
        }

        /// <summary>
        /// Helper method for post requests
        /// </summary>
        /// <returns></returns>
        private static HttpWebRequest GetPostRequest(string url, object obj)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ServicePoint.Expect100Continue = false;
            var data = JsonConvert.SerializeObject(obj);
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] _byteVersion = Encoding.UTF8.GetBytes(data);

            request.ContentLength = _byteVersion.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            return request;
        }

        /// <summary>
        /// Helper method for get requests
        /// </summary>
        /// <returns></returns>
        private static HttpWebRequest GetGetRequest(string url, object obj)
        {
            NameValueCollection propNames = new NameValueCollection();

            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.CanRead)
                {
                    var pName = propertyInfo.Name;
                    var pValue = propertyInfo.GetValue(obj);
                    if (pValue != null)
                    {
                        propNames.Add(pName, pValue.ToString());
                    }
                }
            }

            var parameters = new StringBuilder();

            foreach (string key in propNames.Keys)
            {
                parameters.AppendFormat("{0}={1}&",
                HttpUtility.UrlEncode(key),
                HttpUtility.UrlEncode(propNames[key]));
            }

            parameters.Length -= 1;

            var request = (HttpWebRequest)HttpWebRequest.Create(url + "?" + parameters);
            request.Method = "GET";
            request.ServicePoint.Expect100Continue = false;
            request.KeepAlive = true;

            return request;
        }

        private static HttpWebRequest GetGetRequest(string url)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.ServicePoint.Expect100Continue = false;

            return request;
        }
    }

    /// <summary>
    /// Possible request types.
    /// </summary>
    public enum HttpRequestType
    {
        POST,
        GET
    }
}