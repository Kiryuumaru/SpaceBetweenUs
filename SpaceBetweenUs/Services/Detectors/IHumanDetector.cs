using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public struct Human
    {
        public RelativePoint BL;
        public RelativePoint TL;
        public RelativePoint TR;
        public RelativePoint BR;
        public RelativePoint Center;

        public Human(double x, double y, double width, double height, double frameWidth, double frameHeight)
        {
            BL = RelativePoint.FromNorm(new Point(x, y + height), frameWidth, frameHeight);
            TL = RelativePoint.FromNorm(new Point(x, y), frameWidth, frameHeight);
            TR = RelativePoint.FromNorm(new Point(x + width, y), frameWidth, frameHeight);
            BR = RelativePoint.FromNorm(new Point(x + width, y + height), frameWidth, frameHeight);
            Center = RelativePoint.FromNorm(new Point(x + width / 2, y + height / 2), frameWidth, frameHeight);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Human human)) return false;
            return
                BL == human.BL &&
                TL == human.TL &&
                TR == human.TR &&
                BR == human.BR &&
                Center == human.Center;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Human left, Human right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Human left, Human right)
        {
            return !(left == right);
        }
    }

    public interface IHumanDetector
    {
        bool GPUMode { get; }
        IEnumerable<Human> DetectHuman(byte[] image);
    }
}
