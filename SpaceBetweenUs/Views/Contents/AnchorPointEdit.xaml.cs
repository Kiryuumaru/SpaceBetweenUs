using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels.Contents;
using SpaceBetweenUs.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace SpaceBetweenUs.Views.Contents
{
    /// <summary>
    /// Interaction logic for Introduction.xaml
    /// </summary>
    public partial class AnchorPointEdit : UserControl
    {
        private static readonly Regex regex = new Regex(@"^(\d+(\.\d{0,2})?|\.?\d{1,2})$");

        private AnchorPointEditViewModel viewModel;

        public bool IsReferencedDepth;

        public AnchorPointEdit(Session session, Anchor anchor)
        {
            InitializeComponent();
            viewModel = new AnchorPointEditViewModel(session, anchor);
            DataContext = viewModel;
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string text = ((TextBox)e.Source).Text + e.Text;
            bool isMatch = regex.IsMatch(text);
            e.Handled = !isMatch;
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (regex.IsMatch(text))
                {
                    return;
                }
            }
            e.CancelCommand();
        }

        public void Save() => viewModel.Save();
    }
}

