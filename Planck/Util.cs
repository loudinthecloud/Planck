using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Planck
{
    public static class Util
    {
        public static long TStoSEC(TimeSpan ts)
        {
            return (ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds);
        }

        public static TimeSpan SECtoTS(long seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, (int)seconds);
            return ts;
        }
    }

    // To be used in XAML
    public class SecToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long seconds = (long)value;
            TimeSpan ts = Util.SECtoTS(seconds);
            return string.Format("{0:hh\\:mm\\:ss}", ts);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
