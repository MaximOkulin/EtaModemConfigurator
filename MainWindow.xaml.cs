using System.Security.Principal;
using System.Windows;

namespace EtaModemConfigurator
{
    /// <summary>
    ///// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowsPrincipal p = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            Title = @"Конфигуратор gprs-модемов 'ЭнергоТехАудит'";
            if (p.IsInRole(WindowsBuiltInRole.Administrator))
            {
                Title += " (Administrator)";
            }
            lblVersionInfo.Content = "ООО \"ЭнергоТехАудит\" (версия ПО: 2.18)";
        }
    }
}
