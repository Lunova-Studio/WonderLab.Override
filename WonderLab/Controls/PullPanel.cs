using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Extensions;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

[StyledProperty(typeof(bool), "IsPanelOpen")]
public sealed partial class PullPanel : ContentControl {
    private Border _PART_Layout;
    private CancellationTokenSource _cancellationToken = new();

    private bool IsBottomToTop => Height is not double.NaN;

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (IsPanelOpen) {
            this.RenderTransform = new TranslateTransform(0, 0);
            return;
        }

        if (IsBottomToTop)
            this.RenderTransform = new TranslateTransform(0, Height + 16);
        else
            this.RenderTransform = new TranslateTransform(Width + 16, 0);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_Layout = e.NameScope.Find<Border>("PART_Layout");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (!IsLoaded)
            return;

        if (change.Property == IsPanelOpenProperty)
            RunPullAnimation();
    }

    private TranslateTransform GetTransfrom() {
        return this.RenderTransform as TranslateTransform
            ?? new TranslateTransform(0, 0);
    }

    private void RunPullAnimation() {
        _cancellationToken.Cancel();
        _cancellationToken.Dispose();
        _cancellationToken = new();

        Dispatcher.UIThread.Post(async () => {
            if (IsBottomToTop)
                await BottomToTopAsync();
            else
                await RightToLeftAsync();
        });
    }

    private async Task BottomToTopAsync() {
        var y = GetTransfrom().Y;
        await this.Animate(TranslateTransform.YProperty)
            .WithEasing(IsPanelOpen ? new ExponentialEaseOut() : new ExponentialEaseIn())
            .WithDuration(TimeSpan.FromMilliseconds(250))
            .From(y)
            .To(IsPanelOpen ? 0 : Height + 16)
            .RunAsync(_cancellationToken.Token);
    }

    private async Task RightToLeftAsync() {
        var x = GetTransfrom().X;
        await this.Animate(TranslateTransform.XProperty)
            .WithDuration(TimeSpan.FromMilliseconds(350))
            .WithEasing(IsPanelOpen ? new ExponentialEaseOut() : new ExponentialEaseIn())
            .From(x)
            .To(IsPanelOpen ? 0 : Width + 16)
            .RunAsync(_cancellationToken.Token);
    }
}

public enum AnimationDirection {
    BottomToTop,
    RightToLeft
}