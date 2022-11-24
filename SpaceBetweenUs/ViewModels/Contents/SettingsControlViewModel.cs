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
    public class SettingsControlViewModel : BaseViewModel
    {
        public SettingsControlViewModel()
        {
            LogViolationScreenshot = Control.LogViolationScreenshot;
            AudioFeedback = Control.AudioFeedback;
        }

        private bool logViolationScreenshot;
        public bool LogViolationScreenshot
        {
            get { return logViolationScreenshot; }
            set
            {
                if (SetProperty(ref logViolationScreenshot, value))
                {
                    Control.LogViolationScreenshot = value;
                }
            }
        }

        private bool audioFeedback;
        public bool AudioFeedback
        {
            get { return audioFeedback; }
            set
            {
                if (SetProperty(ref audioFeedback, value))
                {
                    Control.AudioFeedback = value;
                }
            }
        }
    }
}
