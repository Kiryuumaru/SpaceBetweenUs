using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels.Pages;
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

namespace SpaceBetweenUs.Views.Pages
{
    /// <summary>
    /// Interaction logic for GridPerspective.xaml
    /// </summary>
    public partial class GridPerspective : UserControl
    {
        private readonly GridPerspectiveViewModel viewModel = new GridPerspectiveViewModel();
        private bool isMouseDown = false;
        private Anchor? selectedAnchor;

        public GridPerspective()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void FrameHolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
        }

        private void FrameHolder_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void FrameHolder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            buttonBL.Content = "Bottom Left";
            buttonTL.Content = "Top Left";
            buttonTR.Content = "Top Right";
            buttonBR.Content = "Bottom Right";

            var button = (Button)sender;
            if (button == buttonBL)
            {
                if (selectedAnchor.HasValue && selectedAnchor.Value == Anchor.BottomLeft) selectedAnchor = null;
                else
                {
                    button.Content = "Cancel";
                    selectedAnchor = Anchor.BottomLeft;
                }
            }
            else if (button == buttonTL)
            {
                if (selectedAnchor.HasValue && selectedAnchor.Value == Anchor.TopLeft) selectedAnchor = null;
                else
                {
                    button.Content = "Cancel";
                    selectedAnchor = Anchor.TopLeft;
                }
            }
            else if (button == buttonTR)
            {
                if (selectedAnchor.HasValue && selectedAnchor.Value == Anchor.TopRight) selectedAnchor = null;
                else
                {
                    button.Content = "Cancel";
                    selectedAnchor = Anchor.TopRight;
                }
            }
            else if (button == buttonBR)
            {
                if (selectedAnchor.HasValue && selectedAnchor.Value == Anchor.BottomRight) selectedAnchor = null;
                else
                {
                    button.Content = "Cancel";
                    selectedAnchor = Anchor.BottomRight;
                }
            }
        }

        private static readonly Regex regexUInt = new Regex(@"^[0-9]*$");
        private void TextBox_PreviewTextInputUInt(object sender, TextCompositionEventArgs e)
        {
            var text = ((TextBox)e.Source).Text + e.Text;
            e.Handled = !regexUInt.IsMatch(text);
        }
        private void TextBox_PastingUInt(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!regexUInt.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static readonly Regex regexUDouble = new Regex(@"^(\d+(\.\d{0,2})?|\.?\d{1,2})$");
        private void TextBox_PreviewTextInputUDouble(object sender, TextCompositionEventArgs e)
        {
            var text = ((TextBox)e.Source).Text + e.Text;
            e.Handled = !regexUDouble.IsMatch(text);
        }
        private void TextBox_PastingUDouble(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!regexUDouble.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
