using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sinemaci.serviceAccess.APIAccess;
using sinemaci.serviceAccess;

namespace sinemaci.entities
{
    public class Film
    {
        public RootObject Data { get; set; }

        public void get(string ID)
        {
            GetRequest<RootObject> Req = new GetRequest<RootObject>();
            Req.Completed += Req_Completed;
            Req.Download(APIuris.FilmByID, ID);
        }

        void Req_Completed(Film.RootObject deserialized)
        {
            Data = deserialized;

            if (getCompleted != null)
            {
                getCompleted(Data);
            }
        }

        public delegate void getCompletedEvent(RootObject data);
        public event getCompletedEvent getCompleted;

        public class Movie
        {
            public string id { get; set; }
            public string name { get; set; }
            public string orgName { get; set; }
            public string image { get; set; }
            public string rating { get; set; }
            public string type { get; set; }
            public string director { get; set; }
            public string summary { get; set; }
            public string duration { get; set; }
            public string produceYear { get; set; }
            public string week { get; set; }
            public string pubDate { get; set; }
            public string embedId { get; set; }
            public string trailerUrl { get; set; }
        }

        public class Artist
        {
            public string id { get; set; }
            public string nameSurname { get; set; }
            public string characterName { get; set; }
            public string image { get; set; }
        }

        public class Comment
        {
            public string id { get; set; }
            public string username { get; set; }
            public string comment { get; set; }
            public string addDate { get; set; }
        }

        public class RootObject
        {
            public Movie movie { get; set; }
            public List<Artist> artists { get; set; }
            public List<Comment> comments { get; set; }
        }
    }
}
