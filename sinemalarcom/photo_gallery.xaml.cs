using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using sinemaci.entities;
using System.Threading;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media.PhoneExtensions;

namespace sinemaci
{
    public partial class photo_gallery : PhoneApplicationPage
    {
        public List<string> fotolar { get; set; }
        private int CurrentIndex = 0;

        public photo_gallery()
        {
            InitializeComponent();
            PhotoPivot.SelectionChanged += PhotoPivot_SelectionChanged;
        }

        void PhotoPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PhotoPivot.SelectedItem != null)
            {
                PivotItem current = PhotoPivot.SelectedItem as PivotItem;
                asyncImage currentContent = current.Content as asyncImage;
                currentContent.FullURL = currentContent.Tag as string;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            myProgress.Opacity = 1;
            myProgress.IsIndeterminate = true;
            //artist
            string Type = NavigationContext.QueryString["type"];
            if (Type == "artist")
            {
                ArtistPhotos photos = new ArtistPhotos();
                photos.getFailed += photos_getFailed;
                photos.getCompleted += photos_getCompleted;
                photos.get(NavigationContext.QueryString["ID"]);
            }
            else if (Type == "movie")
            {
                MoviePhotos movies = new MoviePhotos();
                movies.getFailed += movies_getFailed;
                movies.getCompleted += movies_getCompleted;
                movies.get(NavigationContext.QueryString["ID"]);
            }  
            
            base.OnNavigatedTo(e);
        }

        void photos_getFailed(ArtistPhotos sender)
        {
            myProgress.IsIndeterminate = false;
            myProgress.Value = 0;
            myProgress.Opacity = 0;
            NavigationService.GoBack();
        }

        void movies_getFailed(MoviePhotos sender)
        {            
            myProgress.IsIndeterminate = false;
            myProgress.Value = 0;
            myProgress.Opacity = 0;
            NavigationService.GoBack();
        }

        void movies_getCompleted(MoviePhotos sender)
        {
            if (sender.fotolar.Count == 0)
            {
                MessageBox.Show("Filme ait fotoğraf albümü mevcut değil.");
                NavigationService.GoBack();
                return;
            }

            fotolar = sender.fotolar.Take(20).ToList();

            foreach (string item in fotolar)
            {
                PivotItem NewPivotItem = new PivotItem();
                NewPivotItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                NewPivotItem.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                NewPivotItem.Margin = new Thickness(0);
                asyncImage IMG = new asyncImage();
                IMG.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                IMG.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                IMG.Margin = new Thickness(0);
                NewPivotItem.Content = IMG;
                PhotoPivot.Items.Add(NewPivotItem);
                IMG.Tag = item;
            }
            asyncImage FirstOne = ((asyncImage)((PivotItem)PhotoPivot.Items[0]).Content);
            FirstOne.FullURL = FirstOne.Tag as string;

            myProgress.IsIndeterminate = false;
        }

        void photos_getCompleted(ArtistPhotos sender)
        {
            if (sender.fotolar.Count == 0)
            {
                MessageBox.Show("Oyuncuya ait fotoğraf albümü mevcut değil.");
                NavigationService.GoBack();
                return;
            }

            fotolar = sender.fotolar.Take(20).ToList();

            foreach (string item in fotolar)
            {
                PivotItem NewPivotItem = new PivotItem();
                NewPivotItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                NewPivotItem.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                NewPivotItem.Margin = new Thickness(0);
                asyncImage IMG = new asyncImage();
                IMG.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                IMG.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                IMG.Margin = new Thickness(0);
                NewPivotItem.Content = IMG;
                PhotoPivot.Items.Add(NewPivotItem);
                IMG.Tag = item;
            }
            asyncImage FirstOne = ((asyncImage)((PivotItem)PhotoPivot.Items[0]).Content);
            FirstOne.FullURL = FirstOne.Tag as string;

            myProgress.IsIndeterminate = false;
        }

        private void barbtn_save(object sender, EventArgs e)
        {
            if (PhotoPivot.SelectedItem != null)
            {
                asyncImage selectedOne = ((asyncImage)((PivotItem)PhotoPivot.SelectedItem).Content);
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += wc_OpenReadCompleted;
                wc.OpenReadAsync(new Uri(selectedOne.FullURL));
            }
            else
            {
                MessageBox.Show("Lüfen bir resim seçin.");
            }
        }

        void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                using (Stream stream = e.Result)  
                {  
                    MediaLibrary mediaLibrary = new MediaLibrary();
                    mediaLibrary.SavePicture(NavigationContext.QueryString["name"] + ".jpg", stream);
                    MessageBox.Show("Resim telefonunuza kaydedildi.");
                }  
            }
            catch (Exception)
            {
                MessageBox.Show("Resim telefonunuza indirilemedi. Lütfen tekrar deneyin.");
            }
        }

        private void barbtn_share(object sender, EventArgs e)
        {
            if (PhotoPivot.SelectedItem != null)
            {
                asyncImage selectedOne = ((asyncImage)((PivotItem)PhotoPivot.SelectedItem).Content);
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += wc_OpenReadCompleted_4Share;
                wc.OpenReadAsync(new Uri(selectedOne.FullURL));
            }
            else
            {
                MessageBox.Show("Lüfen bir resim seçin.");
            }
        }

        void wc_OpenReadCompleted_4Share(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                using (Stream stream = e.Result)
                {
                    MediaLibrary mediaLibrary = new MediaLibrary();
                    Picture pic = mediaLibrary.SavePicture(NavigationContext.QueryString["name"] + ".jpg", stream);
                    var task = new ShareMediaTask();
                    task.FilePath = pic.GetPath();
                    task.Show();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Resim paylaşılmak üzere telefonunuza indirilemedi. Lütfen tekrar deneyin.");
            }
        }

    }
}