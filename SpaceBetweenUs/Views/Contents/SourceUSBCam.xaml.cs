using FirstFloor.ModernUI.Windows.Controls;
using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels.Contents;
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
    /// Interaction logic for SourceUSBCam.xaml
    /// </summary>
    public partial class SourceUSBCam : UserControl
    {
        public SourceUSBCam()
        {
            InitializeComponent();
        }

        public void Open()
        {
            if (Session.HasSourceOpen(""))
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
            else if (false)
            {

            }
            else
            {

            }
        }
    }
}

