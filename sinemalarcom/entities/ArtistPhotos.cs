using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using sinemaci.serviceAccess.APIAccess;
using sinemaci.serviceAccess;

namespace sinemaci.entities
{
    public class ArtistPhotos
    {
        public System.Collections.ObjectModel.ObservableCollection<string> fotolar { get; set; }

        public ArtistPhotos()
        {
            fotolar = new System.Collections.ObjectModel.ObservableCollection<string>();
        }

        public void get(string ArtistID)
        {
            GetRequest<string[]> Req = new GetRequest<string[]>();
            Req.Error += Req_Error;
            Req.Completed += Req_Completed;
            Req.Download(APIuris.ArtistPhotos_byID, ArtistID);
        }

        void Req_Error()
        {
            if (getFailed != null)
            {
                getFailed(this);
            }
        }

        void Req_Completed(string[] data)
        {
            fotolar.Clear();
            if (data != null)
            {
                foreach (string item in data)
                {
                    fotolar.Add(item);
                }
            }

            if (getCompleted != null)
            {
                getCompleted(this);
            }
        }

        public delegate void getCompletedEvent(ArtistPhotos sender);
        public event getCompletedEvent getCompleted;

        public delegate void getFailedEvent(ArtistPhotos sender);
        public event getFailedEvent getFailed;
        
        public class RootObject
        {
            public string URL { get; set; }
        }
    }
}
