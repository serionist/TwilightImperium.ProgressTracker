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
    public class DoubleToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value.ToString();
            if (!string.IsNullOrEmpty(parameter as string))
            {
                int round = 0;
                if (int.TryParse((string) parameter, out round))
                {
                    if (value is double)
                    {
                        val = Math.Round((double) value, round).ToString();
                    }
                }
            }
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string) value;
            List<char> digits = new List<char>() {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
            string newText = "";
            foreach (char c in val)
            {
                if (c == '.' && newText.Length != 0)
                {
                    newText += ".";
                }
                else if (digits.Contains(c))
                {
                    newText += c;
                }
            }
            if (newText.Length == 0 || newText == ".")
            {
                return 0;
            }
            if (newText[newText.Length - 1] == '.')
            {
                newText = newText.Substring(0, newText.Length - 1);
            }
            return double.Parse(newText);

        }
    }

    public class IntegerToString : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value.ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string val = (string)value;
                List<char> digits = new List<char>() { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
                string newText = "";
                foreach (char c in val)
                {
                    if (digits.Contains(c))
                    {
                        newText += c;
                    }
                }
                if (newText.Length == 0)
                {
                    return 0;
                }
                
                return int.Parse(newText);

            }

        }
}
