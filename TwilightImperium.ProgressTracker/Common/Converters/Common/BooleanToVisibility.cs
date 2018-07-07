using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TwilightImperium.Tracker
{
    public class BooleanToVisibility: IValueConverter
    {
        public Visibility DesignValue { get; set; } = Visibility.Visible;
        private static DependencyObject dp = new DependencyObject();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return DesignValue;
            if (value == null) return Visibility.Collapsed;
            bool v = (bool) value;
            return v ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    public class InverseBooleanToVisibility : IValueConverter
    {
        public Visibility DesignValue { get; set; } = Visibility.Visible;
        private static DependencyObject dp = new DependencyObject();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return DesignValue;
            if (value == null) return Visibility.Collapsed;
            bool v = (bool)value;
            return !v ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    public class BooleanToVisibilityHidden : IValueConverter
    {
        public Visibility DesignValue { get; set; } = Visibility.Visible;
        private static DependencyObject dp = new DependencyObject();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return DesignValue;
            if (value == null) return Visibility.Collapsed;
            bool v = (bool)value;
            return v ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    public class InverseBooleanToVisibilityHidden : IValueConverter
    {
        public Visibility DesignValue { get; set; } = Visibility.Visible;
        private static DependencyObject dp = new DependencyObject();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return DesignValue;
            if (value == null) return Visibility.Collapsed;
            bool v = (bool)value;
            return !v ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
