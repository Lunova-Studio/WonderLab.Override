using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Models.Network;
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

        WeakReferenceMessenger.Default.Register<RequestResourcePageMessage>(this, async (_, args) => {
            Dispatcher.UIThread.Post(() => {
                if (HeaderItems.Count > 1)
                    HeaderItems.Remove(HeaderItems.Last());

                HeaderItems.Add(args.ResourceName);
                ActivePageKey = args.Key;
            });

            await Task.Delay(10);
            WeakReferenceMessenger.Default.Send(new MinecraftResponseMessage(args.ResourceName,
                args.Parameter as VersionManifestEntry));
        });
    }

    [RelayCommand]
    private Task OnLoaded() => Dispatcher.UIThread.InvokeAsync(async () => {
        await Task.Delay(250);
        ActivePageKey = "Download/Dashboard";
    });

    [RelayCommand]
    private void OnItemClicked(BreadcrumbBarItemClickedEventArgs arg) {
        var last = HeaderItems.Last();
        var isHasTrueChild = last.Contains("Search");
        var isHasChild = arg.Index + 1 <= HeaderItems.Count - 1;

        if (isHasChild) {
            HeaderItems.Remove(last);
            if (isHasTrueChild)
                WeakReferenceMessenger.Default.Send(new RequestDownloadPageGobackMessage());
            else
                ActivePageKey = "Download/Dashboard";
        }
    }
}