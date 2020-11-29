using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace ReviewerAPI
{
    public static class Cache
    {
        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expirationTime">The expiration time. (absolute)</param>
        public static void Set(string key, object value, DateTime expirationTime)
        // TODO: Á ekki að nota sliding expiration. Mér finnst það allavega ruglandi. Gengur ekki upp á einum crusial stað á vefnum, varðandi cache á sharepoint listum
        {
            Set(key, value, null, expirationTime, System.Web.Caching.Cache.NoSlidingExpiration);
        }

        /// <param name="expirationTime">caching time</param>
        public static void Set(string key, object value, TimeSpan cacheTime)
        {
            Set(key, value, null, DateTime.Now.Add(cacheTime), System.Web.Caching.Cache.NoSlidingExpiration);
        }
        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        public static void SetSliding(string key, object value, TimeSpan slidingExpiration)
        // TODO: Á ekki að nota sliding expiration. Mér finnst það allavega ruglandi. Gengur ekki upp á einum crusial stað á vefnum, varðandi cache á sharepoint listum
        {
            Set(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="dependencies">The dependencies.</param>
        /// <param name="absoluteExpiration">The absolute expiration.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        public static void Set(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            HttpContextFactory.Current.Cache.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// Cachar í eina mínútu. Til að taka oddinn af álaginu
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="data"></param>
        public static void SetForShorttime(String cacheName, object data)
        {
            DateTime expirationTime = DateTime.Now.AddSeconds(15);
            HttpContextFactory.Current.Cache.Insert(cacheName, data, null, expirationTime, System.Web.Caching.Cache.NoSlidingExpiration);

        }

        /// <summary>
        /// Cachar í 3 sekúndur. Til að taka oddinn af álaginu
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="data"></param>
        public static void SetForAnInstance(String cacheName, object data)
        {
            DateTime expirationTime = DateTime.Now.AddSeconds(5);
            HttpContextFactory.Current.Cache.Insert(cacheName, data, null, expirationTime, System.Web.Caching.Cache.NoSlidingExpiration);

        }

        /// <summary>
        /// Gets the specified key.s
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Cached object</returns>
        public static object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public static T1 Get<T1>(string key)
        {
            if (Has(key))
            {
                try
                {
                    return (T1)HttpRuntime.Cache.Get(key);
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException("The object in cache is not of the type requested! Key: " + key, ex);
                }
            }
            else
                throw new ApplicationException("The object requested is not in the cache! Key: " + key);
        }

        public static bool Has(string cacheKey)
        {
            if (HttpRuntime.Cache != null)
                return HttpRuntime.Cache.Get(cacheKey) != null;
            else
                return false;
        }

        public enum CacheDuration
        {
            Week,
            Day,
            Hour,
            HalfHour,
            TenSeconds,
            Minute,
            ThreeMinutes,
            TenMinutes,
            HalfDay,
            TwentyFiveSec
        }

        public static void Add(object objectToCache, string cacheKey)
        {
            Add(objectToCache, cacheKey, CacheDuration.Day);
        }

        //public static void Add(object objectToCache, string cacheKey, CacheDuration duration)
        //{
        //    DateTime expirationDate;

        //    switch (duration)
        //    {
        //        case CacheDuration.Week:
        //            expirationDate = DateTime.Now.AddDays(7);
        //            break;
        //        case CacheDuration.Day:
        //            expirationDate = DateTime.Now.AddDays(1);
        //            break;
        //        case CacheDuration.Hour:
        //            expirationDate = DateTime.Now.AddHours(1);
        //            break;
        //        case CacheDuration.HalfDay:
        //            expirationDate = DateTime.Now.AddHours(12);
        //            break;
        //        case CacheDuration.HalfHour:
        //            expirationDate = DateTime.Now.AddMinutes(30);
        //            break;
        //        case CacheDuration.TenSeconds:
        //            expirationDate = DateTime.Now.AddSeconds(10);
        //            break;
        //        case CacheDuration.ThreeMinutes:
        //            expirationDate = DateTime.Now.AddMinutes(3);
        //            break;
        //        case CacheDuration.Minute:
        //            expirationDate = DateTime.Now.AddMinutes(1);
        //            break;
        //        default:
        //            expirationDate = DateTime.Now;
        //            break;
        //    }

        //    if (HttpRuntime.Cache != null)
        //    {
        //        HttpRuntime.Cache.Add(cacheKey, objectToCache, null, expirationDate, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        //    }
        //    //HttpContext.Current.Cache.Insert(cacheKey, objectToCache, null, expirationDate, System.Web.Caching.Cache.NoSlidingExpiration);
        //}

        public static void Add(object objectToCache, string cacheKey, CacheDuration duration)
        {
            DateTime expirationDate;

            switch (duration)
            {
                case CacheDuration.Week:
                    expirationDate = DateTime.Now.AddDays(7);
                    break;
                case CacheDuration.Day:
                    expirationDate = DateTime.Now.AddDays(1);
                    break;
                case CacheDuration.Hour:
                    expirationDate = DateTime.Now.AddHours(1);
                    break;
                case CacheDuration.HalfDay:
                    expirationDate = DateTime.Now.AddHours(12);
                    break;
                case CacheDuration.HalfHour:
                    expirationDate = DateTime.Now.AddMinutes(30);
                    break;
                case CacheDuration.TenSeconds:
                    expirationDate = DateTime.Now.AddSeconds(10);
                    break;
                case CacheDuration.TwentyFiveSec:
                    expirationDate = DateTime.Now.AddSeconds(25);
                    break;
                case CacheDuration.ThreeMinutes:
                    expirationDate = DateTime.Now.AddMinutes(3);
                    break;
                case CacheDuration.TenMinutes:
                    expirationDate = DateTime.Now.AddMinutes(10);
                    break;
                case CacheDuration.Minute:
                    expirationDate = DateTime.Now.AddMinutes(1);
                    break;
                default:
                    expirationDate = DateTime.Now;
                    break;
            }

            if (HttpContext.Current != null)
                HttpContext.Current.Cache.Insert(cacheKey, objectToCache, null, expirationDate, System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public static void InvalidateCacheFor(string idToInvalidateCacheFor)
        {
            HttpContext.Current.Cache.Remove(idToInvalidateCacheFor);
        }

        public static void RemoveAllCache()
        {
            foreach (System.Collections.DictionaryEntry entry in HttpContext.Current.Cache)
            {
                HttpContext.Current.Cache.Remove((string)entry.Key);
            }
        }

    }

    public class HttpContextFactory
    {
        private static HttpContextBase m_context;
        public static HttpContextBase Current
        {
            get
            {
                if (m_context != null)
                    return m_context;

                if (HttpContext.Current == null)
                    throw new InvalidOperationException("HttpContext is not available");

                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        public static void SetCurrentContext(HttpContextBase context)
        {
            m_context = context;
        }
    }
}