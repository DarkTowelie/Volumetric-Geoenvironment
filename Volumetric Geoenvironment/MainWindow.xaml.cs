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
using Volumetric_Geoenvironment.UserControls;
using VG_InputData.SEGY;
using System.ComponentModel;

namespace Volumetric_Geoenvironment
{
    public struct ControllVisibility
    {
        public Visibility PrTree;
    }

    public partial class MainWindow : Window
    {
        public static List<SEGY_3D> segy3D = new List<SEGY_3D>();
        int progress = 0;
        string seg_file_name = "E:\\Work\\Данные\\Проект для тестирования\\Fsrd_(-40).sgy";
        BackgroundWorker bw = new BackgroundWorker();

        public static ControllVisibility controllVisibility = new ControllVisibility();
        public static TreeView tv_ProjectTree = new TreeView();
        public static Border b_ProjectTree = new Border();

        public MainWindow()
        {
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            segy3D.Add(new SEGY_3D(ref progress, false, seg_file_name,
                        -9999, null, null, false,
                            "ms", "ms", ref bw));
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
