using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TwilightImperium.ProgressTracker
{
    public class SqlConnectionStringValidationRule : ValidationRule
    {
        public bool Required { get; set; } = true;
        public string[] RequiredParameters { get; set; } = new[] { "Data source", "User Id", "Initial Catalog"};
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string s = value as string;
            if (string.IsNullOrEmpty(s))
                if (Required)
                    return new ValidationResult(false, "Field must not be empty");
                else return ValidationResult.ValidResult;

            try
            {
                var a = new SqlConnectionStringBuilder(s);
                var missingParams = new List<string>();
                object val = null;
                foreach (var p in RequiredParameters ?? new string[0])
                    if (!a.TryGetValue(p, out val) || string.IsNullOrEmpty(val as string))
                        missingParams.Add(p);
                if (missingParams.Any())
                    return new ValidationResult(false, $"Parameters missing: {string.Join(",", missingParams)}");
                return ValidationResult.ValidResult;
            }
            catch
            {
                return new ValidationResult(false, "ODBC Connection string is invalid");
            }
        }
    }
}
