using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TwilightImperium.ProgressTracker
{
    public class NumberValidationRule:ValidationRule
    {
        public long? MinValue { get; set; } = null;
        public long? MaxValue { get; set; } = null;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            long n;
            if (!long.TryParse(value?.ToString() ?? "", out n))
            {
                var msg = "Value must be a valid number";
                if (MinValue.HasValue && MaxValue.HasValue)
                    msg += $" between {MinValue.Value} and {MaxValue.Value}";
                else if (MinValue.HasValue)
                    msg += $" larger than {MinValue.Value}";
                else if (MaxValue.HasValue) msg += $" smaller than {MaxValue.Value}";
                return new ValidationResult(false, msg);
            }
            if (MinValue.HasValue && MaxValue.HasValue && 
                (n<MinValue.Value || n> MaxValue.Value))
                return new ValidationResult(false, $"Value must be between {MinValue.Value} and {MaxValue.Value}");
            if (MinValue.HasValue && n< MinValue.Value)
                return new ValidationResult(false, $"Value must be larger than {MinValue.Value}");
            if (MaxValue.HasValue && n > MaxValue.Value)
                return new ValidationResult(false, $"Value must be smaller than {MaxValue.Value}");
            return ValidationResult.ValidResult;
        }
    }
}
