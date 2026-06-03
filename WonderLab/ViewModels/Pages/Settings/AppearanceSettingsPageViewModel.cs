using WonderLab.Interfaces.Navigation;

namespace WonderLab.ViewModels.Pages.Settings;

public sealed partial class AppearanceSettingsPageViewModel : ViewModelBase {
    public  AppearanceSettingsPageViewModel(INavigationService navigationService) : base(navigationService) {
    }
}