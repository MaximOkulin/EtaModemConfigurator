using EtaModemConfigurator.Types;
using System.Linq;
using System.Globalization;
using System.Windows.Controls;
using System.Collections.Generic;

namespace EtaModemConfigurator.ValidationRules
{
    public class DataFormatRule : ValidationRule
    {
        public Wrapper Wrapper { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Wrapper.TransportTypeId == (int)TransportTypes.Direct)
            {
              
            }

            return new ValidationResult(true, null);
        }
    }
}
