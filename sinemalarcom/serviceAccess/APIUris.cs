using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sinemaci.serviceAccess
{
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
