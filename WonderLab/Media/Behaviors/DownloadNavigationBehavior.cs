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
using WonderLab.Media.Transitions;

namespace WonderLab.Media.Behaviors;

public sealed class DownloadNavigationBehavior : Behavior {
    private Control _toTarget;
    private object _pageCache;
    private CancellationTokenSource _cancellationTokenSource = new();

    private static readonly EntrancePageTransition _pageTransition =
        new(TimeSpan.FromSeconds(0.35));

    public static readonly StyledProperty<Control> FromTargetProperty =
    AvaloniaProperty.Register<DownloadNavigationBehavior, Control>(nameof(FromTarget));

    public static readonly StyledProperty<string> PageKeyProperty =
        AvaloniaProperty.Register<SettingNavigationToBehavior, string>(nameof(PageKey));

    public static readonly StyledProperty<AvaloniaPageProvider> PageProviderProperty =
        AvaloniaProperty.Register<SettingNavigationToBehavior, AvaloniaPageProvider>(nameof(PageProvider));

    public Control FromTarget {
        get => GetValue(FromTargetProperty);
        set => SetValue(FromTargetProperty, value);
    }

    public AvaloniaPageProvider PageProvider {
        get => GetValue(PageProviderProperty);
        set => SetValue(PageProviderProperty, value);
    }

    public string PageKey {
        get => GetValue(PageKeyProperty);
        set => SetValue(PageKeyProperty, value);
    }

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

public sealed class SearchAnimationBehavior : Behavior {
    private TextBlock _toTarget;
    private CancellationTokenSource _cancellationTokenSource = new();

    public static readonly StyledProperty<string> KeywordProperty =
        AvaloniaProperty.Register<SearchAnimationBehavior, string>(nameof(Keyword));

    public static readonly StyledProperty<Control> FromTargetProperty =
        AvaloniaProperty.Register<SearchAnimationBehavior, Control>(nameof(FromTarget));

    public static readonly StyledProperty<Control> HideTargetProperty =
        AvaloniaProperty.Register<SearchAnimationBehavior, Control>(nameof(HideTarget));

    public static readonly StyledProperty<bool> IsHideProperty =
        AvaloniaProperty.Register<SearchAnimationBehavior, bool>(nameof(IsHide));

    public static readonly StyledProperty<bool> IsEnterKeyDownProperty =
        AvaloniaProperty.Register<SearchAnimationBehavior, bool>(nameof(IsEnterKeyDown));

    public string Keyword {
        get => GetValue(KeywordProperty);
        set => SetValue(KeywordProperty, value);
    }

    public Control FromTarget {
        get => GetValue(FromTargetProperty);
        set => SetValue(FromTargetProperty, value);
    }

    public Control HideTarget {
        get => GetValue(HideTargetProperty);
        set => SetValue(HideTargetProperty, value);
    }

    public bool IsHide {
        get => GetValue(IsHideProperty);
        set => SetValue(IsHideProperty, value);
    }

    public bool IsEnterKeyDown {
        get => GetValue(IsEnterKeyDownProperty);
        set => SetValue(IsEnterKeyDownProperty, value);
    }

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