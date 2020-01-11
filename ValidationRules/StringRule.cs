using System;
using System.Globalization;
using System.Windows.Controls;

namespace EtaModemConfigurator.ValidationRules
{
    public class StringRule : ValidationRule
    {
        private int _minLength;
        private int _maxLength;

        public int MinLength
        {
            get
            {
                return _minLength;
            }
            set
            {
                _minLength = value;
            }
        }

        public int MaxLength
        {
            get
            {
                return _maxLength;
            }
            set
            {
                _maxLength = value;
            }
        }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int currentLength = ((string)value).Length;

            if (currentLength < _minLength || currentLength > _maxLength)
            {
                return new ValidationResult(false, string.Format("Длина может быть не более {0} символов", _maxLength));
            }

            return new ValidationResult(true, null);
        }
    }
}
