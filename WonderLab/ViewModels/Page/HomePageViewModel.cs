using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using WonderLab.Infrastructure.Models.Launch;
using WonderLab.Infrastructure.Models.Messaging;
using WonderLab.Services;
using WonderLab.Services.Launch;

namespace WonderLab.ViewModels.Page;

public sealed partial class HomePageViewModel : ObservableObject {
    private readonly GameService _gameService;
    private readonly ConfigService _configService;
    private readonly ILogger<HomePageViewModel> _logger;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LaunchCommand))]
    private GameModel _activeGame;

    public HomePageViewModel(GameService gameService, ConfigService configService, ILogger<HomePageViewModel> logger) {
        _logger = logger;
        _gameService = gameService;
        _configService = configService;

        _gameService.CollectionChanged += OnCollectionChanged;
        _gameService.RefreshGames();
    }

    private bool CanLaunch() => ActiveGame is not null;

    [RelayCommand(CanExecute = nameof(CanLaunch))]
    private void Launch() {
    }

    [RelayCommand]
    private void NavigationToGame() {
        WeakReferenceMessenger.Default.Send<PageNotificationMessage>(new("Game"));
    }

    private void OnCollectionChanged(object sender, EventArgs e) => Dispatcher.UIThread.InvokeAsync(() => {
        ActiveGame = _gameService.ActiveGame;
    });
}