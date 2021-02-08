using SpaceBetweenUs.Model;
using SpaceBetweenUs.ViewModels.Pages;
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
        private readonly LogsViewModel viewModel;

        public Logs()
        {
            InitializeComponent();
            viewModel = new LogsViewModel();
            DataContext = viewModel;
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            if (Key.Delete == e.Key)
            {
                foreach (var row in grid.SelectedItems)
                {
                    var log = (ViolationLog)row;
                    log.Delete();
                }
            }
        }
    }
}
