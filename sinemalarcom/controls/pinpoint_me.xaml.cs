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

namespace sinemaci
{
    public partial class pinpoint_me : UserControl
    {
        public pinpoint_me()
        {
            InitializeComponent();
            this.Loaded += pinpoint_me_Loaded;
        }

        void pinpoint_me_Loaded(object sender, RoutedEventArgs e)
        {
            anim_Accuracy.Begin();
        }

        public void SetAccuracy(double AccuracyPixels)
        {
            anim_AccuracyValue1.Value = AccuracyPixels / 100;
            anim_AccuracyValue2.Value = AccuracyPixels / 100;
            anim_AccuracyResize.Begin();
        }
    }
}
