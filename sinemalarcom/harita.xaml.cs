using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using sinemaci.entities;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Maps.Services;

namespace sinemaci
{
    public partial class harita : PhoneApplicationPage
    {
        public harita()
        {
            InitializeComponent();
            myMap.ZoomLevelChanged += myMap_ZoomLevelChanged;
        }

        GeoCoordinateCollection collection = new GeoCoordinateCollection();
        bool ReadyForScaling = false;
        bool SingleTheater = false;
        //int SingleTheater_ID;
        //double SingleTheater_Lat;
        //double SingleTheater_Long;

        void myMap_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            UpdateAccuracyShow();

            if (ReadyForScaling)
            {
                if (myMap.ZoomLevel < 14)
                {
                    ScaleToAnimation(Pin_ME, 0.5, 1500);
                    foreach (pinpoint_salon item in AllPinPoints)
                    {
                        ScaleToAnimation(item, 0.5, 1500);
                    }
                }
                else
                {
                    ScaleToAnimation(Pin_ME, 1, 500);
                    foreach (pinpoint_salon item in AllPinPoints)
                    {
                        ScaleToAnimation(item, 1, 1500);
                    }
                }
            }          
        }

        private void ScaleToAnimation(UIElement targetObject, double targetValue, int milliseconds)
        {
            Storyboard SB = new Storyboard();
            DoubleAnimation DBL = new DoubleAnimation();
            Storyboard.SetTarget(DBL, targetObject.RenderTransform);
            Storyboard.SetTargetProperty(DBL, new PropertyPath(ScaleTransform.ScaleXProperty));
            DBL.To = targetValue;
            DBL.Duration = TimeSpan.FromMilliseconds(milliseconds);
            DBL.EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut };
            SB.Children.Add(DBL);
            DBL = new DoubleAnimation();
            Storyboard.SetTarget(DBL, targetObject.RenderTransform);
            Storyboard.SetTargetProperty(DBL, new PropertyPath(ScaleTransform.ScaleYProperty));
            DBL.To = targetValue;
            DBL.Duration = TimeSpan.FromMilliseconds(milliseconds);
            DBL.EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut };
            SB.Children.Add(DBL);
            SB.Begin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            collection.Clear();

            if (NavigationContext.QueryString.ContainsKey("ID"))
            {
                SingleTheater = true;

                Salonlar.Salon GelenSalon = new Salonlar.Salon();
                GelenSalon.latitude = NavigationContext.QueryString["lat"];
                GelenSalon.longitude = NavigationContext.QueryString["long"];
                GelenSalon.id = NavigationContext.QueryString["ID"];

                collection.Add(new GeoCoordinate(double.Parse(GelenSalon.latitude), double.Parse(GelenSalon.longitude)));
                myMap.SetView(new GeoCoordinate(double.Parse(GelenSalon.latitude), double.Parse(GelenSalon.longitude)), 17);

                Add_SalonPinPoint(GelenSalon, true);
            }

            GeoCoordinateWatcher geoWatch = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            geoWatch.MovementThreshold = 500;
            geoWatch.PositionChanged += geoWatch_PositionChanged;
            geoWatch.Start();
                        
            base.OnNavigatedTo(e);
        }

        private void UpdateAccuracyShow()
        {
            if(mevcutLokasyon !=null)
            {
                var latitude = Math.Min(Math.Max(mevcutLokasyon.Latitude, MinLatitude), MaxLatitude); ;
                var metersPERPixel = Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * EarthRadius / ((uint)256 << (int)myMap.ZoomLevel);
                var AccuracyPixels = mevcutLokasyon.HorizontalAccuracy / metersPERPixel;
                Pin_ME.SetAccuracy(AccuracyPixels);
            }
        }

        GeoCoordinate mevcutLokasyon;
        pinpoint_me Pin_ME = new pinpoint_me();
        MapOverlay MapOverlay_ForMe;

        private const double MinLatitude = -85.05112878;
        private const double MaxLatitude = 85.05112878;
        private const double EarthRadius = 6378137;
        
        void geoWatch_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            mevcutLokasyon = e.Position.Location;
            if (!SingleTheater)
            {
                myMap.SetView(e.Position.Location, 15);
            }
            else
            {
                collection.Add(e.Position.Location);
                LocationRectangle setRect = null;
                setRect = LocationRectangle.CreateBoundingRectangle(collection); 
                myMap.SetView(setRect);
            }

            //Önce bir kendimizi gösterelim.
            //accuracy hesaplansın
            UpdateAccuracyShow();

            if (MapOverlay_ForMe == null)
            {
                MapOverlay_ForMe = new MapOverlay();
                MapOverlay_ForMe.Content = Pin_ME;
                Pin_ME.RenderTransformOrigin = new Point(0, 0);
                Pin_ME.RenderTransform = new ScaleTransform();
                MapOverlay_ForMe.PositionOrigin = new Point(0, 0);
                MapLayer MyLayer = new MapLayer();
                MyLayer.Add(MapOverlay_ForMe);
                myMap.Layers.Add(MyLayer);
            }                    
            MapOverlay_ForMe.GeoCoordinate = e.Position.Location;

            if (!SingleTheater)
            {
                //Pull nearest theaters if not in "SingleTheater" mode.
                Salonlar yakinSalonlar = new Salonlar();
                yakinSalonlar.getCompleted += yakinSalonlar_getCompleted;
                yakinSalonlar.get(mevcutLokasyon.Latitude, mevcutLokasyon.Longitude);
            }
            else
            {
                //Let's do routing if we are in "SingleTheater" mode.
                if (SingleTheater)
                {
                    RouteQuery newQuery = new RouteQuery();
                    List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
                    MyCoordinates.Add(mevcutLokasyon);
                    MyCoordinates.Add(collection[0]);
                    newQuery.Waypoints = MyCoordinates;
                    newQuery.QueryCompleted += newQuery_QueryCompleted;
                    newQuery.QueryAsync();
                }
            }
        }

        void newQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                Route MyRoute = e.Result;
                MapRoute MyMapRoute = new MapRoute(MyRoute);
                myMap.AddRoute(MyMapRoute);
            }
        }

        void yakinSalonlar_getCompleted(Salonlar sender)
        {
            foreach (Salonlar.Salon item in sender.salonlar)
            {
                bool AlreadyExists = false;
                foreach (pinpoint_salon itemSalon in AllPinPoints)
                {
                    if (itemSalon.CurrentSalon.id == item.id)
                    {
                        AlreadyExists = true;
                    }
                }
                if(!AlreadyExists)
                {
                    Add_SalonPinPoint(item);
                }
            }
            LocationRectangle setRect = null;
            setRect = LocationRectangle.CreateBoundingRectangle(collection);
            myMap.SetView(setRect);

            ReadyForScaling = true;
        }

        public void Add_SalonPinPoint(Salonlar.Salon item, bool IsSingle = false)
        {
            pinpoint_salon newPin = new pinpoint_salon();
            if (IsSingle)
            {
                newPin.ShowDetails = false;
            }
            newPin.RenderTransformOrigin = new Point(0, 0);
            newPin.RenderTransform = new ScaleTransform();
            newPin.Selected += newPin_Selected;
            newPin.Redirect2Theater += newPin_Redirect2Theater;
            newPin.SetContent(item);
            MapOverlay newOverlay = new MapOverlay();
            newOverlay.Content = newPin;
            newOverlay.GeoCoordinate = new GeoCoordinate(double.Parse(item.latitude), double.Parse(item.longitude));
            newOverlay.PositionOrigin = new Point(0, 0);
            MapLayer MyLayer = new MapLayer();
            MyLayer.Add(newOverlay);
            myMap.Layers.Add(MyLayer);
            AllPinPoints.Add(newPin);

            collection.Add(newOverlay.GeoCoordinate);
        }

        void newPin_Redirect2Theater(string TheaterID)
        {
            NavigationService.Navigate(new Uri(string.Format("/SalonPage.xaml?ID={0}", TheaterID), UriKind.Relative));
        }

        List<pinpoint_salon> AllPinPoints = new List<pinpoint_salon>();
        
        void newPin_Selected(pinpoint_salon sender, Salonlar.Salon salon)
        {
            if (SingleTheater)
            {
                //Pinpoint transferi olmuyor MapTask'e.. :(
                //MapsTask mapT = new MapsTask();
                //mapT.Center = new GeoCoordinate()
                //mapT.Show();
            }
            else
            {
                foreach (pinpoint_salon item in AllPinPoints)
                {
                    if (item != sender)
                    {
                        item.ClearSelection();
                    }
                }
            }

        }

        private void myMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "e91ac1cb-c942-4e8e-b5b7-f47bedcde677";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "25G0rLFz7kDMYOFEX0yGcg";
        }
    }
}