using FirstFloor.ModernUI.Presentation;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpaceBetweenUs
{
    public static class Appearance
    {
        public static Link DefaultTheme = new Link { DisplayName = "light", Source = AppearanceManager.LightThemeSource };
        public static LinkCollection Themes = new LinkCollection()
        {
            // add the default themes
            DefaultTheme,
            new Link { DisplayName = "dark", Source = AppearanceManager.DarkThemeSource },

            // add additional themes
            new Link { DisplayName = "hello kitty", Source = new Uri("/SpaceBetweenUs;component/Assets/ModernUI.HelloKitty.xaml", UriKind.Relative) },
            new Link { DisplayName = "love", Source = new Uri("/SpaceBetweenUs;component/Assets/ModernUI.Love.xaml", UriKind.Relative) },
            new Link { DisplayName = "snowflakes", Source = new Uri("/SpaceBetweenUs;component/Assets/ModernUI.Snowflakes.xaml", UriKind.Relative) }
        };

        public static Color DefaultAccentColor = Color.FromRgb(0x33, 0x99, 0xff);
        public static IEnumerable<Color> AccentColors = new Color[]
        {
            DefaultAccentColor,
            Color.FromRgb(0x00, 0xab, 0xa9),
            Color.FromRgb(0x33, 0x99, 0x33),
            Color.FromRgb(0x8c, 0xbf, 0x26),
            Color.FromRgb(0xf0, 0x96, 0x09),
            Color.FromRgb(0xff, 0x45, 0x00),
            Color.FromRgb(0xe5, 0x14, 0x00),
            Color.FromRgb(0xff, 0x00, 0x97),
            Color.FromRgb(0xa2, 0x00, 0xff),          
        };

        public static string DefaultFontSize = "large";
        public static IEnumerable<string> FontSizes
        {
            get { return new string[] { "small", DefaultFontSize }; }
        }

        public static Link SelectedTheme
        {
            get
            {
                string theme = Datastore.GeneralGetValue("theme");
                return Themes.FirstOrDefault(i => i.DisplayName.Equals(theme)) ?? DefaultTheme;
            }
            set
            {
                Datastore.GeneralSetValue("theme", value.DisplayName);
                AppearanceManager.Current.ThemeSource = value.Source;
            }
        }

        public static Color SelectedColor
        {
            get
            {
                string accent = Datastore.GeneralGetValue("accent");
                string r = CommonHelpers.BlobGetValue(accent, "r", DefaultAccentColor.R.ToString());
                string g = CommonHelpers.BlobGetValue(accent, "g", DefaultAccentColor.G.ToString());
                string b = CommonHelpers.BlobGetValue(accent, "b", DefaultAccentColor.B.ToString());
                return Color.FromRgb(byte.Parse(r), byte.Parse(g), byte.Parse(b));
            }
            set
            {
                string data = CommonHelpers.BlobSetValue("", "r", value.R.ToString());
                data = CommonHelpers.BlobSetValue(data, "g", value.G.ToString());
                data = CommonHelpers.BlobSetValue(data, "b", value.B.ToString());
                Datastore.GeneralSetValue("accent", data);
                AppearanceManager.Current.AccentColor = value;
            }
        }

        public static string SelectedFontSize
        {
            get
            {
                string font = Datastore.GeneralGetValue("font");
                return FontSizes.FirstOrDefault(i => i.Equals(font)) ?? DefaultFontSize;
            }
            set
            {
                Datastore.GeneralSetValue("font", value);
                AppearanceManager.Current.FontSize = value.Equals("small") ? FontSize.Small : FontSize.Large;
            }
        }

        public static void Initialize()
        {
            AppearanceManager.Current.ThemeSource = SelectedTheme.Source;
            AppearanceManager.Current.AccentColor = SelectedColor;
            AppearanceManager.Current.FontSize = SelectedFontSize.Equals("small") ? FontSize.Small : FontSize.Large;
        }
    }
}
