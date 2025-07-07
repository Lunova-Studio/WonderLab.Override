using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Controls;
using WonderLab.Extensions;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(string), "PageKey")]
[StyledProperty(typeof(Control), "FromTarget")]
[StyledProperty(typeof(AvaloniaPageProvider), "PageProvider")]
public sealed partial class DownloadNavigationBehavior : Behavior {
    private Control _toTarget;
    private object _pageCache;
    private CancellationTokenSource _cancellationTokenSource = new();

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null)
            return;

        _toTarget = AssociatedObject as Control;
        PropertyChanged += OnPropertyChanged;
    }

    protected override void OnDetaching() {
        base.OnDetaching();
        PropertyChanged -= OnPropertyChanged;
    }

    private void Cancel() {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        _cancellationTokenSource = new();
    }

    private async void RunAnimation(bool isForward) {
        Cancel();

        var target1 = isForward ? _toTarget : FromTarget;
        var target2 = isForward ? FromTarget : _toTarget;

        target1.ZIndex = 0;
        target2.ZIndex = 1;

        var task11 = target1.Animate(Visual.OpacityProperty)
            .WithEasing(new ExponentialEaseOut())
            .WithDuration(TimeSpan.FromSeconds(0.35))
            .From(target1.Opacity)
            .To(0)
            .RunAsync(_cancellationTokenSource.Token);

        var task21 = target2.Animate(Visual.OpacityProperty)
            .WithEasing(new ExponentialEaseOut())
            .WithDuration(TimeSpan.FromSeconds(0.35))
            .From(target2.Opacity)
            .To(1)
            .RunAsync(_cancellationTokenSource.Token);

        var task22 = target2.Animate(TranslateTransform.YProperty)
            .WithEasing(new ExponentialEaseOut())
            .WithDuration(TimeSpan.FromSeconds(0.35))
            .From(150)
            .To(0)
            .RunAsync(_cancellationTokenSource.Token);

        await Dispatcher.UIThread.InvokeAsync(async () => {
            await Task.WhenAll(task11, task21, task22);
        });
    }

    private async void OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
        if (e.Property == PageKeyProperty) {
            var flag = string.IsNullOrEmpty(e.GetNewValue<string>());

            if (!flag) {
                (_pageCache as Page)?.InvokeUnNavigated();
                _pageCache = await Dispatcher.UIThread.InvokeAsync(() =>
                    PageProvider.GetPage(e.GetNewValue<string>()), DispatcherPriority.Background);

                Dispatcher.UIThread.Post(() => {
                    (_toTarget as ContentControl).Content = _pageCache;
                });
            }

            RunAnimation(flag);
        }
    }
}

[StyledProperty(typeof(string), "Keyword")]
[StyledProperty(typeof(bool), "IsHide")]
[StyledProperty(typeof(bool), "IsEnterKeyDown")]
[StyledProperty(typeof(Control), "FromTarget")]
[StyledProperty(typeof(Control), "HideTarget")]
public sealed partial class SearchAnimationBehavior : Behavior {
    private TextBlock _toTarget;
    private CancellationTokenSource _cancellationTokenSource = new();

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null and not TextBlock)
            return;

        _toTarget = AssociatedObject as TextBlock;

        PropertyChanged += OnPropertyChanged;
    }

    protected override void OnDetaching() {
        base.OnDetaching();
        PropertyChanged -= OnPropertyChanged;
    }

    private void Cancel() {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        _cancellationTokenSource = new();
    }

    private async void RunAnimation() {
        Cancel();

        bool flag = string.IsNullOrEmpty(Keyword);
        string keyword = flag ? string.Empty : $"\"{Keyword}\"";
        double toY = flag ? 50d : 0d;
        double toOpacity = flag ? 0d : 1d;

        Thickness fromTargetMargin = flag
            ? new(FromTarget.Margin.Left, 0, FromTarget.Margin.Right, FromTarget.Margin.Bottom)
            : new(FromTarget.Margin.Left, 52, FromTarget.Margin.Right, FromTarget.Margin.Bottom);

        await Dispatcher.UIThread.InvokeAsync(async () => {
            _toTarget.Text = keyword;
            if (flag)
                SetCurrentValue(IsHideProperty, false);

            var fromTask1 = FromTarget.Animate(Layoutable.MarginProperty)
                .WithEasing(new ExponentialEaseOut())
                .WithDuration(TimeSpan.FromSeconds(0.35))
                .From(FromTarget.Margin)
                .To(fromTargetMargin)
                .RunAsync(_cancellationTokenSource.Token);

            var toTask1 = _toTarget.Animate(TranslateTransform.YProperty)
                .WithDuration(TimeSpan.FromSeconds(0.35))
                .From(50d)
                .To(toY)
                .RunAsync(_cancellationTokenSource.Token);

            var toTask2 = _toTarget.Animate(Visual.OpacityProperty)
                .WithDuration(TimeSpan.FromSeconds(1))
                .From(0)
                .To(toOpacity)
                .RunAsync(_cancellationTokenSource.Token);

            await Task.WhenAll([fromTask1, toTask1, toTask2]);
        });
    }

    private async void RunHideTargetAnimation() {
        double hideWidth = IsHide ? 0d : 120d;

        await HideTarget.Animate(Layoutable.WidthProperty)
            .WithEasing(new ExponentialEaseOut())
            .WithDuration(TimeSpan.FromSeconds(0.35))
            .From(HideTarget.Width)
            .To(hideWidth)
            .RunAsync(_cancellationTokenSource.Token);
    }

    private void OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
        if (e.Property == IsEnterKeyDownProperty && e.GetNewValue<bool>()) {
            SetCurrentValue(IsEnterKeyDownProperty, false);
            RunAnimation();
        }

        if (e.Property == IsHideProperty) {
            RunHideTargetAnimation();
        }
    }
}