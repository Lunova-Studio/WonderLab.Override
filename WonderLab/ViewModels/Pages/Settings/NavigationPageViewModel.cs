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

        if (index is string key)
            switch (key) {
                case "Launch":
                    _ = _navigationService.NavigateToAsync<LaunchSettingsPageViewModel>();
                    break;
                default:
                    break;
            }
    }
}