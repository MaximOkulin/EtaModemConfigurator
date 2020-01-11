using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections.Generic;
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

namespace EtaModemConfigurator.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void IfMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Tile)
            {
                ((Tile)sender).BorderBrush = Brushes.DimGray;
                ((Tile)sender).BorderThickness = new Thickness(3);
                ((Tile)sender).FontWeight = FontWeights.Normal;
                return;
            }
        }

        private void IfMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Tile)
            {
                ((Tile)sender).BorderBrush = Brushes.Orange;
                ((Tile)sender).BorderThickness = new Thickness(5);
                ((Tile)sender).FontWeight = FontWeights.Bold;
                return;
            }
        }
    }
}
