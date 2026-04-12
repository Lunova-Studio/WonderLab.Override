using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WonderLab.Interfaces.Navigation;
using ZLogger;

namespace WonderLab.Services.Navigation;

public sealed class NavigationService : INavigationService {
    private readonly AvaloniaPageProvider _provider;
    private readonly ILogger<NavigationService> _logger;

    private NavigationPage _nav;
    private string _lastPageKey;

    public NavigationService(AvaloniaPageProvider provider, ILogger<NavigationService> logger) {
        _logger = logger;
        _provider = provider;
    }

    public void Attach(NavigationPage nav) => _nav = nav;

    public async Task NavigateToAsync<TViewModel>() where TViewModel : class {
        if (_nav is null)
            throw new InvalidOperationException("NavigationPage not attached.");

        var key = typeof(TViewModel).FullName!;

        _logger.ZLogDebug($"old:{_lastPageKey} - new:{key}");
        if (key.Equals(_lastPageKey))
            return;

        _lastPageKey = key;
        var content = _provider.GetPage(key);

        await _nav.PushAsync(BuildPage(content));
    }

    public async Task NavigateToPageAsync<TPage>() where TPage : UserControl {
        if (_nav is null)
            throw new InvalidOperationException("NavigationPage not attached.");

        var key = typeof(TPage).FullName!;
        if (key.Equals(_lastPageKey))
            return;

        _lastPageKey = key;
        var content = _provider.GetPage(key);

        await _nav.PushAsync(BuildPage(content));
    }

    public Task GoBackAsync() => _nav!.PopAsync();

    //大抵是这么设计的，ContentPage 在 Xaml 定义获取后无法正常显示内容
    private static ContentPage BuildPage(object page) {
        return new ContentPage {
            Content = page,
        };
    }
}