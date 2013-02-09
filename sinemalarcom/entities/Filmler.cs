using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace sinemaci.entities
{
    public class Filmler
    {
        public System.Collections.ObjectModel.ObservableCollection<Movie> buHafta { get; set; }
        public System.Collections.ObjectModel.ObservableCollection<Movie> gelecekHafta { get; set; }

        public Filmler()
        {
            buHafta  = new System.Collections.ObjectModel.ObservableCollection<Movie>();
            gelecekHafta = new System.Collections.ObjectModel.ObservableCollection<Movie>();
        }

        public void get()
        {
            sinemaci.APIAccess.GetRequest<RootObject> Req = new APIAccess.GetRequest<RootObject>();
            Req.Completed += Req_Completed;
            Req.Download(APIAccess.APIuris.Filmler);
        }

        void Req_Completed(Filmler.RootObject deserialized)
        {
            buHafta.Clear();
            foreach (Movie item in deserialized.movies[0])
            {
                buHafta.Add(item);
            }
            gelecekHafta.Clear();
            foreach (Movie item in deserialized.movies[1])
            {
                gelecekHafta.Add(item);
            }

            if (getCompleted != null)
            {
                getCompleted();
            }
        }

        public delegate void getCompletedEvent();
        public event getCompletedEvent getCompleted;

        public class RootObject
        {
            public List<string> sections { get; set; }
            public List<List<Movie>> movies { get; set; }
        }

        public class Movie
        {
            public string id { get; set; }
            public string name { get; set; }
            public string orgName { get; set; }
            public string image { get; set; }
            public string rating { get; set; }
            public string type { get; set; }
            public string director { get; set; }
        }
    }
}
