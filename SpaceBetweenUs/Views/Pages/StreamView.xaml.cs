using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels.Pages;
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

namespace SpaceBetweenUs.Views.Pages
{
    /// <summary>
    /// Interaction logic for StreamView.xaml
    /// </summary>
    public partial class StreamView : UserControl
    {
        private static readonly Regex regex = new Regex(@"^(\d+(\.\d{0,2})?|\.?\d{1,2})$");
        private StreamViewViewModel viewModel;
        private bool isMouseLeaveWithDown = false;
        private bool isLoaded = false;

        public StreamView()
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
                viewModel = new StreamViewViewModel(window.Session, Dispatcher);
                DataContext = viewModel;
                window.Closing += delegate
                {
                    viewModel.Stop();
                };
            });
        }

        private RelativePoint GetRelativePoint(MouseEventArgs e)
        {
            Point viewPoint = e.GetPosition(frameHolder);
            OpenCvSharp.Point imagePoint = new OpenCvSharp.Point(
                frameHolder.Source.Width * (viewPoint.X / frameHolder.ActualWidth),
                frameHolder.Source.Height * (viewPoint.Y / frameHolder.ActualHeight));
            return RelativePoint.FromFrame(imagePoint, frameHolder.Source.Width, frameHolder.Source.Height);
        }

        private void FrameHolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                viewModel.PointerDown(GetRelativePoint(e));
            }
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                viewModel.PointerDoubleDown(GetRelativePoint(e));
            }
        }

        private void FrameHolder_MouseMove(object sender, MouseEventArgs e)
        {
            viewModel.PointerMove(GetRelativePoint(e));
        }

        private void FrameHolder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.PointerUp();
        }

        private void FrameHolder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isMouseLeaveWithDown && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                viewModel.PointerMove(GetRelativePoint(e));
            }
            else
            {
                viewModel.PointerUp();
            }
            isMouseLeaveWithDown = false;
        }

        private void FrameHolder_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseLeaveWithDown = Mouse.LeftButton == MouseButtonState.Pressed;
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
                bool isMatch = regex.IsMatch(text);
                if (isMatch)
                {
                    return;
                }
            }
            e.CancelCommand();
        }
    }
}
