using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TimeSpan = System.TimeSpan;

namespace TwilightImperium.ProgressTracker.Common.Converters.Common
{
    public class TimespanConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
                return ((TimeSpan)value).ToString("hh\\:mm\\:ss\\.f");
            else 
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (TimeSpan.TryParse(value as string, out var t))
                return t;
            return Binding.DoNothing;
        }
    }
}
