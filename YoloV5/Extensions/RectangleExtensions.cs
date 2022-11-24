using System.Drawing;

namespace YoloV5.Scorer.Extensions;

public static class RectangleExtensions
{
    public static float Area(this RectangleF source)
    {
        return source.Width * source.Height;
    }
}
