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
            Distance = side switch
            {
                GridSide.Left => Session.GridProjection.LeftDistance,
                GridSide.Top => Session.GridProjection.TopDistance,
                GridSide.Right => Session.GridProjection.RightDistance,
                GridSide.Bottom => Session.GridProjection.BottomDistance,
                _ => throw new Exception("Nonexistence side error")
            };
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
