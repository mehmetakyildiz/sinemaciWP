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

namespace sinemaci
{
    public partial class pinpoint_salon : UserControl
    {
        private bool _ShowDetails = true;
        public bool ShowDetails { get { return _ShowDetails; } set { _ShowDetails = value; } }

        public Salonlar.Salon CurrentSalon { get; set; }

        public pinpoint_salon()
        {
            InitializeComponent();
            check.Checked += check_Checked;
            check.Unchecked += check_Unchecked;
            image.Tap += image_Tap;
        }

        void image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Redirect2Theater != null)
            {
                Redirect2Theater(CurrentSalon.id);
            }
        }

        public delegate void goRedirect2Theater(string TheaterID);
        public event goRedirect2Theater Redirect2Theater;

        public delegate void gotSelected(pinpoint_salon sender, Salonlar.Salon salon);
        public event gotSelected Selected;

        public delegate void gotDeSelected(pinpoint_salon sender, Salonlar.Salon salon);
        public event gotSelected DeSelected;

        void check_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ShowDetails)
            {
                VisualStateManager.GoToState(this, "Invisible", true);
            }            
            if (DeSelected != null)
            {
                DeSelected(this, CurrentSalon);
            }
        }

        void check_Checked(object sender, RoutedEventArgs e)
        {
            if (ShowDetails)
            {
                VisualStateManager.GoToState(this, "Visible", true);
            }            
            if(Selected != null)
            {
                Selected(this, CurrentSalon);
            }
        }

        public void ClearSelection()
        {
            check.IsChecked = false;
        }

        public void SetContent(Salonlar.Salon MevcutSalon)
        {
            CurrentSalon = MevcutSalon;
            txtSalonAdi.Text = MevcutSalon.name;
            txtSalonTel.Text = MevcutSalon.phone;
            txtSalonMesafe.Text = MevcutSalon.distance;
        }
    }
}
