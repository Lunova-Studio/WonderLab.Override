using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Animation;
using System;
using Avalonia.Animation.Easings;
using Avalonia.Interactivity;
using Avalonia.Controls;
using System.Diagnostics;

namespace WonderLab.Views.Controls;

/// <summary>
/// 游戏实体管理面板控件
/// </summary>
public sealed class GameManagerPanel : SelectingItemsControl {
    private readonly BlurEffect _blurEffect = new() {
        Radius = 0,
        Transitions = [
            new DoubleTransition {
                Property = BlurEffect.RadiusProperty,
                Duration = TimeSpan.FromSeconds(0.5d),
                Easing = new ExponentialEaseOut()
            }
        ]
    };

    private Border _PART_BottomBarLayout;
    private Border _PART_ClosePanelBorder;

    public bool IsOpenPanel {
        get => GetValue(IsOpenPanelProperty);
        set => SetValue(IsOpenPanelProperty, value);
    }

    public Control EffectControl {
        get => GetValue(EffectControlProperty);
        set => SetValue(EffectControlProperty, value);
    }

    public static readonly StyledProperty<bool> IsOpenPanelProperty =
        AvaloniaProperty.Register<GameManagerPanel, bool>(nameof(IsOpenPanel));

    public static readonly StyledProperty<Control> EffectControlProperty =
        AvaloniaProperty.Register<GameManagerPanel, Control>(nameof(EffectControl));

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (EffectControl is not null) {
            EffectControl.Effect = _blurEffect;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_BottomBarLayout = e.NameScope.Find<Border>("PART_BottomBarLayout");
        _PART_ClosePanelBorder = e.NameScope.Find<Border>("PART_ClosePanelBorder");

        _PART_ClosePanelBorder.PointerPressed += (sender, e) => {
            if (e.GetCurrentPoint(sender as Visual).Properties.IsLeftButtonPressed) {
                IsOpenPanel = !IsOpenPanel;
            }
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenPanelProperty) {
            _blurEffect.Radius = change.GetNewValue<bool>() ? 60 : 0;
            _PART_BottomBarLayout.Margin = change.GetNewValue<bool>() ? new(0) : new(0, 0, 0, -105);

            EffectControl.Margin = change.GetNewValue<bool>() ? new(-20) : new();
        }
    }
}