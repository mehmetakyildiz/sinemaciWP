using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using sinemaci.entities;
using System.Device.Location;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;

namespace sinemaci
{
    public partial class FilmByIDPage : PhoneApplicationPage
    {
        public FilmByIDPage()
        {
            InitializeComponent();
            listSalonlar.SelectionChanged += listSalonlar_SelectionChanged;
            listOyuncular.SelectionChanged += listOyuncular_SelectionChanged;
        }

        void listOyuncular_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOyuncular.SelectedIndex != -1)
            {
                Film.Artist Oyuncu = listOyuncular.SelectedItem as Film.Artist;
                NavigationService.Navigate(new Uri(string.Format("/photo_gallery.xaml?ID={0}&type=artist&name={1}", Oyuncu.id, Oyuncu.nameSurname), UriKind.Relative));
            }
        }

        void listSalonlar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listSalonlar.SelectedIndex != -1)
            {
                SalonVeSeans.Theatre SelectedTheater = listSalonlar.SelectedItem as SalonVeSeans.Theatre;
                NavigationService.Navigate(new Uri(string.Format("/SalonPage.xaml?ID={0}", SelectedTheater.id), UriKind.Relative));
            }
        }

        private string MovieID;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            listSalonlar.SelectedIndex = -1;
            listOyuncular.SelectedIndex = -1;
            MovieID = NavigationContext.QueryString["ID"];

            GeoCoordinateWatcher geoWatch = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            geoWatch.MovementThreshold = 500;
            geoWatch.PositionChanged +=geoWatch_PositionChanged;
            geoWatch.Start();

            Film birFilm = new Film();
            birFilm.getCompleted +=birFilm_getCompleted;
            birFilm.get(MovieID);
        }

        void geoWatch_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            SalonVeSeans yakinSeanslar = new SalonVeSeans();
            yakinSeanslar.getCompleted += yakinSeanslar_getCompleted;
            yakinSeanslar.get(e.Position.Location.Latitude, e.Position.Location.Longitude, MovieID);
        }

        void yakinSeanslar_getCompleted(SalonVeSeans sender)
        {
            listSalonlar.ItemsSource = sender.SalonVeSeanslar.theatre;
            gpsLoader.IsIndeterminate = false;
            gpsLoader.Visibility = System.Windows.Visibility.Collapsed;
        }

        void birFilm_getCompleted(Film.RootObject data)
        {
            loader.IsIndeterminate = false;
            PanoramaRoot.Title = data.movie.name;
            film_hakkinda.DataContext = data.movie;
            listOyuncular.ItemsSource = data.artists;
            listYorumlar.ItemsSource = data.comments;

            var Url = data.movie.trailerUrl;
            if (Url != "False")
            {
                ((ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).IsEnabled = true;
            }
            else
            {
                ((ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).IsEnabled = false;
            }

            MoviePhotos moviePhotos = new MoviePhotos();
            moviePhotos.getCompleted += moviePhotos_getCompleted;
            moviePhotos.get(data.movie.id);
        }

        void moviePhotos_getCompleted(MoviePhotos sender)
        {
            if (sender.fotolar.Count > 0)
            {
                ((ApplicationBarIconButton)this.ApplicationBar.Buttons[1]).IsEnabled = true;
            }
            else
            {
                ((ApplicationBarIconButton)this.ApplicationBar.Buttons[1]).IsEnabled = false;
            }
        }

        private void barbtn_fragman_click(object sender, EventArgs e)
        {
            MediaPlayerLauncher mediaPlayerLauncher = new MediaPlayerLauncher();
            mediaPlayerLauncher.Media = new Uri(((Film.Movie)film_hakkinda.DataContext).trailerUrl, UriKind.Absolute);
            mediaPlayerLauncher.Controls = MediaPlaybackControls.All;

            mediaPlayerLauncher.Location = MediaLocationType.Data;
            mediaPlayerLauncher.Show();
        }

        private void barbtn_photos_click(object sender, EventArgs e)
        {
            Film.Movie mevcutFilm = (Film.Movie)film_hakkinda.DataContext;
            NavigationService.Navigate(new Uri(string.Format("/photo_gallery.xaml?ID={0}&type=movie&name={1}", mevcutFilm.id, mevcutFilm.name), UriKind.Relative));
        }

    }
}