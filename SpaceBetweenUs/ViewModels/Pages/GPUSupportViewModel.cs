using MvvmHelpers;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.ViewModels.Pages
{
    public class GPUSupportViewModel : BaseViewModel
    {
        private bool gpuEnabled;
        public bool GPUEnabled
        {
            get => gpuEnabled;
            set
            {
                SetProperty(ref gpuEnabled, value);
                Save();
            }
        }

        private bool gpuClickable;
        public bool GPUClickable
        {
            get => gpuClickable;
            set => SetProperty(ref gpuClickable, value);
        }

        private bool progressActive;
        public bool ProgressActive
        {
            get => progressActive;
            set => SetProperty(ref progressActive, value);
        }

        public GPUSupportViewModel()
        {
            GPUEnabled = Session.UseGPU;
            GPUClickable = !Session.IsHumanDetectorInitializing;
            ProgressActive = Session.IsHumanDetectorInitializing;
            Session.OnHumanDetectorChanges += delegate
            {
                GPUClickable = !Session.IsHumanDetectorInitializing;
                ProgressActive = Session.IsHumanDetectorInitializing;
            };
        }

        public void Save()
        {
            if (Session.UseGPU != GPUEnabled)
            {
                Session.UseGPU = GPUEnabled;
                Session.InitializeHumanDetector();
            }
        }
    }
}
