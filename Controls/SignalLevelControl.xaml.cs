using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EtaModemConfigurator.Controls
{
    /// <summary>
    /// Interaction logic for SignalLevelControl.xaml
    /// </summary>
    public partial class SignalLevelControl : UserControl
    {
        public SignalLevelControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public int RatingValue
        {
            get { return (int)GetValue(RatingValueProperty); }
            set { SetValue(RatingValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RatingValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RatingValueProperty =
            DependencyProperty.Register("RatingValue", typeof(int), typeof(SignalLevelControl), null);
    }

    public class RatingConverter : IValueConverter
    {
        public Brush OnBrush { get; set; }
        public Brush OffBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int rating = 0;
            int number = 0;
            if (int.TryParse(value.ToString(), out rating) && int.TryParse(parameter.ToString(), out number))
            {
                if (rating >= number)
                {
                    return OnBrush;
                }
                return OffBrush;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
