using MvvmHelpers;
using SpaceBetweenUs.Model;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.ViewModels.Pages
{
    public class LogsViewModel : BaseViewModel
    {
        private IEnumerable<ViolationLog> violationLogs;
        public IEnumerable<ViolationLog> ViolationLogs
        {
            get => violationLogs;
            set => SetProperty(ref violationLogs, value);
        }

        public LogsViewModel()
        {
            ViolationLogs = Session.Logger.ViolationLogs;
        }
    }
}
