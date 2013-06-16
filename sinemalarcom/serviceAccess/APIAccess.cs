using Newtonsoft.Json;
using sinemaci.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace sinemaci.serviceAccess.APIAccess
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
                    System.Windows.MessageBox.Show("Sinemalar.com'a bağlanılamıyor. Lütfen daha sonra tekrar deneyin.");
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




}
