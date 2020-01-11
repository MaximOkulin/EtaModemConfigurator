using DevExpress.Xpf.WindowsUI;
using DevExpress.XtraSplashScreen;
using EtaModemConfigurator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

namespace EtaModemConfigurator.Views
{
    /// <summary>
    /// Interaction logic for View1.xaml
    /// </summary>
    public partial class ComPortSettingsView : UserControl
    {
        public ComPortSettingsView()
        {
            InitializeComponent();
            DataContext = new ComPortSettingsViewModel();
        }       
    }
}
