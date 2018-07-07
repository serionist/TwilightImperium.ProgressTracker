using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TwilightImperium.ProgressTracker
{
    public class UrlValidationRule:ValidationRule
    {
        public UriKind Kind { get; set; } = UriKind.Absolute;
        public bool Required { get; set; } = true;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string s = value as string;
            if (Required && string.IsNullOrEmpty(s))
                return new ValidationResult(false, "Url must not be empty");
            Uri a;
            if (string.IsNullOrEmpty(s) || Uri.TryCreate(s, Kind, out a))
                return ValidationResult.ValidResult;
           return new ValidationResult(false, "Url must be of valid format");
        }
    }
}
