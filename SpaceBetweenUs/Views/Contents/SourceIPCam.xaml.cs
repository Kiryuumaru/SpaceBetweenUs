using FirstFloor.ModernUI.Windows.Controls;
using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels.Contents;
using SpaceBetweenUs.Views.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

namespace SpaceBetweenUs.Views.Contents
{
    /// <summary>
    /// Interaction logic for SourceIPCam.xaml
    /// </summary>
    public partial class SourceIPCam : UserControl
    {
        public SourceIPCam()
        {
            InitializeComponent();
            IPCamLink.Text = Datastore.GeneralGetValue("ip_cam_last");
        }

        public static bool PingHost(string nameOrAddress)
        {
            try
            {
                _ = new Uri(nameOrAddress);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async void Open()
        {
            if (Session.HasSourceOpen(IPCamLink.Text))
            {
                var dlg = new ModernDialog
                {
                    Title = "IP link already open",
                    Content = "IP link provided is already open. Please provide new ip link.",
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
            else if (!PingHost(IPCamLink.Text))
            {
                var dlg = new ModernDialog
                {
                    Title = "IP link error",
                    Content = "IP link provided does not exist. Please select valid ip cam link.",
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
                Datastore.GeneralSetValue("ip_cam_last", IPCamLink.Text);
                var session = await Session.Start(IPCamLink.Text, Path.GetFileName(IPCamLink.Text.Replace("//", ".")));
                var window = new InstanceWindow(session);
                window.Show();
            }
        }
    }
}

