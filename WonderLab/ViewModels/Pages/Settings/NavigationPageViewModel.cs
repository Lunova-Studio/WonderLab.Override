using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using WonderLab.Interfaces.Navigation;
using WonderLab.ViewModels.Windows;
using ZLogger;

namespace WonderLab.ViewModels.Pages.Settings;

public sealed partial class NavigationPageViewModel : ViewModelBase {
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly INavigationService _navigationService;

    public NavigationPageViewModel(INavigationService navigationService, ILogger<MainWindowViewModel> logger) {
        _logger = logger;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private void Navigate(object index) {
        _logger.ZLogInformation($"Current page index: {index}");

        if (index is not string key)
            return;
        
        switch (key) {
            case "Launch":
                _ = _navigationService.NavigateToAsync<LaunchSettingsPageViewModel>();
                break;
            case "Java":
                _ = _navigationService.NavigateToAsync<JavaSettingsPageViewModel>();
                break;
            case "Account":
                _ = _navigationService.NavigateToAsync<AccountSettingsPageViewModel>();
                break;
            case "Network":
                _ = _navigationService.NavigateToAsync<NetworkSettingsPageViewModel>();
                break;
            case "Appearance":
                _ = _navigationService.NavigateToAsync<AppearanceSettingsPageViewModel>();
                break;
            case "About":
                _ = _navigationService.NavigateToAsync<AboutPageViewModel>();
                break;
        }
    }
}