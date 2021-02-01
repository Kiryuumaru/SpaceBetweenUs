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

        public GPUSupportViewModel()
        {
            GPUEnabled = Session.UseGPU;
        }

        public void Save()
        {
            Session.UseGPU = GPUEnabled;
        }
    }
}
