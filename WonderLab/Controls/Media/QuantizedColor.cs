using Avalonia.Media;
using System;

namespace WonderLab.Controls.Media;

public readonly struct QuantizedColor {
    public Color Color { get; }
    public bool IsDark { get; }
    public int Population { get; }

    public QuantizedColor(Color color, int population) {
        Color = color;
        Population = population;
        IsDark = CalculateYiqLuma(color) < 128;
    }

    private static int CalculateYiqLuma(Color color) {
        return Convert.ToInt32(Math.Round((float)(299 * color.R + 587 * color.G + 114 * color.B) / 1000f));
    }
}