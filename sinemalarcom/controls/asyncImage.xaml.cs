using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace sinemaci
{
    public partial class asyncImage : UserControl
    {
        System.Windows.Threading.DispatcherTimer LoadingTriggerTimer;

        public asyncImage()
        {
            InitializeComponent();
            LoadingTriggerTimer = new System.Windows.Threading.DispatcherTimer();
            LoadingTriggerTimer.Interval = TimeSpan.FromMilliseconds(1000);
            LoadingAnim = (Storyboard)this.Resources["anim_Loading"];
            this.Loaded += new RoutedEventHandler(loadingImage_Loaded);
        }

        void loadingImage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingTriggerTimer.Tick += LoadingTriggerTimer_Tick;
            LoadingAnim.Completed += new EventHandler(LoadingAnim_Completed);
        }

        void LoadingTriggerTimer_Tick(object sender, EventArgs e)
        {
            if (LoadComplete == false)
            {
                Storyboard SB = new Storyboard();
                DoubleAnimation DBL = new DoubleAnimation();
                DBL.To = 1;
                DBL.Duration = TimeSpan.FromMilliseconds(300);
                Storyboard.SetTarget(DBL, ellipse);
                Storyboard.SetTargetProperty(DBL, new PropertyPath(UIElement.OpacityProperty));
                SB.Children.Add(DBL);
                SB.Begin();

                LoadingAnim.Begin();
            }
            LoadingTriggerTimer.Stop();
        }

        void LoadingAnim_Completed(object sender, EventArgs e)
        {
            if (!LoadComplete)
            {
                LoadingAnim.Begin();
            }
            else
            {
                Storyboard SB = new Storyboard();
                DoubleAnimation DBL = new DoubleAnimation();
                DBL.To = 0;
                DBL.Duration = TimeSpan.FromMilliseconds(300);
                Storyboard.SetTarget(DBL, ellipse);
                Storyboard.SetTargetProperty(DBL, new PropertyPath(UIElement.OpacityProperty));
                SB.Children.Add(DBL);
                SB.Begin();
            }
        }

        public string FullURL
        {
            get { return (string)GetValue(FullURLProperty); }
            set
            {
                SetValue(FullURLProperty, value);
                ShowImage(FullURL);
            }
        }

        public static DependencyProperty FullURLProperty =
            DependencyProperty.Register("FullURL", typeof(string), typeof(asyncImage), new PropertyMetadata(new PropertyChangedCallback(FullURLProperty_PropertyChanged)));

        private static void FullURLProperty_PropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (source as asyncImage).ShowImage(e.NewValue.ToString());
            }
            else
            {
                (source as asyncImage).ClearImage();
            }
        }

        public Color LoaderColor
        {
            get { return (Color)GetValue(LoaderColorProperty); }
            set
            {
                SetValue(LoaderColorProperty, value);
                ChangeColor(LoaderColor);
            }
        }

        public static DependencyProperty LoaderColorProperty =
            DependencyProperty.Register("LoaderColor", typeof(Color), typeof(asyncImage), new PropertyMetadata(new PropertyChangedCallback(LoaderColorProperty_PropertyChanged)));

        private static void LoaderColorProperty_PropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as asyncImage).ChangeColor((Color)e.NewValue);
        }

        public void ChangeColor(Color C)
        {
            ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, C.R, C.G, C.B));
        }

        Storyboard LoadingAnim;
        bool LoadComplete = false;

        public void ShowImage(string fullURL)
        {
            if (!string.IsNullOrEmpty(fullURL))
            {
                LoadComplete = false;
                LoadingTriggerTimer.Start();               

                BitmapImage NewImage = new BitmapImage(new Uri(fullURL, UriKind.Absolute));
                NewImage.ImageOpened += NewImage_ImageOpened;
                ImageToShow.Source = NewImage;
            }
        }

        void NewImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            LoadComplete = true;
            if (ImageLoaded != null)
            {
                ImageLoaded();
            }
        }

        public void ClearImage()
        {
            ImageToShow.Source = null;
        }

        public delegate void ImageLoadedEvent();
        public event ImageLoadedEvent ImageLoaded;
    }
}
