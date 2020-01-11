using System.Windows;

namespace EtaModemConfigurator.Types
{
    public class Wrapper : DependencyObject
    {
        public static readonly DependencyProperty TransportTypeIdProperty =
             DependencyProperty.Register("TransportTypeId", typeof(int),
             typeof(Wrapper), new FrameworkPropertyMetadata(null));

        public int TransportTypeId
        {
            get { return (int)GetValue(TransportTypeIdProperty); }
            set { SetValue(TransportTypeIdProperty, value); }
        }
    }
}
