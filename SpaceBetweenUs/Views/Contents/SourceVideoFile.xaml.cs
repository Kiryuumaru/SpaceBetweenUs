using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Win32;
using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels.Contents;
using SpaceBetweenUs.Views.Windows;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace SpaceBetweenUs.Views.Contents
{
    /// <summary>
    /// Interaction logic for SourceVideoFile.xaml
    /// </summary>
    public partial class SourceVideoFile : UserControl
    {
        public SourceVideoFile()
        {
            InitializeComponent();
            Filename.Text = Datastore.GeneralGetValue("video_file_last");
        }

        private void Button_Click_Browse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Video Files|*.mp4;*.MP4;*.avi;*.AVI;*.mov;*.MOV"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Filename.Text = openFileDialog.FileName;
            }
        }

        public async void Open()
        {
            if (Session.HasSourceOpen(Filename.Text))
            {
                ModernDialog dlg = new ModernDialog
                {
                    Title = "File already open",
                    Content = "File provided is already open. Please choose new file to open.",
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
            else if (!File.Exists(Filename.Text))
            {
                ModernDialog dlg = new ModernDialog
                {
                    Title = "File error",
                    Content = "File does not exist. Please select valid and existing file.",
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
            else if ((OutputResult.IsChecked ?? false) && !Directory.GetParent(OutputFilename.Text).Exists)
            {
                var dlg = new ModernDialog
                {
                    Title = "File output is invalid",
                    Content = "Please input the valid file output.",
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
                Datastore.GeneralSetValue("video_file_last", Filename.Text);
                var session = await Session.Start(Filename.Text, Path.GetFileName(Filename.Text), (OutputResult.IsChecked ?? false) ? OutputFilename.Text : null);
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

