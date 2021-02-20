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
    /// Interaction logic for SourceUSBCam.xaml
    /// </summary>
    public partial class SourceUSBCam : UserControl
    {
        public SourceUSBCam()
        {
            InitializeComponent();
            var camList = FrameSource.GetAllConnectedCameras();
            cameraList.ItemsSource = camList;
            if (camList.Count > 0)
            {
                var lastOpen = Datastore.GeneralGetValue("usb_cam_last");
                cameraList.SelectedItem = camList.FirstOrDefault(i => i.Name.Equals(lastOpen)) ?? camList[0];
            }
        }

        public async void Open()
        {
            var selected = (CameraObject)cameraList.SelectedItem;
            if (selected == null)
            {
                var dlg = new ModernDialog
                {
                    Title = "None selected",
                    Content = "Please select a USB camera",
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
            else if(Session.HasSourceOpen(selected.Name))
            {
                var dlg = new ModernDialog
                {
                    Title = "USB Cam already open",
                    Content = "USB Cam is already open. Please choose new USB Cam to open.",
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
                var session = await Session.Start(selected, selected.Name);
                var window = new InstanceWindow(session);
                window.Show();
            }
        }
    }
}

