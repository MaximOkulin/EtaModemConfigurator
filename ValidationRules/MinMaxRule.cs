using System;
using System.Globalization;
using System.Windows.Controls;

namespace EtaModemConfigurator.ValidationRules
{
    public class MinMaxRule : ValidationRule
    {
        private int _min;
        private int _max;

        public int Min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
            }
        }

        public int Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int servCount = 0;

            try
            {
                servCount = int.Parse((string)value);
            }
            catch
            {
                return new ValidationResult(false, "Недопустимые символы");
            }

            if (servCount < _min || servCount > _max)
            {
                return new ValidationResult(false, string.Format("Значение может находиться в диапазоне от {0} до {1}", _min, _max));
            }

            return new ValidationResult(true, null);
        }
    }
}
