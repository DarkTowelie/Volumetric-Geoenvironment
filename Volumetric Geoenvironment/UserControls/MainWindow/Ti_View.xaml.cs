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

namespace Volumetric_Geoenvironment.UserControls
{
    /// <summary>
    /// Interaction logic for Ti_View.xaml
    /// </summary>
    public partial class Ti_View : UserControl
    {
        public Ti_View()
        {
            InitializeComponent();
        }

        private void mb_PrTree_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.controllVisibility.PrTree == Visibility.Visible)
            {
                MainWindow.controllVisibility.PrTree = Visibility.Hidden;
            }
            else
            {
                MainWindow.controllVisibility.PrTree = Visibility.Visible;
            }
            MainWindow.ControllsRefresh();
        }
    }
}
