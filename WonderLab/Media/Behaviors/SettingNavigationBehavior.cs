using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using WonderLab.Controls;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Media.Transitions;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(string), "PageKey")]
[StyledProperty(typeof(Visual), "ToControl")]
[StyledProperty(typeof(Visual), "FromControl")]
[StyledProperty(typeof(AvaloniaPageProvider), "PageProvider")]
public sealed partial class SettingNavigationBehavior : Behavior {
    private static readonly EntrancePageTransition EntrancePageTransition = new();

    private object _newPageData;
    private object _oldPageData;
    private bool _isForward = true;

    protected override void OnLoaded() {
        base.OnLoaded();
        PropertyChanged += OnPropertyChanged;
    }

    private async void OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
        if (e.Property == PageKeyProperty) {
            if (!string.IsNullOrEmpty(e.GetNewValue<string>())) {
                _oldPageData = _newPageData;
                _newPageData = await Dispatcher.UIThread.InvokeAsync(() =>
                    PageProvider.GetPage(e.GetNewValue<string>()), DispatcherPriority.Background);

                Dispatcher.UIThread.Post(() => {
                    if (ToControl is ContentControl control)
                        control.Content = _newPageData;
                });
            }

            RunAnimation();
        }
    }

    private async void RunAnimation() {
        if (_isForward)
            await EntrancePageTransition.Start(FromControl, ToControl, true, default);
        else
            await EntrancePageTransition.Start(ToControl, FromControl, true, default);

        (_newPageData as Page)?.InvokeNavigated();
        (_oldPageData as Page)?.InvokeUnNavigated();

        _isForward = !_isForward;
    }
}