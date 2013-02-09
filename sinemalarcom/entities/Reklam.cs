using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace sinemaci.entities
{
    class Reklam
    {
        public Reklam()
        {
            Reklamlar = new RootObject();
        }

        public static RootObject Reklamlar { get; set; }

        public void get()
        {
            APIAccess.GetRequest<RootObject> Req = new APIAccess.GetRequest<RootObject>();
            Req.Completed += Req_Completed;
            Req.Download(APIAccess.APIuris.Reklamlar);
        }

        void Req_Completed(Reklam.RootObject data)
        {
            Reklamlar = data;

            if (getCompleted != null)
            {
                getCompleted(Reklamlar);
            }
        }

        public delegate void getCompletedEvent(RootObject data);
        public event getCompletedEvent getCompleted;

        public class Splash
        {
            public bool active { get; set; }
            public string target { get; set; }
            public int duration { get; set; }
            public string clickUrl { get; set; }
            public string source { get; set; }
        }

        public class Banner
        {
            public bool active { get; set; }
            public string target { get; set; }
            public int duration { get; set; }
            public string clickUrl { get; set; }
            public string source { get; set; }
        }

        public class Logo
        {
            public bool active { get; set; }
            public string target { get; set; }
            public int duration { get; set; }
            public string clickUrl { get; set; }
            public string source { get; set; }
        }

        public class Salon
        {
            public bool active { get; set; }
            public string target { get; set; }
            public int duration { get; set; }
            public string clickUrl { get; set; }
            public string source { get; set; }
        }

        public class Vizyon
        {
            public bool active { get; set; }
            public string target { get; set; }
            public int duration { get; set; }
            public string clickUrl { get; set; }
            public string source { get; set; }
        }

        public class InGallery
        {
            public bool active { get; set; }
            public string target { get; set; }
            public int duration { get; set; }
            public string clickUrl { get; set; }
            public string source { get; set; }
        }

        public class RootObject
        {
            public Splash splash { get; set; }
            public Banner banner { get; set; }
            public Logo logo { get; set; }
            public Salon salon { get; set; }
            public Vizyon vizyon { get; set; }
            public InGallery in_gallery { get; set; }
        }
    }
}
