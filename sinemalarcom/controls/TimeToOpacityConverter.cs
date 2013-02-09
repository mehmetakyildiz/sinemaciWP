using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace sinemaci.controls
{
    public class TimeToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "false")
            {
                return 0;
            }
            int gelenSaat = int.Parse(value.ToString().Substring(0,2));
            int gelenDakika = int.Parse(value.ToString().Substring(3, 2));

            int anlikSaat = DateTime.Now.Hour;
            int anlikDakika = DateTime.Now.Minute;

            if (anlikSaat == 0)
            {
                anlikSaat = 24;
            }

            if (anlikSaat > gelenSaat)
            {
                return 0.5;
            }
            if (anlikSaat == gelenSaat)
            {
                if (anlikDakika > gelenDakika)
                {
                    return 0.5;
                }
            }

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
