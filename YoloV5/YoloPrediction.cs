﻿using System.Drawing;

namespace YoloV5.Scorer;

/// <summary>
/// Object prediction.
/// </summary>
public class YoloPrediction
{
    public YoloLabel Label { get; set; }

    public RectangleF Rectangle { get; set; }

    public float Score { get; set; }

    public YoloPrediction(YoloLabel label, float confidence) : this(label)
    {
        Score = confidence;
    }

    public YoloPrediction(YoloLabel label)
    {
        Label = label;
    }
}
