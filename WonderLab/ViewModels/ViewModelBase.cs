using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DialogHostAvalonia;
using System.Threading;
using WonderLab.Classes.Models.Messaging;

namespace WonderLab.ViewModels;


public partial class PageViewModelBase : ObservableObject {
    protected virtual CancellationTokenSource CancellationTokenSource { get; set; } = new();

    protected virtual void Cancel() {
        using (CancellationTokenSource) {
            CancellationTokenSource.Cancel();
        }

        CancellationTokenSource = new();
    }

    [RelayCommand]
    protected virtual void OnNavigated() { }

    [RelayCommand]
    protected virtual void OnUnNavigated() {
        Cancel();
    }
}

public partial class DialogViewModelBase : ObservableObject {
    [RelayCommand]
    public virtual void Close() => Dispatcher.UIThread.Post(() => DialogHost.Close("Host"));
}

public partial class DynamicPageViewModelBase : PageViewModelBase {
    [RelayCommand]
    public virtual void Close() =>
        WeakReferenceMessenger.Default.Send(new DynamicPageCloseNotificationMessage());
}