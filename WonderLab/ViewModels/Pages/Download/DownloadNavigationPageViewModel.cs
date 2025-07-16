using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Controls.Experimental.BreadcrumbBar;
using WonderLab.Extensions.Hosting.UI;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class DownloadNavigationPageViewModel : PageViewModelBase {
    private readonly AvaloniaPageProvider _pageProvider;

    [ObservableProperty] private string _activePageKey;
    [ObservableProperty] private ObservableCollection<string> _headerItems = ["Download"];

    public AvaloniaPageProvider PageProvider => _pageProvider;

    public DownloadNavigationPageViewModel(AvaloniaPageProvider pageProvider) {
        _pageProvider = pageProvider;

        WeakReferenceMessenger.Default.Register<RequestPageMessage>(this, (_, arg) => {
            Dispatcher.UIThread.Post(() => {
                if (HeaderItems.Contains(arg.S))
                    return;

                HeaderItems.Add(arg.S);
            });
        });
    }

    [RelayCommand]
    private Task OnLoaded() => Dispatcher.UIThread.InvokeAsync(async () => {
        await Task.Delay(250);
        ActivePageKey = "Download/Dashboard";
    });

    [RelayCommand]
    private void OnItemClicked(BreadcrumbBarItemClickedEventArgs arg) {
        var isHasChild = arg.Index + 1 <= HeaderItems.Count - 1;

        if (isHasChild) {
            HeaderItems.RemoveAt(arg.Index + 1);
            WeakReferenceMessenger.Default.Send(new RequestDownloadPageGobackMessage());
        }
    }
}