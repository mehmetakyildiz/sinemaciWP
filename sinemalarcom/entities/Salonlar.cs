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
    public class Salonlar
    {
        public System.Collections.ObjectModel.ObservableCollection<Salon> salonlar { get; set; }

        public Salonlar()
        {
            salonlar = new System.Collections.ObjectModel.ObservableCollection<Salon>();
        }

        public void get(double lat, double lng)
        {
            GetRequest<RootObject> Req = new GetRequest<RootObject>();
            Req.Completed += Req_Completed;
            Req.Download(APIuris.Salonlar_ByGPS, lat.ToString(), lng.ToString());
        }
        
        void Req_Completed(Salonlar.RootObject deserialized)
        {
            salonlar.Clear();
            foreach (Salon item in deserialized.five)
            {
                salonlar.Add(item);
            }
            foreach (Salon item in deserialized.ten)
            {
                salonlar.Add(item);
            }

            if (getCompleted != null)
            {
                getCompleted(this);
            }
        }

        public delegate void getCompletedEvent(Salonlar sender);
        public event getCompletedEvent getCompleted;

        public class Salon
        {
            public string id { get; set; }
            public string name { get; set; }
            public string city { get; set; }
            public string phone { get; set; }
            public string address { get; set; }
            public string town { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string distance { get; set; }
        }

        public class RootObject
        {
            public List<Salon> five { get; set; }
            public List<Salon> ten { get; set; }
        }
    }
}
