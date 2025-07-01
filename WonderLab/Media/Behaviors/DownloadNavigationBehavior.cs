using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using System;
using WonderLab.Controls;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Media.Transitions;

namespace WonderLab.Media.Behaviors;

public sealed class DownloadNavigationBehavior : Behavior {
    private Control _toTarget;
    private object _pageCache;

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

    private async void RunAnimation(bool isForward) {
        if (isForward)
            await _pageTransition.Start(_toTarget, FromTarget, false, default);
        else
            await _pageTransition.Start(FromTarget, _toTarget, false, default);
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
