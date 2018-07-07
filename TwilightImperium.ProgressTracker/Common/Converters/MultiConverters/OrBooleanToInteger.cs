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
    
    public class OrBooleanToInteger : IMultiValueConverter
    {
        /// <summary>
        /// Expects parameter [d1]|[d2] where d
        /// d1: value to return when success
        /// d2: value when failed
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string p = parameter.ToString();
            string[] param = p.Split('|');

            try
            {
                foreach (var v in values)
                {
                    if ((bool) v) return int.Parse(param[0]);
                }
            }
            catch
            {

            }
            return int.Parse(param[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class AndBooleanToInteger : IMultiValueConverter
    {
        /// <summary>
        /// Expects parameter [d1]|[d2] where d
        /// d1: value to return when success
        /// d2: value when failed
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string p = parameter.ToString();
            string[] param = p.Split('|');

            try
            {
                foreach (var v in values)
                {
                    if (!(bool)v) return int.Parse(param[1]);
                }
            }
            catch
            {

            }
            return int.Parse(param[0]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
