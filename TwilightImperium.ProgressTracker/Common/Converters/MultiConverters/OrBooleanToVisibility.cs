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
   
    public class OrBooleanToVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue)) return Binding.DoNothing;
            try
            {
                foreach (var v in values)
                {
                    if ((bool)v) return Visibility.Visible;
                }
            }
            catch
            {

            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class AndBooleanToVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue)) return Binding.DoNothing;
            try
            {
                foreach (var v in values)
                {
                    if (!(bool)v) return Visibility.Collapsed;
                }
            }
            catch
            {

            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class InverseAndBooleanToVisibility : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue)) return Binding.DoNothing;
            try
            {
                foreach (var v in values)
                {
                    if ((bool)v) return Visibility.Collapsed;
                }
            }
            catch
            {

            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class InverseOrBooleanToVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue)) return Binding.DoNothing;
            try
            {
                foreach (var v in values)
                {
                    if ((bool)v) return Visibility.Collapsed;
                }
            }
            catch
            {

            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
