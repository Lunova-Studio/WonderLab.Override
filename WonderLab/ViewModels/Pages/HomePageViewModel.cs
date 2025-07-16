using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Models.Game;
using System.Threading.Tasks;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services;
using WonderLab.Services.Launch;
using WonderLab.ViewModels.Tasks;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : PageViewModelBase {
    private readonly GameService _gameService;
    private readonly LaunchService _launchService;
    private readonly SettingService _settingService;

    [ObservableProperty] private MinecraftEntry _activeMinecraft;
    [ObservableProperty] private string _launchButtonText = "启动";
    [ObservableProperty] private LaunchTaskViewModel _currentLaunchTask;

    public HomePageViewModel(SettingService settingService, GameService gameService, LaunchService launchService) {
        _gameService = gameService;
        _launchService = launchService;
        _settingService = settingService;

        _gameService.ActivateMinecraft(_settingService.Setting.ActiveGameId);
        ActiveMinecraft = _gameService.ActiveGame;
    }

    [RelayCommand]
    private void NavigationToGame() {
        WeakReferenceMessenger.Default.Send<PageNotificationMessage>(new("Game"));
    }

    [RelayCommand]
    private async Task Launch() {
        LaunchButtonText = "启动中";
        CurrentLaunchTask = await _launchService.LaunchTaskAsync(_gameService.ActiveGame);

        CurrentLaunchTask.Completed += (_, _) => WeakReferenceMessenger.Default.Send(
            new NotificationMessage($"游戏 {_gameService.ActiveGame.Id} 启动成功，祝您游戏愉快！", 
                NotificationType.Success));
    }
}