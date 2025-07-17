using Avalonia.Media;

namespace WonderLab.Classes.Models;

public sealed record ColorInfo {
    public string Key { get; set; }
    public Color Color { get; set; }
    public uint ColorData { get; set; }

    public IBrush Brush => new SolidColorBrush(Color);
}