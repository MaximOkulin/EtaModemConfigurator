using EtaModemConfigurator.ViewModels;
using System.Windows.Controls;

namespace EtaModemConfigurator.Views
{
    /// <summary>
    /// Interaction logic for ClientView.xaml
    /// </summary>
    public partial class ClientView : UserControl
    {
        public ClientView()
        {
            InitializeComponent();
            DataContext = new ClientViewModel();
        }
    }
}
