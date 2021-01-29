using FirstFloor.ModernUI.Presentation;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SpaceBetweenUs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            // Setup Quick Converter.
            QuickConverter.EquationTokenizer.AddNamespace(typeof(object));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(Visibility));
            QuickConverter.EquationTokenizer.AddExtensionMethods(typeof(Enumerable));
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await Session.Start(@"..\..\..\..\Additionals\SampleVideos\test1.mp4");
            Appearance.Initialize();
        }
    }
}
