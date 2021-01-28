using FirstFloor.ModernUI.Presentation;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpaceBetweenUs.ViewsModels.Contents
{
    /// <summary>
    /// A simple view model for configuring theme, font and accent colors.
    /// </summary>
    public class SettingsAppearanceViewModel : BaseViewModel
    {
        public IEnumerable<Link> Themes => Appearance.Themes;
        public IEnumerable<string> FontSizes => Appearance.FontSizes;
        public IEnumerable<Color> AccentColors => Appearance.AccentColors;

        public SettingsAppearanceViewModel()
        {
            SyncThemeAndColor();
            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;
        }

        private void SyncThemeAndColor()
        {
            SelectedFontSize = Appearance.SelectedFontSize;
            SelectedTheme = Appearance.SelectedTheme;
            SelectedAccentColor = Appearance.SelectedColor;
        }

        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ThemeSource" || e.PropertyName == "AccentColor" || e.PropertyName == "FontSizes") {
                SyncThemeAndColor();
            }
        }

        private Link selectedTheme;
        public Link SelectedTheme
        {
            get { return selectedTheme; }
            set
            {
                if (SetProperty(ref selectedTheme, value))
                {
                    Appearance.SelectedTheme = value;
                }
            }
        }

        private string selectedFontSize;
        public string SelectedFontSize
        {
            get { return selectedFontSize; }
            set
            {
                if (SetProperty(ref selectedFontSize, value))
                {
                    Appearance.SelectedFontSize = value;
                }
            }
        }

        private Color selectedAccentColor;
        public Color SelectedAccentColor
        {
            get { return selectedAccentColor; }
            set
            {
                if (SetProperty(ref selectedAccentColor, value))
                {
                    Appearance.SelectedColor = value;
                }
            }
        }
    }
}
