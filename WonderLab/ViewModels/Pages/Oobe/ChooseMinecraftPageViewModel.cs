using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ObservableCollections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class ChooseMinecraftPageViewModel : PageViewModelBase {
    private readonly SettingService _settingService;
    private readonly ObservableList<string> _mcFolders;

    [ObservableProperty] private string _activeMinecraftFolder;

    public INotifyCollectionChangedSynchronizedViewList<string> MinecraftFolders { get; }

    public ChooseMinecraftPageViewModel(SettingService settingService) {
        _settingService = settingService;
        _mcFolders = [.. _settingService.Setting.MinecraftFolders];

        MinecraftFolders = _mcFolders.ToNotifyCollectionChangedSlim();
        ActiveMinecraftFolder = _settingService.Setting.ActiveMinecraftFolder;
        SendEnabledMessage();

        _mcFolders.CollectionChanged += OnCollectionChanged;
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(ActiveMinecraftFolder)) {
            _settingService.Setting.ActiveMinecraftFolder = ActiveMinecraftFolder;
            SendEnabledMessage();
        }
    }

    private void SendEnabledMessage() {
        WeakReferenceMessenger.Default.Send(new EnabledChangedMessage(_mcFolders.Count > 0 
            && !string.IsNullOrEmpty(ActiveMinecraftFolder)));
    }

    private void OnCollectionChanged(in NotifyCollectionChangedEventArgs<string> e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                _settingService.Setting.MinecraftFolders.Add(_mcFolders.Last());
                break;
            case NotifyCollectionChangedAction.Remove:
                _settingService.Setting.MinecraftFolders.Remove(e.OldItems[0].ToString());
                break;
        }

        SendEnabledMessage();
    }

    [RelayCommand]
    private Task BrowserFolder() => Dispatcher.UIThread.InvokeAsync(async () => {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
            var result = await lifetime.MainWindow.StorageProvider.OpenFolderPickerAsync(new() {
                AllowMultiple = false,
            });

            if (result is { Count: 0 })
                return;

            var path = result[0].Path.LocalPath;
            _mcFolders.Add(path);
            ActiveMinecraftFolder = _mcFolders.Last();
        }
    }, DispatcherPriority.Background);
}