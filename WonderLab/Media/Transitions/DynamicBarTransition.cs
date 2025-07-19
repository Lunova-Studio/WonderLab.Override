using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Controls;
using WonderLab.Extensions;

namespace WonderLab.Media.Transitions;
internal class DynamicBarTransition : IPageTransition {
    public BarState OldState { get; set; }
    public BarState NewState { get; set; }

    public async Task Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken) {
        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        switch ((OldState, NewState)) {
            case (BarState.Collapsed, BarState.Expanded):
                await CollapsedToExpandedAsync(from, to, cancellationToken);
                break;
            case (BarState.Expanded, BarState.Collapsed):
                await ExpandedToCollapsedAsync(from, to, cancellationToken);
                break;
            case (BarState.Collapsed, BarState.Hidden):
                await CollapsedToHiddenAsync(from, cancellationToken);
                break;
            case (BarState.Hidden, BarState.Collapsed):
                await HiddenToCollapsedAsync(from, cancellationToken);
                break;
            case (BarState.Expanded, BarState.Hidden):
                await ExpandedToHiddenAsync(to);
                break;
            case (BarState.Hidden, BarState.Expanded):
                await HiddenToExpandedAsync(to);
                break;
        }
    }

    public static double? GetTransformXValue(Visual control) {
        if (control.RenderTransform is not TransformGroup transforms || transforms.Children.Count <= 0)
            return null;

        return (transforms.Children
            .FirstOrDefault(x => x is TranslateTransform) as TranslateTransform).X;
    }

    private static async Task ExpandedToCollapsedAsync(Visual bar, Visual content, CancellationToken cancellationToken) {
        await content.Animate(TranslateTransform.XProperty)
            .WithEasing(new ExponentialEaseIn())
            .WithDuration(TimeSpan.FromSeconds(0.3))
            .From(-16d)
            .To(460d)
            .RunAsync(CancellationToken.None);//这段代码不要使用参数里的 cancellationToken，会因为玄学问题出现 Bug.

        await bar.Animate(TranslateTransform.XProperty)
            .WithDuration(TimeSpan.FromSeconds(0.3))
            .From(25d)
            .To(-16d)
            .RunAsync(cancellationToken);
    }

    private static async Task CollapsedToExpandedAsync(Visual bar, Visual content, CancellationToken cancellationToken) {
        var value = GetTransformXValue(bar);

        await bar.Animate(TranslateTransform.XProperty)
            .WithEasing(new ExponentialEaseIn())
            .WithDuration(TimeSpan.FromSeconds(0.3))
            .From(value ?? -16d)
            .To(25d)
            .RunAsync(cancellationToken);

        var task = content.Animate(Visual.OpacityProperty)
            .WithDuration(TimeSpan.FromSeconds(0.4))
            .From(content.Opacity)
            .To(1)
            .RunAsync(cancellationToken);

        var task1 = content.Animate(TranslateTransform.XProperty)
            .WithDuration(TimeSpan.FromSeconds(0.3))
            .From(460d)
            .To(-16d)
            .RunAsync(cancellationToken);

        await Task.WhenAll(task, task1);
    }

    private static async Task CollapsedToHiddenAsync(Visual bar, CancellationToken cancellationToken) {
        var value = GetTransformXValue(bar);

        await bar.Animate(TranslateTransform.XProperty)
            .WithEasing(new ExponentialEaseIn())
            .WithDuration(TimeSpan.FromSeconds(0.35))
            .From(value ?? -16d)
            .To(25d)
            .RunAsync(cancellationToken);
    }

    private static async Task HiddenToCollapsedAsync(Visual bar, CancellationToken cancellationToken) {
        var value = GetTransformXValue(bar);

        await bar.Animate(TranslateTransform.XProperty)
             .WithDuration(TimeSpan.FromSeconds(0.4))
             .From(value ?? 25d)
             .To(-16d)
             .RunAsync(cancellationToken);
    }

    private static async Task ExpandedToHiddenAsync(Visual content) {
        var value = GetTransformXValue(content);

        await content.Animate(TranslateTransform.XProperty)
            .WithEasing(new ExponentialEaseIn())
            .WithDuration(TimeSpan.FromSeconds(0.35))
            .From(value ?? -16d)
            .To(460d)
            .RunAsync();
    }

    private static async Task HiddenToExpandedAsync(Visual content) {
        var value = GetTransformXValue(content);

        await content.Animate(TranslateTransform.XProperty)
            .WithDuration(TimeSpan.FromSeconds(0.4))
            .From(value ?? 460d)
            .To(-16d)
            .RunAsync();
    }
}