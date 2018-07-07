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
    public class IntegerToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int v = (int) value;
                string p = (string) parameter;
                List<double> Params = new List<double>();
                if (!p.Contains("|"))
                {
                    Params.Add(double.Parse(p));
                }
                else
                {
                    string[] parameters = p.Split('|');
                    foreach (var param in parameters)
                    {
                        double o;
                        if (double.TryParse(param, out o))
                        {
                            Params.Add(o);
                        }
                    }
                }

                return Params.Contains(v);
            }
            catch
            {
                return false;
            }
          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(bool) value || ((string)parameter).Contains("|")) return Binding.DoNothing;
            if (targetType == typeof(int)) return int.Parse((string)parameter);
            if (targetType == typeof(long)) return long.Parse((string)parameter);
            if (targetType.IsEnum)
            {
                object ret = Enum.ToObject(targetType, int.Parse((string) parameter));
                return ret;
            }
            return Binding.DoNothing;
        }

    }
    public class InverseIntegerToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool) new IntegerToBoolean().Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class DoubleToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double v = (double)value;
            string p = (string)parameter;
            List<double> Params = new List<double>();
            if (!p.Contains("|"))
            {
                Params.Add(double.Parse(p));
            }
            else
            {
                string[] parameters = p.Split('|');
                foreach (var param in parameters)
                {
                    double o;
                    if (double.TryParse(param, out o)) { Params.Add(o); }
                }
            }

            return Params.Contains(v);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    public class InverseDoubleToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)new DoubleToBoolean().Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
