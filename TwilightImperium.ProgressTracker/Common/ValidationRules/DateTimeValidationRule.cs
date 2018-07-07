using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TwilightImperium.ProgressTracker
{
    public class DateTimeValidationRule : ValidationRule
    {
        public string format { get; set; } = "MM/dd/yyyy hh:mm:ss";
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string s;
            DateTime dt;
            if (string.IsNullOrEmpty(s = value as string) || DateTime.TryParseExact(s,format, new DateTimeFormatInfo() {LongDatePattern = format, ShortDatePattern = format}, DateTimeStyles.AllowWhiteSpaces, out dt))
                return new ValidationResult(true, null);

            return new ValidationResult(false, $"Invalid date. Format eg: '{DateTime.UtcNow:G}'");

        }
    }
}
