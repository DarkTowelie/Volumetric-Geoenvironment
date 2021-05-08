using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Volumetric_Geoenvironment.ControlConstructors
{
    public static class PrTree
    {
        public static void Border()
        {
            MainWindow.b_ProjectTree.Width = 250;
            MainWindow.b_ProjectTree.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 100, 100, 100));
            MainWindow.b_ProjectTree.SetValue(Grid.RowProperty, 1);
            MainWindow.b_ProjectTree.BorderThickness = new Thickness(2);
            MainWindow.b_ProjectTree.HorizontalAlignment = HorizontalAlignment.Left;
        }

        public static void TreeView()
        {
            MainWindow.tv_ProjectTree.Width = 250;
            MainWindow.tv_ProjectTree.Background = new SolidColorBrush(Color.FromArgb(255, 45, 45, 48));
            MainWindow.tv_ProjectTree.SetValue(Grid.RowProperty, 1);
            MainWindow.tv_ProjectTree.HorizontalAlignment = HorizontalAlignment.Left;

            List<TreeViewItem> l = new List<TreeViewItem>();
            l.Add(new TreeViewItem());
            l.Add(new TreeViewItem());
            l.Add(new TreeViewItem());

            foreach(TreeViewItem el in l)
            {
                el.Foreground =  new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }

            l[0].Header = "123";
            l[1].Header = "321";
            l[2].Header = "432";

            MainWindow.tv_ProjectTree.Items.Add(l[0]);
            l[0].Items.Add(l[1]);
            l[1].Items.Add(l[2]);


        }
    }
}
