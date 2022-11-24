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
    public static class Control
    {
        public static bool LogViolationScreenshot
        {
            get
            {
                string logViolationScreenshot = Datastore.GeneralGetValue("logViolationScreenshot");
                return logViolationScreenshot == "1";
            }
            set
            {
                Datastore.GeneralSetValue("logViolationScreenshot", value ? "1" : "0");
            }
        }

        public static bool AudioFeedback
        {
            get
            {
                string audioFeedback = Datastore.GeneralGetValue("audioFeedback");
                return audioFeedback == "1";
            }
            set
            {
                Datastore.GeneralSetValue("audioFeedback", value ? "1" : "0");
            }
        }
    }
}
