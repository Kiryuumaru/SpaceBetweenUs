using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Model
{
    public class HumanDistance
    {
        public Human Human1;
        public Human Human2;
        public RelativeLine Line;
        public double Distance;
        public bool IsViolation;

        public HumanDistance(Human human1, Human human2, double distance, bool isViolation)
        {
            Human1 = human1;
            Human2 = human2;
            Line = new RelativeLine(human1.BottomCenter, human2.BottomCenter);
            Distance = distance;
            IsViolation = isViolation;
        }
    }

}
