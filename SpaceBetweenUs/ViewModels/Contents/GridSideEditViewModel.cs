using MvvmHelpers;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.ViewModels.Contents
{
    public class GridSideEditViewModel : BaseViewModel
    {
        private readonly GridSide side;

        private double distance;
        public double Distance
        {
            get => distance;
            set => SetProperty(ref distance, value);
        }

        public GridSideEditViewModel(GridSide side)
        {
            this.side = side;
            switch (side)
            {
                case GridSide.Left:
                    Header = "Left";
                    Distance = Session.GridProjection.LeftDistance;
                    break;
                case GridSide.Top:
                    Header = "Top";
                    Distance = Session.GridProjection.TopDistance;
                    break;
                case GridSide.Right:
                    Header = "Right";
                    Distance = Session.GridProjection.RightDistance;
                    break;
                case GridSide.Bottom:
                    Header = "Bottom";
                    Distance = Session.GridProjection.BottomDistance;
                    break;
            }
        }

        public void Save()
        {
            switch (side)
            {
                case GridSide.Left:
                     Session.GridProjection.LeftDistance = Distance;
                    break;
                case GridSide.Top:
                     Session.GridProjection.TopDistance = Distance;
                    break;
                case GridSide.Right:
                     Session.GridProjection.RightDistance = Distance;
                    break;
                case GridSide.Bottom:
                     Session.GridProjection.BottomDistance = Distance;
                    break;
            }
        }
    }
}
