using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch;
using System.ComponentModel;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class NetworkPageViewModel : ObservableObject {
    private readonly SettingService _settingService;

    [ObservableProperty] private int _maxThread = 128;
    [ObservableProperty] private bool _isEnableMirror;

    public NetworkPageViewModel(SettingService settingService) {
        _settingService = settingService;

        MaxThread = _settingService.Setting.MaxThread;
        IsEnableMirror = _settingService.Setting.IsEnableMirror;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        switch (e.PropertyName) {
            case nameof(MaxThread):
                DownloadManager.MaxThread =
                    _settingService.Setting.MaxThread = MaxThread;
                break;
            case nameof(IsEnableMirror):
                DownloadManager.IsEnableMirror =
                    _settingService.Setting.IsEnableMirror = IsEnableMirror;
                break;
        }
    }
}