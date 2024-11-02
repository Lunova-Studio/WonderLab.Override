using Avalonia.Media;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Collections.Generic;
using System.Linq;
using WonderLab.Controls.Media;

namespace WonderLab.Extensions;

public static class ColorExtension {
    public static readonly List<Avalonia.Media.Color> Colors = [
        Avalonia.Media.Color.FromRgb(255,185,0),
        Avalonia.Media.Color.FromRgb(255,140,0),
        Avalonia.Media.Color.FromRgb(247,99,12),
        Avalonia.Media.Color.FromRgb(202,80,16),
        Avalonia.Media.Color.FromRgb(218,59,1),
        Avalonia.Media.Color.FromRgb(239,105,80),
        Avalonia.Media.Color.FromRgb(209,52,56),
        Avalonia.Media.Color.FromRgb(255,67,67),
        Avalonia.Media.Color.FromRgb(231,72,86),
        Avalonia.Media.Color.FromRgb(232,17,35),
        Avalonia.Media.Color.FromRgb(234,0,94),
        Avalonia.Media.Color.FromRgb(195,0,82),
        Avalonia.Media.Color.FromRgb(227,0,140),
        Avalonia.Media.Color.FromRgb(191,0,119),
        Avalonia.Media.Color.FromRgb(194,57,179),
        Avalonia.Media.Color.FromRgb(154,0,137),
        Avalonia.Media.Color.FromRgb(0,120,212),
        Avalonia.Media.Color.FromRgb(0,99,177),
        Avalonia.Media.Color.FromRgb(142,140,216),
        Avalonia.Media.Color.FromRgb(107,105,214),
        Avalonia.Media.Color.FromRgb(135,100,184),
        Avalonia.Media.Color.FromRgb(116,77,169),
        Avalonia.Media.Color.FromRgb(177,70,194),
        Avalonia.Media.Color.FromRgb(136,23,152),
        Avalonia.Media.Color.FromRgb(0,153,188),
        Avalonia.Media.Color.FromRgb(45,125,154),
        Avalonia.Media.Color.FromRgb(0,183,195),
        Avalonia.Media.Color.FromRgb(3,131,135),
        Avalonia.Media.Color.FromRgb(0,178,148),
        Avalonia.Media.Color.FromRgb(1,133,116),
        Avalonia.Media.Color.FromRgb(0,204,106),
        Avalonia.Media.Color.FromRgb(16,137,62),
        Avalonia.Media.Color.FromRgb(122,117,116),
        Avalonia.Media.Color.FromRgb(93,90,88),
        Avalonia.Media.Color.FromRgb(104,118,138),
        Avalonia.Media.Color.FromRgb(81,92,107),
        Avalonia.Media.Color.FromRgb(86,124,115),
        Avalonia.Media.Color.FromRgb(72,104,96),
        Avalonia.Media.Color.FromRgb(73,130,5),
        Avalonia.Media.Color.FromRgb(16,124,16),
        Avalonia.Media.Color.FromRgb(118,118,118),
        Avalonia.Media.Color.FromRgb(76,74,72),
        Avalonia.Media.Color.FromRgb(105,121,126),
        Avalonia.Media.Color.FromRgb(74,84,89),
        Avalonia.Media.Color.FromRgb(100,124,100),
        Avalonia.Media.Color.FromRgb(82,94,84),
        Avalonia.Media.Color.FromRgb(132,117,69),
        Avalonia.Media.Color.FromRgb(126,115,95)
    ];

    public static SolidColorBrush ToBrush(this Avalonia.Media.Color color) {
        return new SolidColorBrush(color);
    }

    public static Avalonia.Media.Color ToColor(this string color) {
        if (string.IsNullOrEmpty(color)) {
            return Avalonia.Media.Color.Parse("#ffd99d00");
        }

        return Avalonia.Media.Color.Parse(color);
    }

    public static Avalonia.Media.Color GetColorAfterLuminance(this Avalonia.Media.Color color, float percent) {
        var hslColor = color.ToHsl();
        double luminance = Math.Clamp(hslColor.L + (hslColor.L * percent), 0, 1);

        return new HslColor(hslColor.A, hslColor.H, hslColor.S, luminance).ToRgb();
    }

    public static Image<Rgba32> ToRgba32Bitmap(this string path) {
        return Image.Load<Rgba32>(path);
    }

    public static IEnumerable<QuantizedColor> GetPaletteFromBitmap(this Image<Rgba32> bitmap, int colorCount = 8) {
        // 克隆和量化图像
        var image = bitmap.Clone(ctx => ctx.Resize(256, 0).Quantize(new OctreeQuantizer(new QuantizerOptions { MaxColors = colorCount + 3 })));
        var dictionary = new Dictionary<Rgba32, int>();

        // 统计每种颜色出现的频率
        for (int i = 0; i < image.Width; i++) {
            for (int j = 0; j < image.Height; j++) {
                var key = image[i, j];
                if (key.A != byte.MaxValue || key.R != byte.MaxValue || key.G != byte.MaxValue || key.B != byte.MaxValue) {
                    if (!dictionary.TryAdd(key, 1))
                        dictionary[key]++;
                }
            }
        }

        // 筛选并返回前 colorCount 种最常见的颜色
        return dictionary.OrderByDescending(c => c.Value)
            .Take(colorCount)
            .Select(c => new QuantizedColor(new Avalonia.Media.Color(c.Key.A, c.Key.R, c.Key.G, c.Key.B), c.Value))
            .ToList();
    }
}