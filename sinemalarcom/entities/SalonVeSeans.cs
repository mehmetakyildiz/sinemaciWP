using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sinemaci.serviceAccess.APIAccess;
using sinemaci.serviceAccess;

namespace sinemaci.entities
{
    public class SalonVeSeans
    {
        public RootObject SalonVeSeanslar { get; set; }
        public NearestCinema_RootObject EnYakinSalon { get; set; }

        private string filmID;

        public void get(double lat, double lng, string FilmID)
        {
            filmID = FilmID;
            GetRequest<NearestCinema_RootObject> Req = new GetRequest<NearestCinema_RootObject>();
            Req.Completed +=NearestCinema_Req_Completed;
            Req.Download(APIuris.TekSalon_byGPS_byMovieID, lat.ToString(), lng.ToString(), FilmID);
        }

        void NearestCinema_Req_Completed(SalonVeSeans.NearestCinema_RootObject data)
        {
            EnYakinSalon = data;

            GetRequest<RootObject> Req = new GetRequest<RootObject>();
            Req.Completed += Req_Completed;
            Req.Download(APIuris.SalonlarVeSeanslar_byCity_byMovieID, data.cinema.cityId, filmID);
        }

        void Req_Completed(SalonVeSeans.RootObject data)
        {
            SalonVeSeanslar = data;

            List<Theatre> NoSeance_ToBeRemoved = new List<Theatre>();
            foreach (var theater in SalonVeSeanslar.theatre)
            {
                theater.allseances = new List<string>();
                foreach (var item in theater.seances)
                {
                    if (!(item is string))
                    {
                        foreach (string seanceInfo in ((Newtonsoft.Json.Linq.JArray)item).Values<string>())
                        {
                            theater.allseances.Add(seanceInfo);
                        }
                    }
                    else
                    {
                        NoSeance_ToBeRemoved.Add(theater);
                    }
                }
            }
            foreach (var Remove in NoSeance_ToBeRemoved)
            {
                SalonVeSeanslar.theatre.Remove(Remove);
            }

            if (getCompleted != null)
            {
                getCompleted(this);
            }
        }

        public delegate void getCompletedEvent(SalonVeSeans sender);
        public event getCompletedEvent getCompleted;

        public class Movie
        {
            public string name { get; set; }
            public string orgName { get; set; }
            public string id { get; set; }
        }

        public class Theatre
        {
            public string id { get; set; }
            public string name { get; set; }
            public string city { get; set; }
            public List<object> seances { get; set; }
            public object selected { get; set; }
            public int ad { get; set; }
            public List<string> allseances { get; set; }
        }

        public class RootObject
        {
            public Movie movie { get; set; }
            public List<Theatre> theatre { get; set; }
        }

        public class Cinema
        {
            public string name { get; set; }
            public string cityId { get; set; }
            public string id { get; set; }
        }

        public class NearestCinema_RootObject
        {
            public List<string> seances { get; set; }
            public int selected { get; set; }
            public Cinema cinema { get; set; }
        }
    }
}
