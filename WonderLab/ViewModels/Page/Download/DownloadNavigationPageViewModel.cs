using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Extensions.Hosting.UI;

namespace WonderLab.ViewModels.Page.Download;

public sealed partial class DownloadNavigationPageViewModel : ObservableObject {
    public AvaloniaPageProvider PageProvider { get; }

    [ObservableProperty] private string _activePageKey;
    [ObservableProperty] private int _activePageIndex = -1;

    public DownloadNavigationPageViewModel(AvaloniaPageProvider avaloniaPageProvider) {
        PageProvider = avaloniaPageProvider;
    }

    [RelayCommand]
    private void OnLoaded() {
        ActivePageIndex = 0;
    }

    [RelayCommand]
    private void ChangeActivePage() {
        ActivePageKey = ActivePageIndex switch {
            0 => "Download/News",
            1 => "Setting/Account",
            _ => "Download/News"
        };
    }
}