using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sinemaci.entities
{
    public class seanslar
    {
        public RootObject SalonBilgisi { get; set; }

        public void get(string SalonID)
        {
            APIAccess.GetRequest<List<RootObject>> Req = new APIAccess.GetRequest<List<RootObject>>();
            Req.Completed += Req_Completed;
            Req.Download(APIAccess.APIuris.Seanslar_bySalon, SalonID);
        }

        void Req_Completed(List<seanslar.RootObject> data)
        {
            SalonBilgisi = data[0];

            if (SalonBilgisi.movies != null)
            {
                foreach (var movieitem in SalonBilgisi.movies)
                {
                    movieitem.allseances = new List<string>();
                    foreach (var item in movieitem.seances)
                    {
                        foreach (var seanceInfo in item)
                        {
                            movieitem.allseances.Add(seanceInfo);
                        }
                    }
                }
            }


            if (getCompleted != null)
            {
                getCompleted(this);
            }
        }

        public delegate void getCompletedEvent(seanslar sender);
        public event getCompletedEvent getCompleted;

        public class Movie
        {
            public string id { get; set; }
            public string name { get; set; }
            public string orgName { get; set; }
            public string image { get; set; }
            public string rating { get; set; }
            public string director { get; set; }
            public List<List<string>> seances { get; set; }
            public int selected { get; set; }
            public List<string> allseances { get; set; }
        }

        public class RootObject
        {
            public string city { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string phone { get; set; }
            public string address { get; set; }
            public string cityId { get; set; }
            public int ad { get; set; }
            public List<Movie> movies { get; set; }
        }
    }
}
