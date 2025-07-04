using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Extensions.Hosting.UI;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class DownloadNavigationPageViewModel : ObservableObject {
    private readonly AvaloniaPageProvider _pageProvider;

    [ObservableProperty] private string _activePageKey;
    [ObservableProperty] private ObservableCollection<string> _headerItems = ["下载"];

    public AvaloniaPageProvider PageProvider => _pageProvider;

    public DownloadNavigationPageViewModel(AvaloniaPageProvider pageProvider) {
        _pageProvider = pageProvider;
    }

    [RelayCommand]
    private void OnLoaded() => Dispatcher.UIThread.InvokeAsync(async () => {
        await Task.Delay(250);
        ActivePageKey = "Download/Dashboard";
    });
}