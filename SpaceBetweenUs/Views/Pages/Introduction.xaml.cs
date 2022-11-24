using FirstFloor.ModernUI.Windows.Controls;
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
    /// Interaction logic for Introduction.xaml
    /// </summary>
    public partial class Introduction : UserControl
    {
        public Introduction()
        {
            InitializeComponent();
        }

        private void OpenWindow(ModernDialog dialog, Action onOk)
        {
            Button cancelButton = dialog.CancelButton;
            cancelButton.Content = "Cancel";
            Button okButton = dialog.OkButton;
            okButton.Content = "Open";
            dialog.Buttons = new Button[] { cancelButton, okButton };
            dialog.MinWidth = 400;
            dialog.MinHeight = 0;
            dialog.MaxWidth = 400;
            dialog.SizeChanged += (s, e) =>
            {
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double windowWidth = e.NewSize.Width;
                double windowHeight = e.NewSize.Height;
                dialog.Left = (screenWidth / 2) - (windowWidth / 2);
                dialog.Top = (screenHeight / 2) - (windowHeight / 2);
            };
            _ = dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                onOk?.Invoke();
            }
        }

        private void Button_Click_IPCam(object sender, RoutedEventArgs e)
        {
            SourceIPCam editor = new SourceIPCam();
            ModernDialog dlg = new ModernDialog
            {
                Title = "IP Cam",
                Content = editor,
            };
            OpenWindow(dlg, editor.Open);
        }

        private void Button_Click_USBCam(object sender, RoutedEventArgs e)
        {
            SourceUSBCam editor = new SourceUSBCam();
            ModernDialog dlg = new ModernDialog
            {
                Title = "USB Cam",
                Content = editor,
            };
            OpenWindow(dlg, editor.Open);
        }

        private void Button_Click_VideoFile(object sender, RoutedEventArgs e)
        {
            SourceVideoFile editor = new SourceVideoFile();
            ModernDialog dlg = new ModernDialog
            {
                Title = "Video File",
                Content = editor,
            };
            OpenWindow(dlg, editor.Open);
        }
    }
}
