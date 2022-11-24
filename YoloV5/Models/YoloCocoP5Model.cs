using System.Collections.Generic;
using YoloV5.Scorer.Models.Abstract;

namespace YoloV5.Scorer.Models;

public class YoloCocoP5Model : YoloModel
{
    public override int Width { get; set; } = 640;

    public override int Height { get; set; } = 640;

    public override int Depth { get; set; } = 3;

    public override int Dimensions { get; set; } = 85;

    public override int[] Strides { get; set; } = new int[] { 8, 16, 32 };

    public override int[][][] Anchors { get; set; } = new int[][][]
    {
        new int[][] { new int[] { 010, 13 }, new int[] { 016, 030 }, new int[] { 033, 023 } },
        new int[][] { new int[] { 030, 61 }, new int[] { 062, 045 }, new int[] { 059, 119 } },
        new int[][] { new int[] { 116, 90 }, new int[] { 156, 198 }, new int[] { 373, 326 } }
    };

    public override int[] Shapes { get; set; } = new int[] { 80, 40, 20 };

    public override float Confidence { get; set; } = 0.20f;

    public override float MulConfidence { get; set; } = 0.25f;

    public override float Overlap { get; set; } = 0.45f;

    public override string[] Outputs { get; set; } = new[] { "output" };

    public override List<YoloLabel> Labels { get; set; } = new List<YoloLabel>()
    {
        new YoloLabel(1, "person"),
        new YoloLabel(2, "bicycle"),
        new YoloLabel(3, "car"),
        new YoloLabel(4, "motorcycle"),
        new YoloLabel(5, "airplane"),
        new YoloLabel(6, "bus"),
        new YoloLabel(7, "train"),
        new YoloLabel(8, "truck"),
        new YoloLabel(9, "boat"),
        new YoloLabel(10, "traffic light"),
        new YoloLabel(11, "fire hydrant"),
        new YoloLabel(12, "stop sign"),
        new YoloLabel(13, "parking meter"),
        new YoloLabel(14, "bench"),
        new YoloLabel(15, "bird"),
        new YoloLabel(16, "cat"),
        new YoloLabel(17, "dog"),
        new YoloLabel(18, "horse"),
        new YoloLabel(19, "sheep"),
        new YoloLabel(20, "cow"),
        new YoloLabel(21, "elephant"),
        new YoloLabel(22, "bear"),
        new YoloLabel(23, "zebra"),
        new YoloLabel(24, "giraffe"),
        new YoloLabel(25, "backpack"),
        new YoloLabel(26, "umbrella"),
        new YoloLabel(27, "handbag"),
        new YoloLabel(28, "tie"),
        new YoloLabel(29, "suitcase"),
        new YoloLabel(30, "frisbee"),
        new YoloLabel(31, "skis"),
        new YoloLabel(32, "snowboard"),
        new YoloLabel(33, "sports ball"),
        new YoloLabel(34, "kite"),
        new YoloLabel(35, "baseball bat"),
        new YoloLabel(36, "baseball glove"),
        new YoloLabel(37, "skateboard"),
        new YoloLabel(38, "surfboard"),
        new YoloLabel(39, "tennis racket"),
        new YoloLabel(40, "bottle"),
        new YoloLabel(41, "wine glass"),
        new YoloLabel(42, "cup"),
        new YoloLabel(43, "fork"),
        new YoloLabel(44, "knife"),
        new YoloLabel(45, "spoon"),
        new YoloLabel(46, "bowl"),
        new YoloLabel(47, "banana"),
        new YoloLabel(48, "apple"),
        new YoloLabel(49, "sandwich"),
        new YoloLabel(50, "orange"),
        new YoloLabel(51, "broccoli"),
        new YoloLabel(52, "carrot"),
        new YoloLabel(53, "hot dog"),
        new YoloLabel(54, "pizza"),
        new YoloLabel(55, "donut"),
        new YoloLabel(56, "cake"),
        new YoloLabel(57, "chair"),
        new YoloLabel(58, "couch"),
        new YoloLabel(59, "potted plant"),
        new YoloLabel(60, "bed"),
        new YoloLabel(61, "dining table"),
        new YoloLabel(62, "toilet"),
        new YoloLabel(63, "tv"),
        new YoloLabel(64, "laptop"),
        new YoloLabel(65, "mouse"),
        new YoloLabel(66, "remote"),
        new YoloLabel(67, "keyboard"),
        new YoloLabel(68, "cell phone"),
        new YoloLabel(69, "microwave"),
        new YoloLabel(70, "oven"),
        new YoloLabel(71, "toaster"),
        new YoloLabel(72, "sink"),
        new YoloLabel(73, "refrigerator"),
        new YoloLabel(74, "book"),
        new YoloLabel(75, "clock"),
        new YoloLabel(76, "vase"),
        new YoloLabel(77, "scissors"),
        new YoloLabel(78, "teddy bear"),
        new YoloLabel(79, "hair drier"),
        new YoloLabel(80, "toothbrush")
    };

    public override bool UseDetect { get; set; } = true;
}
