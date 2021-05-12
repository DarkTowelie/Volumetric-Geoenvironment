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
using VG_InputData.EXCEL.Well_logging;

namespace Volumetric_Geoenvironment
{
    public struct ControllVisibility
    {
        public Visibility PrTree;
    }

    public partial class MainWindow : Window
    {
        string segyPath = "E:/Work/Данные/Проект для тестирования/Fsrd_(-40).sgy";
        int pr = 0;

        public static ControllVisibility controllVisibility = new ControllVisibility();
        public static TreeView tv_ProjectTree = new TreeView();
        public static Border b_ProjectTree = new Border();

        public MainWindow()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            SEGY_3D seg = new SEGY_3D(ref pr, false, segyPath, 0, null, null, Metric.MS, Metric.MKS, ref bw);
            ExcelData wellLog = new ExcelData("E:\\Work\\Данные\\Проект для тестирования\\Фес_по_срезам.xlsx", "ANGK", -999);
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
