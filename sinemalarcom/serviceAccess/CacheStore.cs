using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sinemaci.serviceAccess
{
    public static class CacheStore
    {
        static CacheStore()
        {
            CacheList = new Dictionary<string, CacheObject>();
        }

        static Dictionary<string, CacheObject> CacheList { get; set; }

        public class CacheObject
        {
            public object Data { get; set; }
            public DateTime LastUpdate { get; set; }
        }

        public static void PutCache(string key, CacheObject OB)
        {
            if (CacheList.ContainsKey(key))
            {
                CacheList.Remove(key);
            }
            CacheList.Add(key, OB);
        }

        public static bool TryGetCache<T>(string key, out T data)
        {
            if (CacheList.ContainsKey(key))
            {
                data = (T)CacheList[key].Data;
                return true;
            }
            else
            {
                data = default(T);
                return false;
            }
        }
    }
}
