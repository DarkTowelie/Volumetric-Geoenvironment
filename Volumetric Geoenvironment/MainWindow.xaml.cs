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
using Volumetric_Geoenvironment.ControlConstructors;

namespace Volumetric_Geoenvironment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public struct ControllVisibility
    {
        public Visibility PrTree;

    }

    public partial class MainWindow : Window
    {
        public static ControllVisibility controllVisibility = new ControllVisibility();
        public static TreeView tv_ProjectTree = new TreeView();
        public static Border b_ProjectTree = new Border();

        public MainWindow()
        {
            InitializeComponent();
            
            PrTree.Border();
            PrTree.TreeView();
            b_ProjectTree.Child = tv_ProjectTree;
            g_Main.Children.Add(b_ProjectTree);
        }

        public static void ControllsRefresh()
        {
            tv_ProjectTree.Visibility = controllVisibility.PrTree;
            b_ProjectTree.Visibility = controllVisibility.PrTree;
        }
    }
}
