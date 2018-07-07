using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TwilightImperium.ProgressTracker
{
    public class StringValidationRule:ValidationRule
    {
        public bool Required { get; set; } = true;
        public int? MinLength { get; set; } = null;
        public int? MaxLength { get; set; } = null;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = value as string;
            if (string.IsNullOrEmpty(s))
                if (!Required)
                    return ValidationResult.ValidResult;
                else
                    return new ValidationResult(false, "Field is required");
            
            if (MinLength.HasValue && s.Length < MinLength.Value)
                return new ValidationResult(false,getMsg());
            if (MaxLength.HasValue && s.Length > MaxLength.Value)
                return new ValidationResult(false,getMsg());
            return ValidationResult.ValidResult;
        }

        private string getMsg()
        {
            if (!MinLength.HasValue && MaxLength.HasValue)
                return $"Field must not be longer than {MaxLength} characters";
            if (MinLength.HasValue && !MaxLength.HasValue)
                return $"Field must be at least {MaxLength} characters long";
            if (MinLength.HasValue&&MaxLength.HasValue)
                if (MinLength == MaxLength)
                    return $"Field must be {MaxLength} characters long";
                else return $"Field must be {MinLength}-{MaxLength} characters long";
            return "Field is required";
        }
    }
}
