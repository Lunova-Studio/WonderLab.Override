using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WonderLab.Classes.Enums;
using WonderLab.Extensions;
using WonderLab.Media.Easings;

namespace WonderLab.Controls;

[StyledProperty(typeof(ICommand), "FABCommand")]
public sealed partial class NavigationRail : ItemsControl {
    private bool _isHidden = false;
    private Border _PART_RootBorder;

    private CancellationTokenSource _cancellationTokenSource = new();

    public Frame Frame { get; private set; }

    private async Task HiddenRailAsync(CancellationToken cancellationToken) {
        _isHidden = true;

        var task1 = _PART_RootBorder.Animate(MarginProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(_PART_RootBorder.Margin)
            .To(new(-98, 0, 0, 0))
            .RunAsync(cancellationToken);

        var task2 = Frame.Animate(MarginProperty)
            .WithEasing(new WonderBackEaseOut() { Amplitude = Amplitude.Strong })
            .WithDuration(TimeSpan.FromMilliseconds(250))
            .From(Frame.Margin)
            .To(new(0, 0, 0, 80))
            .RunAsync(cancellationToken);

        await Task.WhenAll(task1, task2);
    }

    private async Task ExpandRailAsync(CancellationToken cancellationToken) {
        _isHidden = false;

        var task1 = _PART_RootBorder.Animate(MarginProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(_PART_RootBorder.Margin)
            .To(new(0))
            .RunAsync(cancellationToken);

        var task2 = Frame.Animate(MarginProperty)
            .WithEasing(new WonderBackEaseOut() { Amplitude = Amplitude.Strong })
            .WithDuration(TimeSpan.FromMilliseconds(250))
            .From(Frame.Margin)
            .To(new(0))
            .RunAsync(cancellationToken);

        await Task.WhenAll(task1, task2);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        Frame = e.NameScope.Find<Frame>("PART_Frame");
        _PART_RootBorder = e.NameScope.Find<Border>("PART_RootBorder");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (!IsLoaded)
            return;

        if (change.Property == BoundsProperty) {
            var width = change.GetNewValue<Rect>().Width;

            _ = Dispatcher.UIThread.InvokeAsync(async () => {
                if (width > 839 && _isHidden) {
                    Refresh();
                    await ExpandRailAsync(_cancellationTokenSource.Token);
                } else if (width < 839 && !_isHidden) {
                    Refresh();
                    await HiddenRailAsync(_cancellationTokenSource.Token);
                }
            });
        }

        void Refresh() {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new();
        }
    }
}