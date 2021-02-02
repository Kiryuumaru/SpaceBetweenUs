using FirstFloor.ModernUI.Presentation;
using MvvmHelpers;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.ViewModels.Pages
{
    public class MachineLearningViewModel : BaseViewModel
    {
        #region ViewBindings

        private Uri selectedModelLink;
        public Uri SelectedModelLink
        {
            get => selectedModelLink;
            set => SetProperty(ref selectedModelLink, value);
        }

        private LinkCollection modelLinks;
        public LinkCollection ModelLinks
        {
            get => modelLinks;
            set => SetProperty(ref modelLinks, value);
        }

        #endregion

        #region ConvertedBindings

        public string SelectedModel
        {
            set
            {
                if (value == null) return;
                JumperCurrentModelSelect = value;
                SelectedModelLink = new Uri("/SpaceBetweenUs;component/Views/Contents/MachineLearningSelect.xaml?Name=" + value, UriKind.Relative);
            }
        }

        public IEnumerable<MLModel> Models
        {
            get
            {
                return MLModel.GetMLModels().Where(i => ModelLinks.Any(j => j.DisplayName.Equals(i.Name)));
            }
            set
            {
                if (value == null) return;
                ModelLinks = new LinkCollection(MLModel.GetMLModels().Select(i => new Link()
                {
                    DisplayName = i.Name,
                    Source = new Uri("/SpaceBetweenUs;component/Views/Contents/MachineLearningSelect.xaml?Name=" + i.Name, UriKind.Relative)
                }));
            }
        }

        #endregion

        #region JumperWires

        public static Action JumperModelTabChange;
        public static string JumperCurrentModelSelect;

        #endregion

        #region Initializers

        public MachineLearningViewModel()
        {
            var models = MLModel.GetMLModels();
            SelectedModel = Session.MLModel?.Name ?? models.First()?.Name;
            Models = models;
        }

        #endregion

        #region Methods



        #endregion
    }
}
