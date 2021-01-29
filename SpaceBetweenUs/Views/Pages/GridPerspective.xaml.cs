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

        public GridPerspective()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void SelectAxis(MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && viewModel.SelectedEditAnchor.HasValue)
            {
                var viewPoint = e.GetPosition(frameHolder);
                var imagePoint = new OpenCvSharp.Point(
                    frameHolder.Source.Width * (viewPoint.X / frameHolder.ActualWidth),
                    frameHolder.Source.Height * (viewPoint.Y / frameHolder.ActualHeight));
                var point = RelativePoint.FromFrame(imagePoint, frameHolder.Source.Width, frameHolder.Source.Height);
                viewModel.SetAnchorAxis(viewModel.SelectedEditAnchor.Value, point);
            }
        }

        private void FrameHolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectAxis(e);
        }

        private void FrameHolder_MouseMove(object sender, MouseEventArgs e)
        {
            SelectAxis(e);
        }

        private void FrameHolder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.SetAnchorPersistent();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SetAnchorPersistent();
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
                if (viewModel.SelectedEditAnchor.HasValue && viewModel.SelectedEditAnchor.Value == Anchor.BottomLeft) viewModel.SelectedEditAnchor = null;
                else
                {
                    button.Content = "Done";
                    viewModel.SelectedEditAnchor = Anchor.BottomLeft;
                }
            }
            else if (button == buttonTL)
            {
                if (viewModel.SelectedEditAnchor.HasValue && viewModel.SelectedEditAnchor.Value == Anchor.TopLeft) viewModel.SelectedEditAnchor = null;
                else
                {
                    button.Content = "Done";
                    viewModel.SelectedEditAnchor = Anchor.TopLeft;
                }
            }
            else if (button == buttonTR)
            {
                if (viewModel.SelectedEditAnchor.HasValue && viewModel.SelectedEditAnchor.Value == Anchor.TopRight) viewModel.SelectedEditAnchor = null;
                else
                {
                    button.Content = "Done";
                    viewModel.SelectedEditAnchor = Anchor.TopRight;
                }
            }
            else if (button == buttonBR)
            {
                if (viewModel.SelectedEditAnchor.HasValue && viewModel.SelectedEditAnchor.Value == Anchor.BottomRight) viewModel.SelectedEditAnchor = null;
                else
                {
                    button.Content = "Done";
                    viewModel.SelectedEditAnchor = Anchor.BottomRight;
                }
            }

            viewModel.DrawResult();
        }

        private static readonly Regex regex = new Regex(@"^(\d+(\.\d{0,2})?|\.?\d{1,2})$");
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var text = ((TextBox)e.Source).Text + e.Text;
            bool isMatch = regex.IsMatch(text);
            e.Handled = !isMatch;
            if (isMatch) viewModel.SetAnchorPersistent();
        }
        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!regex.IsMatch(text))
                {
                    e.CancelCommand();
                }
                else
                {
                    viewModel.SetAnchorPersistent();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
