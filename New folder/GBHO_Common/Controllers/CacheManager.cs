using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GBHO_Common.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheManager
    {
        private const string KeysUsedRegister = "KeysUsedRegister";
        public static void ClearCache()
        {
            if (ExistsInCache(KeysUsedRegister))
            {
                List<string> cachedKeys = GetFromCache<List<string>>(KeysUsedRegister);
                foreach (string cachedKey in cachedKeys)
                {
                    HttpContext.Current.Cache.Remove(cachedKey);
                }
            }
        }

        public static void RemoveFromCache(string key)
        {
            if (ExistsInCache(key))
            {
                HttpContext.Current.Cache.Remove(key);
            }

        }


        public static bool ExistsInCache(string key)
        {
            bool result = false;

            if (HttpContext.Current != null)
            {

                if (HttpContext.Current.Cache != null)
                {

                    result = (HttpContext.Current.Cache[key] != null);

                }
            }


            return result;
        }

        public static bool ExistsInCache(string key, Type expectedType)
        {
            bool result = false;

            if (ExistsInCache(key))
            {
                result = (HttpContext.Current.Cache[key].GetType() == expectedType);
            }


            return result;
        }

        private static void RegisterCacheKey(string keyUsed)
        {
            if (keyUsed == KeysUsedRegister) //dont want to add to the keys used
                return;

            List<string> keys = new List<string>();

            if (ExistsInCache(KeysUsedRegister))
            {
                keys = GetFromCache<List<string>>(KeysUsedRegister);
            }

            if (!keys.Contains(keyUsed))
                keys.Add(keyUsed);

            SaveToCache(KeysUsedRegister, keys);
        }

        public static void SaveToCache(string key, object value)
        {
            if (!CacheManager.CacheEnabled())
                return;

            RegisterCacheKey(key);

            if (ExistsInCache(key))
            {
                HttpContext.Current.Cache[key] = value;
            }
            else
            {
                HttpContext.Current.Cache.Insert(key, value);
            }
        }

        public static void SaveToCache(string key, object value, int expiryInMinutes)
        {
            if (!CacheManager.CacheEnabled())
                return;

            RegisterCacheKey(key);

            if (ExistsInCache(key))
            {
                HttpContext.Current.Cache[key] = value;
            }
            else
            {
                HttpContext.Current.Cache.Insert(key, value, null, DateTime.Now.AddMinutes(expiryInMinutes), System.Web.Caching.Cache.NoSlidingExpiration);
            }
        }

        private static bool CacheEnabled()
        {
            return (HttpContext.Current != null && HttpContext.Current.Cache != null);
        }

        public static T GetFromCache<T>(string key)
        {
            T cachedItem = (T)HttpContext.Current.Cache[key];
            return cachedItem;
        }

    }

}
