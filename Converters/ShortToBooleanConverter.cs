using System;
using System.Globalization;
using System.Windows.Data;

namespace EtaModemConfigurator.Converters
{
    public class ShortToBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (short)value == 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                    return 1;
                return 0;
            }

            return 0;
        }
    }
}
