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
        private Session session;

        private double threshold;
        public double Threshold
        {
            get => threshold;
            set => SetProperty(ref threshold, value);
        }

        private string unit;
        public string Unit
        {
            get => unit;
            set => SetProperty(ref unit, value);
        }

        public ViolationThresEditViewModel(Session session)
        {
            this.session = session;
            Threshold = session.HumanDetector?.ViolationThreshold ?? 0;
            Unit = session.GridProjection.Unit;
        }

        public void Save()
        {
            if (session.HumanDetector != null) session.HumanDetector.ViolationThreshold = Threshold;
            session.GridProjection.Unit = Unit;
        }
    }
}
