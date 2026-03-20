using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Extensions;
using WonderLab.Media.Easings;

namespace WonderLab.Controls;

[StyledProperty(typeof(object), "ExpandContent")]
[StyledProperty(typeof(TaskBarState), "TaskBarState", TaskBarState.Expand)]
public sealed partial class TaskBar : TemplatedControl {
    private bool _isHidden = false;

    private Button _PART_FAButton;
    private Border _PART_ExpandRoot;
    private CancellationTokenSource _cancellationTokenSource = new();

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (Bounds.Width > 839) {

            _isHidden = true;
            
            _PART_FAButton.Width = 8;
            _PART_FAButton.Height = 8;
            _PART_FAButton.Opacity = 0;

            _PART_ExpandRoot.Margin = new(0, -85);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_FAButton = e.NameScope.Find<Button>("PART_FAButton");
        _PART_ExpandRoot = e.NameScope.Find<Border>("PART_ExpandRoot");
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (!IsLoaded)
            return;

        if (change.Property == BoundsProperty) {
            var width = change.GetNewValue<Rect>().Width;

            _ = Dispatcher.UIThread.InvokeAsync(async () => {
                if (width < 839 && _isHidden) {
                    Refresh();
                    await ExpandBarAsync(_cancellationTokenSource.Token);
                } else if (width >= 839 && !_isHidden) {
                    Refresh();
                    await HiddenBarAsync(_cancellationTokenSource.Token);
                }
            });
        }

        void Refresh() {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new();
        }
    }

    #region Animation Methods

    private async Task HiddenBarAsync(CancellationToken cancellationToken) {
        var task1 = _PART_ExpandRoot.Animate(MarginProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(_PART_ExpandRoot.Margin)
            .To(new(0, -85))
            .RunAsync(cancellationToken);


        var task2 = _PART_FAButton.Animate(HeightProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(56d)
            .To(8d)
            .RunAsync(cancellationToken);

        var task3 = _PART_FAButton.Animate(WidthProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(56d)
            .To(8d)
            .RunAsync(cancellationToken);

        var task4 = _PART_FAButton.Animate(OpacityProperty)
            .WithEasing(new ExponentialEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(150))
            .From(_PART_FAButton.Opacity)
            .To(0d)
            .RunAsync(cancellationToken);

        _isHidden = true;
        await Task.WhenAll(task1, task2, task3, task4);
    }

    private async Task ExpandBarAsync(CancellationToken cancellationToken) {
        var task1 = _PART_ExpandRoot.Animate(MarginProperty)
            .WithEasing(new WonderBackEaseOut() { Amplitude = Amplitude.Strong })
            .WithDuration(TimeSpan.FromMilliseconds(250))
            .From(_PART_ExpandRoot.Margin)
            .To(new(0, 0, 0, -5))
            .RunAsync(cancellationToken);


        var task2 = _PART_FAButton.Animate(HeightProperty)
            .WithEasing(new WonderBackEaseOut() { Amplitude = Amplitude.Strong })
            .WithDuration(TimeSpan.FromMilliseconds(250))
            .From(8d)
            .To(56d)
            .RunAsync(cancellationToken);

        var task3 = _PART_FAButton.Animate(WidthProperty)
            .WithEasing(new WonderBackEaseOut() { Amplitude = Amplitude.Strong })
            .WithDuration(TimeSpan.FromMilliseconds(250))
            .From(8d)
            .To(56d)
            .RunAsync(cancellationToken);

        var task4 = _PART_FAButton.Animate(OpacityProperty)
            .WithEasing(new ExponentialEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(150))
            .From(_PART_FAButton.Opacity)
            .To(1d)
            .RunAsync(cancellationToken);

        _isHidden = false;
        await Task.WhenAll(task1, task2, task3, task4);
    }

    #endregion
}

public enum TaskBarState {
    Expand,
    Hidden,
    Mini
}