using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace WonderLab.Controls;

public sealed class SettingExpander : ItemsControl {
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<SettingExpander, string>(nameof(Header), "Hello Title");

    public static readonly StyledProperty<string> GlyphProperty =
        AvaloniaProperty.Register<SettingExpander, string>(nameof(Glyph));

    public string Header {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string Glyph {
        get => GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }
}

public sealed class SettingExpanderItem : TemplatedControl {
    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<SettingExpanderItem, object>(nameof(Content));

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<SettingExpanderItem, string>(nameof(Description), "Hello");

    public string Description {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    [Content]
    public object Content {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
}