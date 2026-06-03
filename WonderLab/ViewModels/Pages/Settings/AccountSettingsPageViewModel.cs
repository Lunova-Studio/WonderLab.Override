using Avalonia.Data.Converters;
using WonderLab.Interfaces.Navigation;

namespace WonderLab.ViewModels.Pages.Settings;

public sealed partial class AccountSettingsPageViewModel : ViewModelBase {
    public AccountSettingsPageViewModel(INavigationService navigationService) : base(navigationService) {
        
    }
}