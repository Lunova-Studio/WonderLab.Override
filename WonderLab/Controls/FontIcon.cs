using Avalonia;
using Avalonia.Controls;

namespace WonderLab.Controls;

public sealed class FontIcon : ContentControl {
    public string Glyph {
        get => GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public static readonly StyledProperty<string> GlyphProperty =
        AvaloniaProperty.Register<FontIcon, string>(nameof(Glyph), "\uE76E");
}