using Avalonia.Controls.Chrome;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using WonderLab.Interfaces.Navigation;

namespace WonderLab.ViewModels.Pages.Settings;

public sealed partial class LaunchSettingsPageViewModel : ViewModelBase {
    private INavigationService _navigationService;

    public LaunchSettingsPageViewModel(INavigationService navigationService) {
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task Goback() {
        await _navigationService.GoBackAsync();
    }
}