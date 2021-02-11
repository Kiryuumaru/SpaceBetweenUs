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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Appearance.Initialize();
            HumanDetector.InitializeDetector();
        }
    }
}
