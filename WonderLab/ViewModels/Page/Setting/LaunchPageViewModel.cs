using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Fetcher;
using MinecraftLaunch.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using WonderLab.Infrastructure.Models;
using WonderLab.Services;
using WonderLab.Services.Launch;
using WonderLab.Extensions;

namespace WonderLab.ViewModels.Page.Setting;

public sealed partial class LaunchPageViewModel : ObservableObject {
    private readonly GameService _gameService;
    private readonly ConfigService _configService;
    private readonly JavaFetcher _javaFetcher = new();

    private ObservableCollection<JavaEntry> _javaEntrys;
    private ObservableCollection<string> _minecraftFolders;

    [ObservableProperty] private bool _isFullScreen;
    [ObservableProperty] private bool _isAutoSelectJava;
    [ObservableProperty] private bool _isGameIndependent;
    [ObservableProperty] private bool _isAutoAllocateMemory;

    [ObservableProperty] private int _maxMemory;
    [ObservableProperty] private string _activeFolder;
    [ObservableProperty] private JavaEntry _activeJava;
    [ObservableProperty] private ReadOnlyObservableCollection<string> _folders;
    [ObservableProperty] private ReadOnlyObservableCollection<JavaEntry> _javas;

    public Config Config => _configService.Entries;

    public LaunchPageViewModel(ConfigService configService, GameService gameService) {
        _gameService = gameService;
        _configService = configService;
    }

    [RelayCommand]
    private void OnLoaded() {
        Dispatcher.UIThread.Post(() => {
            _javaEntrys = new(Config.Javas);
            _minecraftFolders = new(Config.MinecraftFolders);

            Javas = new(_javaEntrys);
            Folders = new(_minecraftFolders);

            ActiveJava = Config.ActiveJava;
            ActiveFolder = Config.ActiveMinecraftFolder;
        });
    }

    [RelayCommand]
    private void RemoveItem(string key) {
        if (key == "Folder" && _minecraftFolders.Remove(ActiveFolder)) {
            Config.MinecraftFolders = [.. _minecraftFolders];
            ActiveFolder = _minecraftFolders.FirstOrDefault();
        } else if (_javaEntrys.Remove(ActiveJava)) {
            Config.Javas = [.. _javaEntrys];
            ActiveJava = _javaEntrys.FirstOrDefault();
        }
    }

    [RelayCommand]
    private Task SearchJava() => Task.Run(async () => {
        var result = await _javaFetcher.FetchAsync();
        var javas = _javaEntrys.Union(result);

        _javaEntrys.Clear();
        foreach (var java in javas) {
            _javaEntrys.Add(java);
        }

        Config.Javas = [.. _javaEntrys];
        Config.ActiveJava = ActiveJava = _javaEntrys.LastOrDefault();
    });

    [RelayCommand]
    private Task BrowserJava() => Dispatcher.UIThread.InvokeAsync(async () => {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
            var result = await lifetime.MainWindow.StorageProvider.OpenFolderPickerAsync(new() {
                AllowMultiple = false,
            });

            if (result is { Count: 0 }) {
                return;
            }

            _javaEntrys.Add(JavaUtil.GetJavaInfo(result[0].Path.LocalPath));
            Config.ActiveJava = ActiveJava = _javaEntrys.Last();

            Config.Javas.Add(ActiveJava);
        }
    });

    [RelayCommand]
    private Task BrowserFolder() => Dispatcher.UIThread.InvokeAsync(async () => {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
            var result = await lifetime.MainWindow.StorageProvider.OpenFolderPickerAsync(new() {
                AllowMultiple = false,
            });

            if (result is { Count: 0 }) {
                return;
            }

            _minecraftFolders.Add(result[0].Path.LocalPath);
            Config.ActiveMinecraftFolder = ActiveFolder = _minecraftFolders.Last();
            Config.MinecraftFolders.Add(ActiveFolder);
        }
    });

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        switch (e.PropertyName) {
            case nameof(ActiveFolder):
                _gameService.RefreshGames();
                Config.ActiveMinecraftFolder = ActiveFolder;
                break;
            case nameof(MaxMemory):
                Config.MaxMemory = MaxMemory;
                break;
            case nameof(ActiveJava):
                Config.ActiveJava = ActiveJava;
                break;
            case nameof(IsFullScreen):
                Config.IsFullScreen = IsFullScreen;
                break;
            case nameof(IsAutoSelectJava):
                Config.IsAutoSelectJava = IsAutoSelectJava;
                break;
            case nameof(IsGameIndependent):
                Config.IsGameIndependent = IsGameIndependent;
                break;
            case nameof(IsAutoAllocateMemory):
                Config.IsAutoAllocateMemory = IsAutoAllocateMemory;
                break;
        }
    }
}