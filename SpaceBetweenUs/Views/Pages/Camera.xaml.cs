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
    public partial class Camera : UserControl
    {
        private readonly CameraViewModel viewModel;
        private bool isMouseLeaveWithDown = false;

        public Camera()
        {
            InitializeComponent();
            viewModel = new CameraViewModel(Dispatcher);
            DataContext = viewModel;
        }

        private RelativePoint GetRelativePoint(MouseEventArgs e)
        {
            var viewPoint = e.GetPosition(frameHolder);
            var imagePoint = new OpenCvSharp.Point(
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Session.InitializeHumanDetector();
        }
    }
}
