using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

[StyledProperty(typeof(bool), "IsIndeterminate")]
[StyledProperty(typeof(bool), "PreserveAspect", true)]
[StyledProperty(typeof(double), "ValueAngle", 0)]
[StyledProperty(typeof(double), "StartAngle", 0)]
[StyledProperty(typeof(double), "EndAngle", 360)]
[PseudoClasses(":preserveaspect", ":indeterminate")]
public sealed partial class ProgressRing : RangeBase {
    static ProgressRing() {
        MinimumProperty.Changed.AddClassHandler<ProgressRing>(OnMinimumPropertyChanged);
        MaximumProperty.Changed.AddClassHandler<ProgressRing>(OnMaximumPropertyChanged);
        ValueProperty.Changed.AddClassHandler<ProgressRing>(OnValuePropertyChanged);
        MaximumProperty.Changed.AddClassHandler<ProgressRing>(OnStartAnglePropertyChanged);
        MaximumProperty.Changed.AddClassHandler<ProgressRing>(OnEndAnglePropertyChanged);
    }

    public ProgressRing() {
        UpdatePseudoClasses(IsIndeterminate, PreserveAspect);
    }

    private void UpdatePseudoClasses(bool? isIndeterminate, bool? preserveAspect) {
        if (isIndeterminate.HasValue) {
            PseudoClasses.Set(":indeterminate", isIndeterminate.Value);
        }

        if (preserveAspect.HasValue) {
            PseudoClasses.Set(":preserveaspect", preserveAspect.Value);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);
        var e = change as AvaloniaPropertyChangedEventArgs<bool>;
        if (e is null) return;

        if (e.Property == IsIndeterminateProperty) {
            UpdatePseudoClasses(e.NewValue.GetValueOrDefault(), null);
        } else if (e.Property == PreserveAspectProperty) {
            UpdatePseudoClasses(null, e.NewValue.GetValueOrDefault());
        }
    }

    private static void OnMinimumPropertyChanged(ProgressRing sender, AvaloniaPropertyChangedEventArgs e) {
        sender.Minimum = (double)e.NewValue;
    }

    private static void OnMaximumPropertyChanged(ProgressRing sender, AvaloniaPropertyChangedEventArgs e) {
        sender.Maximum = (double)e.NewValue;
    }

    private static void OnValuePropertyChanged(ProgressRing sender, AvaloniaPropertyChangedEventArgs e) {
        sender.ValueAngle = ((double)e.NewValue - sender.Minimum) * (sender.EndAngle - sender.StartAngle) / (sender.Maximum - sender.Minimum);
    }

    private static void OnStartAnglePropertyChanged(ProgressRing sender, AvaloniaPropertyChangedEventArgs e) {
        sender.StartAngle = (double)e.NewValue;
    }

    private static void OnEndAnglePropertyChanged(ProgressRing sender, AvaloniaPropertyChangedEventArgs e) {
        sender.EndAngle = (double)e.NewValue;
    }
}