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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            InitialDataBinding();

            //GPS lokasyonunu bul ki yakın sinemaları gösterelim.
            GeoCoordinateWatcher geoWatch = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            geoWatch.MovementThreshold = 500;
            geoWatch.PositionChanged += geoWatch_PositionChanged;
            geoWatch.Start();
        }

        void InitialDataBinding()
        {
            //Vizyondaki filmleri indir
            Filmler filmler = new Filmler();
            filmler.getCompleted += filmler_getCompleted;
            filmler.get();

            listBuHafta.ItemsSource = filmler.buHafta;
            listGecenHafta.ItemsSource = filmler.gelecekHafta;
        }

        void PostGPSDetection_DataBinding()
        {
            Salonlar yakinSalonlar = new Salonlar();
            yakinSalonlar.getCompleted += yakinSalonlar_getCompleted;
            yakinSalonlar.get(mevcutLokasyon.Latitude, mevcutLokasyon.Longitude);
        }

        void geoWatch_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            mevcutLokasyon = e.Position.Location;
            PostGPSDetection_DataBinding();
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

        private void barbtn_refresh(object sender, EventArgs e)
        {
            ApplicationBarIconButton source = (ApplicationBarIconButton)sender;
            source.IsEnabled = false;
            InitialDataBinding();
            PostGPSDetection_DataBinding();
            source.IsEnabled = true;
        }
    }
}