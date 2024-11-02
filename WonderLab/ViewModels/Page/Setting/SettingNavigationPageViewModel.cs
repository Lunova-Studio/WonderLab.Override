using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using WonderLab.Extensions.Hosting.UI;

namespace WonderLab.ViewModels.Page.Setting;
public sealed partial class SettingNavigationPageViewModel : ObservableObject {
    public AvaloniaPageProvider PageProvider { get; }

    [ObservableProperty] private string _activePageKey;
    [ObservableProperty] private int _activePageIndex = -1;

    public SettingNavigationPageViewModel(AvaloniaPageProvider avaloniaPageProvider) {
        PageProvider = avaloniaPageProvider;
    }

    [RelayCommand]
    private void OnLoaded() {
        ActivePageIndex = 0;
    }

    [RelayCommand]
    private void ChangeActivePage() {
        ActivePageKey = ActivePageIndex switch {
            0 => "Setting/Launch",
            1 => "Setting/Account",
            2 => "Setting/Network",
            3 => "Setting/Appearance",
            4 => "Setting/About",
            _ => "Setting/Launch"
        };
    }
}