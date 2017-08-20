using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Wrly.Infrastructure.Utils
{
    public static class CacheManager
    {
        const int defaultExpiration = 20;
        public static T GetValue<T>(string key)
        {
            return (T)WebCache.Get(key);
        }

        public static void Add(string key, object value, int expiration = defaultExpiration)
        {
            WebCache.Set(key, value, defaultExpiration);
        }

        public static void Delete(string key)
        {
            WebCache.Remove(key);
        }
    }
}