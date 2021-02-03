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
                case GridSide.TopBottom:
                    Header = "Top - Bottom";
                    Distance = Session.GridProjection.TopBottomDistance;
                    break;
                case GridSide.LeftRight:
                    Header = "Left - Right";
                    Distance = Session.GridProjection.LeftRightDistance;
                    break;
            }
        }

        public void Save()
        {
            switch (side)
            {
                case GridSide.TopBottom:
                    Session.GridProjection.TopBottomDistance = Distance;
                    break;
                case GridSide.LeftRight:
                    Session.GridProjection.LeftRightDistance = Distance;
                    break;
            }
        }
    }
}
