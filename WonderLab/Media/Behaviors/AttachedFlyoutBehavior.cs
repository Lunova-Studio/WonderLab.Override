using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System;

namespace WonderLab.Media.Behaviors;

public sealed class AttachedFlyoutParentControlBehavior : Behavior<Control> {
    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null)
            return;

        AssociatedObject.GotFocus += OnGotFocus;
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        if (AssociatedObject is null)
            return;

        AssociatedObject.GotFocus -= OnGotFocus;
    }

    private void OnGotFocus(object sender, GotFocusEventArgs e) =>
        FlyoutBase.ShowAttachedFlyout(AssociatedObject);
}

public sealed class AttachedFlyoutBehavior : Behavior<Flyout> {
    public static readonly StyledProperty<Control> TargetProperty =
        AvaloniaProperty.Register<AttachedFlyoutBehavior, Control>(nameof(Target));

    public Control Target {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null)
            return;

        AssociatedObject.Closed += OnClosed;
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        if (AssociatedObject is null)
            return;

        AssociatedObject.Closed -= OnClosed;
    }

    private void OnClosed(object sender, EventArgs e) {
        Target.Focus();
    }
}