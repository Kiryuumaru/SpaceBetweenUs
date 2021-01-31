using FirstFloor.ModernUI.Presentation;
using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels.Pages;
using SpaceBetweenUs.Views.Contents;
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
    /// Interaction logic for MLModel.xaml
    /// </summary>
    public partial class MachineLearning : UserControl
    {
        public MachineLearningViewModel viewModel = new MachineLearningViewModel();

        public MachineLearning()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ModernTab_SelectedSourceChanged(object sender, FirstFloor.ModernUI.Windows.Controls.SourceEventArgs e)
        {
            int index = e.Source.OriginalString.IndexOf("Name=");
            string name = e.Source.OriginalString.Substring(index + 5);
            MachineLearningViewModel.JumperCurrentModelSelect = name;
            MachineLearningViewModel.JumperModelTabChange?.Invoke();
        }
    }
}
