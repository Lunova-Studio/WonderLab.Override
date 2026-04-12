using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using WonderLab.Interfaces.Navigation;
using WonderLab.ViewModels.Pages;
using WonderLab.ViewModels.Pages.Settings;
using ZLogger;

namespace WonderLab.ViewModels.Windows;

public partial class MainWindowViewModel : ViewModelBase {
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly INavigationService _navigationService;

    [ObservableProperty] public partial int CurrentPageIndex { get; set; }

    public MainWindowViewModel(INavigationService navigationService, ILogger<MainWindowViewModel> logger) {
        _logger = logger;
        _navigationService = navigationService;
    }

    private void Navigate(int index) {
        _logger.ZLogInformation($"Current page index: {index}");

        switch (index) {
            case 0:
                _navigationService.NavigateToAsync<HomePageViewModel>();
                break;
            case 1:
                _navigationService.NavigateToAsync<MinecraftPageViewModel>();
                break;
            case 4:
                _navigationService.NavigateToAsync<NavigationPageViewModel>();
                break;
            default:
                break;
        }
    }

    partial void OnCurrentPageIndexChanged(int value) {
        Navigate(value);
    }
}
