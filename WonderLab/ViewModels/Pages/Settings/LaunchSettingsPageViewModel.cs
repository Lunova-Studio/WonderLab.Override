using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using WonderLab.Interfaces.Navigation;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Settings;

public sealed partial class LaunchSettingsPageViewModel : ViewModelBase {
    public SettingsService Settings { get; }
    
    public LaunchSettingsPageViewModel(INavigationService navigationService, SettingsService settingsService) : base(navigationService) {
        Settings = settingsService;
    }
}