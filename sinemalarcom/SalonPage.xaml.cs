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
using Microsoft.Phone.Maps.Controls;

namespace sinemaci
{
    public partial class SalonPage : PhoneApplicationPage
    {
        public SalonPage()
        {
            InitializeComponent();
            listFilmler.SelectionChanged += listFilmler_SelectionChanged;
            recMap.Tap += recMap_Tap;
        }

        void recMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (SalonCoordinate != null)
            {
                NavigationService.Navigate(
                    new Uri(string.Format("/harita.xaml?ID={0}&lat={1}&long={2}", 
                        TheaterID, SalonCoordinate.Latitude, SalonCoordinate.Longitude), UriKind.Relative));
            }
        }

        void listFilmler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listFilmler.SelectedItem != null)
            {
                seanslar.Movie gelenMovie = listFilmler.SelectedItem as seanslar.Movie;
                NavigationService.Navigate(new Uri(string.Format("/FilmByIDPage.xaml?ID={0}", gelenMovie.id), UriKind.Relative));
            }
        }

        private string TheaterID;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            listFilmler.SelectedIndex = -1;

            TheaterID = NavigationContext.QueryString["ID"];

            seanslar Seanslar = new seanslar();
            Seanslar.getCompleted += Seanslar_getCompleted;
            Seanslar.get(TheaterID);
        }

        GeoCoordinate SalonCoordinate;

        void Seanslar_getCompleted(seanslar sender)
        {
            loader.IsIndeterminate = false;

            PanoramaRoot.Title = sender.SalonBilgisi.name;
            pItem1.DataContext = sender.SalonBilgisi;
            listFilmler.ItemsSource = sender.SalonBilgisi.movies;

            if (sender.SalonBilgisi.latitude.ToString() != "false")
            {
                SalonCoordinate = new GeoCoordinate(double.Parse(sender.SalonBilgisi.latitude), double.Parse(sender.SalonBilgisi.longitude));
                myMap.SetView(SalonCoordinate, 17);

                pinpoint_salon newPin = new pinpoint_salon();
                MapOverlay newOverlay = new MapOverlay();
                newOverlay.Content = newPin;
                newOverlay.GeoCoordinate = SalonCoordinate;
                newOverlay.PositionOrigin = new Point(0, 0);
                MapLayer MyLayer = new MapLayer();
                MyLayer.Add(newOverlay);
                myMap.Layers.Add(MyLayer);
            }
            else
            {
                myMap.Visibility = Visibility.Collapsed;
                recMap.Visibility = System.Windows.Visibility.Collapsed;
            }

        }
    }
}