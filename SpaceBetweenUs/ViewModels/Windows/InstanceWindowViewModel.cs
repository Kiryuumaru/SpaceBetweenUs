using MvvmHelpers;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.ViewModels.Windows
{
    public class InstanceWindowViewModel : BaseViewModel
    {
        private readonly Session session;

        public InstanceWindowViewModel(Session session)
        {
            this.session = session;
        }
    }
}
