using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Threading.Tasks;
using WonderLab.Interfaces.Navigation;
using WonderLab.ViewModels.Pages;

namespace WonderLab.Services.Navigation;

public sealed class NavigationService : INavigationService {
    private readonly AvaloniaPageProvider _provider;
    private NavigationPage _nav;

    public NavigationService(AvaloniaPageProvider provider) {
        _provider = provider;
    }

    public void Attach(NavigationPage nav) => _nav = nav;

    public async Task NavigateToAsync<TViewModel>() where TViewModel : class {
        if (_nav is null)
            throw new InvalidOperationException("NavigationPage not attached.");

        var key = typeof(TViewModel).FullName!;
        var content = _provider.GetPage(key);

        await _nav.PushAsync(BuildPage(content));
    }

    public async Task NavigateToPageAsync<TPage>() where TPage : ContentPage {
        if (_nav is null)
            throw new InvalidOperationException("NavigationPage not attached.");

        var key = typeof(TPage).FullName!;
        var content = _provider.GetPage(key);

        await _nav.PushAsync(BuildPage(content));
    }

    public Task GoBackAsync() => _nav!.PopAsync();

    //大抵是这么设计的，ContentPage 在 Xaml 定义获取后无法正常显示内容
    private static ContentPage BuildPage(object page) {
        return new ContentPage {
            Content = page
        };
    }
}