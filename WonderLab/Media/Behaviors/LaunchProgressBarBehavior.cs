using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using System;
using System.Threading.Tasks;
using WonderLab.Extensions;
using WonderLab.SourceGenerator.Attributes;
using WonderLab.ViewModels.Tasks;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(ProgressBar), "ProgressBar")]
[StyledProperty(typeof(LaunchTaskViewModel), "ActiveTask")]
public sealed partial class LaunchProgressBarBehavior : Behavior {
    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null)
            return;

        var control = AssociatedObject as Panel;
        control.Height = 0d;
        control.Opacity = 0d;
        control.RenderTransform = new ScaleTransform(0d, 1d);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        if (change.Property == ActiveTaskProperty) {
            var control = AssociatedObject as Control;

            if (ActiveTask is null) {
                ProgressBar.IsIndeterminate = false;

                RunClosedAnimation(control);
                return;
            }

            ProgressBar.IsIndeterminate = true;
            RunOpenedAnimation(control);
        }

        base.OnPropertyChanged(change);
    }

    private static double GetScaleX(Control transform) {
        return (transform.RenderTransform as ScaleTransform)?.ScaleX ?? -1d;
    }

    private static void RunOpenedAnimation(Control control) => Dispatcher.UIThread.Post(async () => {
        var task1 = control.Animate(Visual.OpacityProperty)
            .WithDuration(TimeSpan.FromMilliseconds(250))
            .From(control.Opacity)
            .To(1d)
            .RunAsync();

        var task2 = control.Animate(ScaleTransform.ScaleXProperty)
            .WithDuration(TimeSpan.FromMilliseconds(400))
            .From(GetScaleX(control))
            .To(1d)
            .RunAsync();

        var task3 = control.Animate(Layoutable.HeightProperty)
            .WithDuration(TimeSpan.FromMilliseconds(250))
            .From(control.Height)
            .To(20)
            .RunAsync();

        await Task.WhenAll(task1, task2, task3);
    });

    private static void RunClosedAnimation(Control control) => Dispatcher.UIThread.Post(async () => {
        var task1 = control.Animate(Visual.OpacityProperty)
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(control.Opacity)
            .To(0d)
            .RunAsync();

        var task2 = control.Animate(ScaleTransform.ScaleXProperty)
            .WithDuration(TimeSpan.FromMilliseconds(400))
            .From(GetScaleX(control))
            .To(0d)
            .RunAsync();

        var task3 = control.Animate(Layoutable.HeightProperty)
            .WithDuration(TimeSpan.FromMilliseconds(250))
            .From(control.Height)
            .To(0)
            .RunAsync();

        await Task.WhenAll(task1, task2, task3);
    });
}