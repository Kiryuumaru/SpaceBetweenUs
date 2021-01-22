using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SpaceBetweenUsGUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            NavigateToView("DashboardView");
        }

        private NavigationViewItem lastItem;

        private bool NavigateToView(string clickedView)
        {
            var view = Assembly.GetExecutingAssembly()
                .GetType($"SpaceBetweenUsGUI.Views.{clickedView}");

            if (string.IsNullOrWhiteSpace(clickedView) || view == null)
            {
                return false;
            }

            ContentFrame.Navigate(view, null, new EntranceNavigationTransitionInfo());
            return true;
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (!(args.InvokedItemContainer is NavigationViewItem item) || item == lastItem) return;
            var clickedView = item.Tag?.ToString() ?? "SettingsView";
            if (!NavigateToView(clickedView)) return;
            lastItem = item;
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
                ContentFrame.GoBack();
        }
    }
}
