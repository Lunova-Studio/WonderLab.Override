using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Base.Models.Authentication;
using ObservableCollections;
using System.ComponentModel;
using WonderLab.Classes.Models;
using WonderLab.Services;
using WonderLab.Services.Authentication;
using WonderLab.Services.Launch;

namespace WonderLab.ViewModels.Pages.GameSetting;

public sealed partial class GameSettingPageViewModel : ObservableObject {
    private readonly GameService _gameService;
    private readonly SettingService _settingService;
    private readonly AccountService _accountService;

    private SpecificSettingModel _profile;

    [ObservableProperty] private bool _isFullScreen;
    [ObservableProperty] private bool _isEnableIndependency;
    [ObservableProperty] private int _width;
    [ObservableProperty] private int _height;
    [ObservableProperty] private string _jvmArgument;
    [ObservableProperty] private string _serverAddress;
    [ObservableProperty] private bool _isEnableSpecificSetting;
    [ObservableProperty] private Account _activeAccount;

    public INotifyCollectionChangedSynchronizedViewList<Account> Accounts { get; }

    public GameSettingPageViewModel(GameService gameService, AccountService accountService, SettingService settingService) {
        _gameService = gameService;
        _settingService = settingService;
        _accountService = accountService;

        Accounts = _accountService.Accounts.ToNotifyCollectionChangedSlim();
    }

    [RelayCommand]
    private void OnLoaded() {
        if (_gameService.TryGetMinecraftProfile(out _profile)) {
            IsEnableSpecificSetting = _profile.IsEnableSpecificSetting;
            IsEnableIndependency = _profile.IsEnableIndependency;
            IsFullScreen = _profile.IsFullScreen;

            Width = _profile.Width;
            Height = _profile.Height;

            JvmArgument = _profile.JvmArgument;
            ServerAddress = _profile.ServerAddress;
            ActiveAccount = _profile.ActiveAccount;
        }

        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(IsFullScreen):
                _profile.IsFullScreen = IsFullScreen;
                break;
            case nameof(IsEnableIndependency):
                _profile.IsEnableIndependency = IsEnableIndependency;
                break;
            case nameof(IsEnableSpecificSetting):
                _profile.IsEnableSpecificSetting = IsEnableSpecificSetting;
                break;
            case nameof(Width):
                _profile.Width = Width;
                break;
            case nameof(Height):
                _profile.Height = Height;
                break;
            case nameof(JvmArgument):
                _profile.JvmArgument = JvmArgument;
                break;
            case nameof(ServerAddress):
                _profile.ServerAddress = ServerAddress;
                break;
            case nameof(ActiveAccount):
                _profile.ActiveAccount = ActiveAccount;
                break;
        }
    }
}