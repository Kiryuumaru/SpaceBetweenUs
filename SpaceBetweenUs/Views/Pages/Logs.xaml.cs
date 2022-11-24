using SpaceBetweenUs.Model;
using SpaceBetweenUs.ViewModels.Pages;
using SpaceBetweenUs.Views.Windows;
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

namespace SpaceBetweenUs.Views.Pages
{
    /// <summary>
    /// Interaction logic for Introduction.xaml
    /// </summary>
    public partial class Logs : UserControl
    {
        private LogsViewModel viewModel;
        private bool isLoaded = false;

        public Logs()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(delegate
            {
                if (isLoaded)
                {
                    return;
                }

                isLoaded = true;
                InstanceWindow window = (InstanceWindow)Window.GetWindow(this);
                viewModel = new LogsViewModel(window.Session, Dispatcher);
                DataContext = viewModel;
            });
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            if (Key.Delete == e.Key)
            {
                foreach (object row in grid.SelectedItems)
                {
                    ViolationLog log = (ViolationLog)row;
                    log.Delete();
                }
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            if (grid.SelectedItems.Count == 1)
            {
                viewModel.SelectFrame((ViolationLog)grid.SelectedItems[0]);
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
