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

namespace sinemaci
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinate mevcutLokasyon;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            loader.IsIndeterminate = true;
            bannerClick.Tap += bannerClick_Tap;
            listBuHafta.SelectionChanged += listBuHafta_SelectionChanged;
            listGecenHafta.SelectionChanged += listGecenHafta_SelectionChanged;
            listSalonlar.SelectionChanged += listSalonlar_SelectionChanged;
        }

        void listSalonlar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listSalonlar.SelectedItem != null)
            {
                Salonlar.Salon SelectedTheater = listSalonlar.SelectedItem as Salonlar.Salon;
                NavigationService.Navigate(new Uri(string.Format("/SalonPage.xaml?ID={0}", SelectedTheater.id), UriKind.Relative));
            }
        }

        void listGecenHafta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listGecenHafta.SelectedItem != null)
            {
                Filmler.Movie SelectedMovie = listGecenHafta.SelectedItem as Filmler.Movie;
                NavigationService.Navigate(new Uri(string.Format("/FilmByIDPage.xaml?ID={0}", SelectedMovie.id), UriKind.Relative));
            }
        }

        void listBuHafta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBuHafta.SelectedItem != null)
            {
                Filmler.Movie SelectedMovie = listBuHafta.SelectedItem as Filmler.Movie;
                NavigationService.Navigate(new Uri(string.Format("/FilmByIDPage.xaml?ID={0}", SelectedMovie.id), UriKind.Relative));
            }
        }

        void bannerClick_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var task = new WebBrowserTask();
            task.Uri = new Uri(Reklam.Reklamlar.splash.clickUrl, UriKind.Absolute);
            task.Show();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //Vizyondaki filmleri indir
            Filmler filmler = new Filmler();
            filmler.getCompleted += filmler_getCompleted;
            filmler.get();

            //GPS lokasyonunu bul ki yakın sinemaları gösterelim.
            GeoCoordinateWatcher geoWatch = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            geoWatch.MovementThreshold = 500;
            geoWatch.PositionChanged += geoWatch_PositionChanged;
            geoWatch.Start();

            //reklam verisini de indirmeye başlayalım.
            Reklam reklam = new Reklam();
            reklam.getCompleted += reklam_getCompleted;
            reklam.get();

            listBuHafta.ItemsSource = filmler.buHafta;
            listGecenHafta.ItemsSource = filmler.gelecekHafta;
        }

        void reklam_getCompleted(Reklam.RootObject data)
        {
            bannerBrowser.Navigated += bannerBrowser_Navigated;
            bannerBrowser.Navigate(new Uri(data.vizyon.source, UriKind.Absolute));
        }

        void bannerBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            bannerBrowser.Height = bannerBrowser.ActualWidth * 40.5 / 480;
            bannerClick.Height = bannerBrowser.Height;
            AppRoot.Margin = new Thickness(0, bannerBrowser.ActualHeight, 0, 0);
        }

        void geoWatch_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            mevcutLokasyon = e.Position.Location;
            Salonlar yakinSalonlar = new Salonlar();
            yakinSalonlar.getCompleted += yakinSalonlar_getCompleted;
            yakinSalonlar.get(e.Position.Location.Latitude, e.Position.Location.Longitude);
        }

        void yakinSalonlar_getCompleted(Salonlar sender)
        {
            listSalonlar.ItemsSource = sender.salonlar;
            gpsLoader.IsIndeterminate = false;
            gpsLoader.Visibility = System.Windows.Visibility.Collapsed;
        }

        void filmler_getCompleted()
        {
            loader.IsIndeterminate = false;
        }

        private void barbtn_harita(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/harita.xaml", UriKind.Relative));
        }
    }
}