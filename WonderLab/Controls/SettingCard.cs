using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace WonderLab.Controls;

public class SettingCard : ContentControl {
    private Grid _PART_Layout;
    private ContentPresenter _PART_ContentPresenter;

    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<SettingCard, string>(nameof(Icon));

    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<SettingCard, string>(nameof(Header), "Title");

    public string Icon {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Header {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (_PART_ContentPresenter is not null && _PART_ContentPresenter.Bounds.Width > Bounds.Width / 2) {
            Grid.SetRow(_PART_ContentPresenter, 1);
            Grid.SetColumn(_PART_ContentPresenter, 1);
            _PART_ContentPresenter.Margin = new(12, 0, 0, 0);
            _PART_Layout.RowSpacing = 8;
        }
    }

    protected override Size MeasureOverride(Size availableSize) {
        if (_PART_ContentPresenter.Bounds.Width > availableSize.Width / 2) {
            Grid.SetRow(_PART_ContentPresenter, 1);
            Grid.SetColumn(_PART_ContentPresenter, 1);
            _PART_ContentPresenter.Margin = new(12, 0, 0, 0);
            _PART_Layout.RowSpacing = 8;
        } else {
            Grid.SetRow(_PART_ContentPresenter, 0);
            Grid.SetColumn(_PART_ContentPresenter, 3);
            _PART_ContentPresenter.Margin = new(0);
            _PART_Layout.RowSpacing = 0;
        }

        return base.MeasureOverride(availableSize);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_Layout = e.NameScope.Find<Grid>("PART_LayoutGrid");
        _PART_ContentPresenter = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
    }
}

public sealed class SettingExpandCard : ItemsControl {
    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<SettingExpandCard, string>(nameof(Icon));

    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<SettingExpandCard, bool>(nameof(IsExpanded));

    public static readonly StyledProperty<object> HeaderProperty =
        AvaloniaProperty.Register<SettingExpandCard, object>(nameof(Header), "Title");

    public static readonly StyledProperty<bool> CanExpandedProperty =
        AvaloniaProperty.Register<SettingExpandCard, bool>(nameof(CanExpanded));

    public static readonly StyledProperty<object> FooterProperty =
        AvaloniaProperty.Register<SettingExpandCard, object>(nameof(Footer));

    public string Icon {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public object Header {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object Footer {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }
    public bool IsExpanded {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public bool CanExpanded {
        get => GetValue(CanExpandedProperty);
        set => SetValue(CanExpandedProperty, value);
    }
}