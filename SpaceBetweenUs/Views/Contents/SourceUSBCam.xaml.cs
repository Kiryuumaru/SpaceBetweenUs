using FirstFloor.ModernUI.Windows.Controls;
using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels.Contents;
using SpaceBetweenUs.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;

namespace SpaceBetweenUs.Views.Contents
{
    /// <summary>
    /// Interaction logic for SourceUSBCam.xaml
    /// </summary>
    public partial class SourceUSBCam : UserControl
    {
        public SourceUSBCam()
        {
            InitializeComponent();
            List<CameraObject> camList = FrameSource.GetAllConnectedCameras();
            cameraList.ItemsSource = camList;
            if (camList.Count > 0)
            {
                string lastOpen = Datastore.GeneralGetValue("usb_cam_last");
                cameraList.SelectedItem = camList.FirstOrDefault(i => i.Name.Equals(lastOpen)) ?? camList[0];
            }
        }

        public async void Open()
        {
            CameraObject selected = (CameraObject)cameraList.SelectedItem;
            if (selected == null)
            {
                ModernDialog dlg = new ModernDialog
                {
                    Title = "None selected",
                    Content = "Please select a USB camera",
                };
                Button okButton = dlg.OkButton;
                okButton.Content = "Ok";
                dlg.Buttons = new Button[] { okButton };
                dlg.MinWidth = 400;
                dlg.MinHeight = 0;
                dlg.SizeChanged += (s, e) =>
                {
                    double screenWidth = SystemParameters.PrimaryScreenWidth;
                    double screenHeight = SystemParameters.PrimaryScreenHeight;
                    double windowWidth = e.NewSize.Width;
                    double windowHeight = e.NewSize.Height;
                    dlg.Left = (screenWidth / 2) - (windowWidth / 2);
                    dlg.Top = (screenHeight / 2) - (windowHeight / 2);
                };
                _ = dlg.ShowDialog();
            }
            else if(Session.HasSourceOpen(selected.Name))
            {
                ModernDialog dlg = new ModernDialog
                {
                    Title = "USB Cam already open",
                    Content = "USB Cam is already open. Please choose new USB Cam to open.",
                };
                Button okButton = dlg.OkButton;
                okButton.Content = "Ok";
                dlg.Buttons = new Button[] { okButton };
                dlg.MinWidth = 400;
                dlg.MinHeight = 0;
                dlg.SizeChanged += (s, e) =>
                {
                    double screenWidth = SystemParameters.PrimaryScreenWidth;
                    double screenHeight = SystemParameters.PrimaryScreenHeight;
                    double windowWidth = e.NewSize.Width;
                    double windowHeight = e.NewSize.Height;
                    dlg.Left = (screenWidth / 2) - (windowWidth / 2);
                    dlg.Top = (screenHeight / 2) - (windowHeight / 2);
                };
                _ = dlg.ShowDialog();
            }
            else if ((OutputResult.IsChecked ?? false) && !Directory.Exists(Path.GetFullPath(OutputFilename.Text)))
            {
                var dlg = new ModernDialog
                {
                    Title = "File output is empty",
                    Content = "Please input the file output.",
                };
                var okButton = dlg.OkButton;
                okButton.Content = "Ok";
                dlg.Buttons = new Button[] { okButton };
                dlg.MinWidth = 400;
                dlg.MinHeight = 0;
                dlg.SizeChanged += (s, e) =>
                {
                    double screenWidth = SystemParameters.PrimaryScreenWidth;
                    double screenHeight = SystemParameters.PrimaryScreenHeight;
                    double windowWidth = e.NewSize.Width;
                    double windowHeight = e.NewSize.Height;
                    dlg.Left = (screenWidth / 2) - (windowWidth / 2);
                    dlg.Top = (screenHeight / 2) - (windowHeight / 2);
                };
                dlg.ShowDialog();
            }
            else
            {
                Datastore.GeneralSetValue("usb_cam_last", selected.Name);
                var session = await Session.Start(selected, selected.Name, (OutputResult.IsChecked ?? false) ? OutputFilename.Text : null);
                var window = new InstanceWindow(session);
                window.Show();
            }
        }

        private void OutputResult_Checked(object sender, RoutedEventArgs e)
        {
            OutputFilename.IsEnabled = true;
            OutputBrowse.IsEnabled = true;
        }

        private void OutputResult_Unchecked(object sender, RoutedEventArgs e)
        {
            OutputFilename.IsEnabled = false;
            OutputBrowse.IsEnabled = false;
        }

        private void Button_Click_Output(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "MP4 Video Files|*.mp4;*.MP4;"
            };
            if (saveFileDialog.ShowDialog() == true)
                OutputFilename.Text = saveFileDialog.FileName;
        }
    }
}

