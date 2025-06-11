using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Game;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services.Launch;
using WonderLab.ViewModels.Pages.GameSetting;

namespace WonderLab.ViewModels.Pages;

public sealed partial class GamePageViewModel : ObservableObject {
    private readonly GameService _gameService;
    private readonly ILogger<GamePageViewModel> _logger;

    [ObservableProperty] private bool _hasMinecrafts;

    [ObservableProperty]
    private ReadOnlyObservableCollection<MinecraftEntry> _minecrafts;

    public GamePageViewModel(GameService gameService, ILogger<GamePageViewModel> logger) {
        _logger = logger;
        _gameService = gameService;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(() => {
        _gameService?.RefreshGames();
        Minecrafts = new(_gameService.Minecrafts);

        HasMinecrafts = Minecrafts.Count > 0;
    });

    [RelayCommand]
    private void ActiveMinecraft(MinecraftEntry minecraft) {
        _gameService.ActivateMinecraft(minecraft);
        WeakReferenceMessenger.Default.Send<PageNotificationMessage>(new("Home"));
    }

    [RelayCommand]
    private void OpenGameSettingPage(MinecraftEntry minecraft) {
        _gameService.ActiveGameCache = minecraft;
        WeakReferenceMessenger.Default.Send<DynamicPageNotificationMessage>(new("GameSetting/Navigation"));
    }
}
