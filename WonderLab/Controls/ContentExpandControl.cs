using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;

namespace WonderLab.Controls;

public sealed class ContentExpandControl : ContentControl {
    public static readonly StyledProperty<double> MultiplierProperty =
        AvaloniaProperty.Register<ContentExpandControl, double>(nameof(Multiplier));

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<ContentExpandControl, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public double Multiplier {
        get => GetValue(MultiplierProperty);
        set => SetValue(MultiplierProperty, value);
    }

    public Orientation Orientation {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    static ContentExpandControl() {
        AffectsArrange<ContentExpandControl>(MultiplierProperty, OrientationProperty);
        AffectsMeasure<ContentExpandControl>(MultiplierProperty, OrientationProperty);
    }

    protected override Size MeasureCore(Size availableSize) {
        return base.MeasureCore(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize) {
        var result = base.ArrangeOverride(finalSize);

        if (Parent is Control c) {
            c.Margin = new Thickness(1);
        }

        return result;
    }

    protected override Size MeasureOverride(Size availableSize) {
        var result = base.MeasureOverride(availableSize);

        var w = result.Width;
        var h = result.Height;

        switch (Orientation) {
            case Orientation.Horizontal:
                w *= Multiplier;
                break;

            case Orientation.Vertical:
                h *= Multiplier;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (Parent is Control c) {
            c.Margin = new Thickness(0);
        }

        return new(w, h);
    }
}