using MvvmHelpers;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.ViewModels.Contents
{
    public class ViolationThresEditViewModel : BaseViewModel
    {
        private double threshold;
        public double Threshold
        {
            get => threshold;
            set => SetProperty(ref threshold, value);
        }

        public ViolationThresEditViewModel()
        {
            Threshold = Session.HumanDetector?.ViolationThreshold ?? 0;
        }

        public void Save()
        {
            if (Session.HumanDetector != null) Session.HumanDetector.ViolationThreshold = Threshold;
        }
    }
}
