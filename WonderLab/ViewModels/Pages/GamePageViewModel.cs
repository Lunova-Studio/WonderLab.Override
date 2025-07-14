using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Game;
using ObservableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services.Launch;

namespace WonderLab.ViewModels.Pages;

public sealed partial class GamePageViewModel : ObservableObject {
    private readonly GameService _gameService;
    private readonly ILogger<GamePageViewModel> _logger;
    private readonly ObservableDictionary<string, IEnumerable<MinecraftEntry>> _minecraftsDict = [];

    [ObservableProperty] private int _sortMode = 0;
    [ObservableProperty] private int _groupMode = 0;
    [ObservableProperty] private bool _hasMinecrafts = true;
    [ObservableProperty] private string _filter = string.Empty;

    public INotifyCollectionChangedSynchronizedViewList<KeyValuePair<string, IEnumerable<MinecraftEntry>>> Minecrafts { get; }

    public GamePageViewModel(GameService gameService, ILogger<GamePageViewModel> logger) {
        _logger = logger;
        _gameService = gameService;

        Minecrafts = _minecraftsDict.ToNotifyCollectionChanged();
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        await Task.Delay(100);
        _gameService?.RefreshGames();
        _minecraftsDict.Add("All", _gameService.Minecrafts);

        HasMinecrafts = _minecraftsDict.Count > 0;
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

    partial void OnFilterChanged(string value) {
        bool flag = string.IsNullOrEmpty(value);
        string key = flag ? "All" : "Filtered";
        var values = flag
            ? _gameService.Minecrafts
            : _gameService.Minecrafts.Where(x => x.Id.Contains(value, StringComparison.OrdinalIgnoreCase));

        if (_minecraftsDict.ContainsKey(key)) {
            _minecraftsDict[key] = values;
        } else {
            _minecraftsDict.Clear();
            _minecraftsDict.Add(key, values);
        }
    }

    partial void OnGroupModeChanged(int value) {
        _minecraftsDict.Clear();

        ILookup<string, MinecraftEntry> lookup = GroupMode switch {
            0 => _gameService.Minecrafts.ToLookup(_ => "All"),
            1 => _gameService.Minecrafts.ToLookup(x => x.Version.VersionId),
            2 => _gameService.Minecrafts.Where(x => x is ModifiedMinecraftEntry m && m.ModLoaders.FirstOrDefault() != default)
                    .ToLookup(x => ((ModifiedMinecraftEntry)x).ModLoaders.First().Type.ToString(), x => x),
            _ => throw new ArgumentOutOfRangeException(nameof(GroupMode), GroupMode, null)
        };

        foreach (var group in lookup) {
            _minecraftsDict[group.Key] = group;
        }
    }
}
