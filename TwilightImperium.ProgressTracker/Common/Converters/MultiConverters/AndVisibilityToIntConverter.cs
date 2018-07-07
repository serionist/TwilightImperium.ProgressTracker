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
    
    public class AndVisibilityToIntConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string p = parameter.ToString();
            string[] pa = p.Split('|');

            try
            {
                foreach (var v in values)
                {
                    if ((Visibility)v == Visibility.Collapsed) return int.Parse(pa[0]);
                }
            }
            catch
            {

            }
            return int.Parse(pa[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
