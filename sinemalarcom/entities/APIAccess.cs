using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace sinemaci.APIAccess
{
    public class GetRequest<T>
    {
        public void Download(string AccessUri, params object[] args)
        {
            string internalUri = string.Format(AccessUri, args);

            T FoundCache;
            if (CacheStore.TryGetCache<T>(internalUri, out FoundCache))
            {
                if (Completed != null)
                {
                    Completed(FoundCache);
                }
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += client_DownloadStringCompleted;
                client.DownloadStringAsync(new Uri(internalUri, UriKind.Absolute), internalUri);
            }
        }

        private int RetryCount = 0;

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                T deserialized = JsonConvert.DeserializeObject<T>(e.Result);

                CacheStore.CacheObject newCache = new CacheStore.CacheObject();
                newCache.Data = deserialized;
                newCache.LastUpdate = DateTime.Now;
                CacheStore.PutCache(e.UserState.ToString(), newCache);

                if (Completed != null)
                {
                    Completed(deserialized);
                }
            }
            catch (Exception ex)
            {
                if (RetryCount < 2)
                {
                    RetryCount += 1;
                    Download(e.UserState.ToString());
                }
                else
                {
                    throw ex;
                    //System.Windows.MessageBox.Show("Can't connect! Try again later.");
                    if (Error != null)
                    {
                        Error();
                    }
                }
            }
        }

        public delegate void DownloadCompleted(T data);
        public event DownloadCompleted Completed;

        public delegate void ErrorHappened();
        public event ErrorHappened Error;
    }

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

    public static class APIuris
    {
        public static string Filmler = @"http://www.sinemalar.com/json/mobile/playingMovies";
        public static string Salonlar_ByGPS = @"http://api.sinemalar.com/ajax/json/ios/v1/gps/nearTheatre/{0}/{1}";
        public static string Reklamlar = @"http://prm.virgul.com/ad_mobile47.php";
        public static string FilmByID = @"http://api.sinemalar.com/ajax/json/ios/v1/get/movie/{0}/1/1";
        //Şehrde ve filme göre filim oynadığı tüm salonları ve seansları getirir.
        public static string SalonlarVeSeanslar_byCity_byMovieID = @"http://api.sinemalar.com/ajax/json/ios/v1/get/theatreSeance/{0}/{1}";
        //Salona göre seansları getirir.
        public static string Seanslar_bySalon = @"http://api.sinemalar.com/ajax/json/ios/v1/get/theatre/{0}";
        //GPS'e göre bir filmin oynadığı en yakın salonu ve seansları getirir.
        public static string TekSalon_byGPS_byMovieID = @"http://api.sinemalar.com/ajax/json/ios/v1/gps/seance/{0}/{1}/{2}";
        //ArtistID ile artist fotoğraflarını getirir
        public static string ArtistPhotos_byID = @"http://api.sinemalar.com/ajax/json/ios/v1/gallery/artist/{0}";
        //MovieID ile film fotoğraflarını getirir
        public static string MoviePhotos_byID = @"http://api.sinemalar.com/ajax/json/ios/v1/gallery/movie/{0}";
        
    }
}
