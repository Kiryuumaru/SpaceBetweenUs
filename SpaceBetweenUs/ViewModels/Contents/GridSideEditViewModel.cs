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
        private Session session;

        private readonly GridSide side;

        private double distance;
        public double Distance
        {
            get => distance;
            set => SetProperty(ref distance, value);
        }

        public GridSideEditViewModel(Session session, GridSide side)
        {
            this.session = session;
            this.side = side;
            switch (side)
            {
                case GridSide.TopBottom:
                    Header = "Top and Bottom";
                    Distance = session.GridProjection.TopBottomDistance;
                    break;
                case GridSide.LeftRight:
                    Header = "Left and Right";
                    Distance = session.GridProjection.LeftRightDistance;
                    break;
            }
        }

        public void Save()
        {
            switch (side)
            {
                case GridSide.TopBottom:
                     session.GridProjection.TopBottomDistance = Distance;
                    break;
                case GridSide.LeftRight:
                     session.GridProjection.LeftRightDistance = Distance;
                    break;
            }
        }
    }
}
