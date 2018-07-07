using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TwilightImperium.Tracker
{
    public class ScaleMarginCalculator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            FrameworkElement f = value as FrameworkElement;
            double scaleRatio = double.Parse((string) parameter);
            if (f == null) return new Thickness(0);

            var w = f.ActualWidth*scaleRatio;
            var h = f.ActualWidth*scaleRatio;
            return new Thickness(w*-0.5, h*-0.5, w*-0.5, h*-0.5);

            bool v = (bool)value;
            return v ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
