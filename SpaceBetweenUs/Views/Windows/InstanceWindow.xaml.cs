using FirstFloor.ModernUI.Windows.Controls;
using SpaceBetweenUs.Services;
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
using System.Windows.Shapes;

namespace SpaceBetweenUs.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class InstanceWindow : ModernWindow
    {
        public readonly Session Session;

        public InstanceWindow(Session session)
        {
            InitializeComponent();
            Session = session;
        }
    }
}
