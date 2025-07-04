using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Rendering.Composition;
using WonderLab.Utilities;

namespace WonderLab.Controls;

public sealed class NavigationTile : Tile {
    public static readonly StyledProperty<object> IconProperty =
        AvaloniaProperty.Register<NavigationTile, object>(nameof(Icon));

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<NavigationTile, string>(nameof(Title));

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<NavigationTile, string>(nameof(Description));

    public static readonly StyledProperty<bool> IsDescriptionVisibleProperty =
        AvaloniaProperty.Register<NavigationTile, bool>(nameof(IsDescriptionVisible), true);

    public static readonly StyledProperty<object> FooterProperty =
        AvaloniaProperty.Register<NavigationTile, object>(nameof(Footer));

    public object Icon {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public object Footer {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public bool IsDescriptionVisible {
        get => GetValue(IsDescriptionVisibleProperty);
        set => SetValue(IsDescriptionVisibleProperty, value);
    }
}