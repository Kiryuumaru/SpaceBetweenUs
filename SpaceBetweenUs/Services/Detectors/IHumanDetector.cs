using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class Human
    {
        public RelativePoint BL;
        public RelativePoint TL;
        public RelativePoint TR;
        public RelativePoint BR;
        public RelativePoint Center;
        public RelativePoint BottomCenter;
        public bool IsViolation;

        public Human(double x, double y, double width, double height, double frameWidth, double frameHeight, bool isViolation)
        {
            BL = RelativePoint.FromFrame(new Point(x, y + height), frameWidth, frameHeight);
            TL = RelativePoint.FromFrame(new Point(x, y), frameWidth, frameHeight);
            TR = RelativePoint.FromFrame(new Point(x + width, y), frameWidth, frameHeight);
            BR = RelativePoint.FromFrame(new Point(x + width, y + height), frameWidth, frameHeight);
            Center = RelativePoint.FromFrame(new Point(x + width / 2, y + height / 2), frameWidth, frameHeight);
            BottomCenter = RelativePoint.FromFrame(new Point(x + width / 2, y + height), frameWidth, frameHeight);
            IsViolation = isViolation;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Human human)) return false;
            return
                BL == human.BL &&
                TL == human.TL &&
                TR == human.TR &&
                BR == human.BR &&
                Center == human.Center &&
                BottomCenter == human.BottomCenter;
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
