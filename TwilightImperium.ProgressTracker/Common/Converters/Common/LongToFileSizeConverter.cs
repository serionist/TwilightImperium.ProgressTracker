using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TwilightImperium.ProgressTracker;

namespace TwilightImperium.Tracker
{
 

    public class LongToFileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int decimals = 0;
            if (parameter is int) decimals = (int) parameter;
            if (parameter != null) int.TryParse(parameter.ToString(), out decimals);

            if (!(value is long))
            {
                return "";
            }
            return ((long) value).FileSizeToShortString(decimals);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
