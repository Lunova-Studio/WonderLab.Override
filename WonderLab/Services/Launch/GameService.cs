using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Checker;
using MinecraftLaunch.Components.Resolver;
using MinecraftLaunch.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Infrastructure.Models.Launch;

namespace WonderLab.Services.Launch;

public sealed class GameService {
    private readonly ConfigService _configService;
    private readonly ILogger<GameService> _logger;
    private readonly ObservableCollection<GameModel> _gameEntries = new();

    public event EventHandler CollectionChanged;
    public event EventHandler ActiveGameChanged;

    public GameModel ActiveGame { get; private set; }
    public IGameResolver GameResolver { get; private set; }
    public ReadOnlyObservableCollection<GameModel> Games { get; }


    public GameService(ConfigService configService, ILogger<GameService> logger) {
        _logger = logger;
        _configService = configService;
        Games = new ReadOnlyObservableCollection<GameModel>(_gameEntries);

        if (!string.IsNullOrEmpty(_configService?.Entries?.ActiveMinecraftFolder)) {
            _ = Task.Run(Initialize);
        }
    }

    public void Initialize() {
        _logger.LogInformation("Initializing game service.");
        RefreshGames();
    }

    public void RefreshGames() {
        _gameEntries.Clear();
        if (string.IsNullOrEmpty(_configService?.Entries?.ActiveMinecraftFolder)) {
            return;
        }
        
        GameResolver = new GameResolver(_configService?.Entries?.ActiveMinecraftFolder);
        var games = GameResolver.GetGameEntitys();
        var root = Path.Combine(GameResolver.Root.FullName, "gamedata.json");

        if (!File.Exists(root)) {
            File.WriteAllText(root, games.Select(x => new GameJsonModel() {
                IsCollection = false,
                Id = x.Id,
                MinecraftFolder = x.GameFolderPath
            }).AsJson());
        }

        var jsonModels = File.ReadAllText(root).AsJsonEntry<List<GameJsonModel>>();
        var models = games.Select(x => ParseGameModel(x, jsonModels)).ToList();

        if (!models.Any()) {
            return;
        }

        foreach (var game in models) {
            _gameEntries.Add(game);
        }

        if (!string.IsNullOrEmpty(_configService?.Entries?.ActiveGameId)) {
            var game = GameResolver.GetGameEntity(_configService.Entries.ActiveGameId);
            if (game != null) {
                ActivateGame(ParseGameModel(game, jsonModels));
            } else {
                Empty();
            }
        } else {
            Empty();
        }

        if (_gameEntries.Any() && ActiveGame == null) {
            ActivateGame(_gameEntries.First());
        }

        Save();
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Empty() {
        ActiveGame = null!;
        _configService.Entries.ActiveGameId = null!;
    }

    public void ActivateGame(GameModel gameModel) {
        if (ActiveGame == null || !ActiveGame.Equals(gameModel)) {
            ActiveGame = gameModel ?? ActiveGame;
            _configService.Entries.ActiveGameId = gameModel?.Entry.Id ?? ActiveGame.Entry.Id;
        }

        ActiveGameChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Save() {
        var root = Path.Combine(_configService?.Entries?.ActiveMinecraftFolder, "gamedata.json");
        File.WriteAllText(root, Games.Select(x => x.Model).AsJson());
    }

    private GameModel ParseGameModel(GameEntry entry, List<GameJsonModel> gameJsonModels) {
        var model = gameJsonModels?.FirstOrDefault(x => x.Id == entry.Id) ?? new GameJsonModel() {
            Id = entry.Id,
            IsCollection = false,
            MinecraftFolder = entry.GameFolderPath
        };

        return new GameModel(entry, model);
    }
}

public sealed partial class GameViewModel : ObservableObject {
    private readonly GameService _gameService;

    public GameModel Model { get; }

    public GameViewModel(GameModel gameModel, GameService gameService) {
        Model = gameModel;
        _gameService = gameService;
    }

    [RelayCommand]
    private void Delete() {
        _gameService.Save();
    }


    [RelayCommand]
    private void Collect() {
        Model.Model.IsCollection = true;
        _gameService.Save();
    }
}