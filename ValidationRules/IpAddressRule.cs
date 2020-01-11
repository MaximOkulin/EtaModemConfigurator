using System.Windows.Controls;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EtaModemConfigurator.ValidationRules
{
    public class IpAddressRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Match match = Regex.Match((string)value, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");

            if (!match.Success)
            {
                return new ValidationResult(false, "Ip-адрес должен быть задан в правильном формате");
            }

            return new ValidationResult(true, null);
        }       
    }
}
