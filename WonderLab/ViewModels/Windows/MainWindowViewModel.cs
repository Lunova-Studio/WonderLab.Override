using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Models.Game;
using ObservableCollections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Models;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Services;
using WonderLab.Services.Authentication;
using WonderLab.Services.Launch;
using WonderLab.ViewModels.Tasks;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ObservableObject {
    private readonly TaskService _taskService;
    private readonly GameService _gameService;
    private readonly LaunchService _launchService;
    private readonly AccountService _accountService;
    private readonly SettingService _settingService;
    private readonly GameProcessService _gameProcessService;

    private readonly ObservableList<AccountModel> _accounts = [];

    public double ShieldBackgroundOpacity => ActivePageIndex is 0
        ? 0
        : _settingService?.Setting?.ActiveBackground
            is BackgroundType.Bitmap or BackgroundType.Voronoi ? 1 : 0;

    public AvaloniaPageProvider PageProvider { get; }
    public ReadOnlyObservableCollection<TaskModel> Tasks { get; private set; }

    [ObservableProperty] private string _pageKey;
    [ObservableProperty] private string _gamesPageKey;
    [ObservableProperty] private string _tasksPageKey;
    [ObservableProperty] private string _dynamicPageKey;
    [ObservableProperty] private bool _isTasksPanelOpen;
    [ObservableProperty] private AccountModel _activeAccount;
    [ObservableProperty] private MinecraftEntry _activeMinecraft;
    [ObservableProperty] private LaunchTaskViewModel _activeTask;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShieldBackgroundOpacity))]
    private int _activePageIndex = -1;

    public ReadOnlyObservableCollection<MinecraftEntry> Minecrafts { get; }
    public ReadOnlyObservableCollection<MinecraftProcessModel> MinecraftProcesses { get; }
    public INotifyCollectionChangedSynchronizedViewList<AccountModel> Accounts { get; }

    public MainWindowViewModel(
        AvaloniaPageProvider avaloniaPageProvider,
        SettingService settingService,
        GameProcessService gameProcessService,
        TaskService taskService,
        GameService gameService,
        AccountService accountService,
        LaunchService launchService) {
        _taskService = taskService;
        _gameService = gameService;
        _launchService = launchService;
        _settingService = settingService;
        _accountService = accountService;
        _gameProcessService = gameProcessService;

        PageProvider = avaloniaPageProvider;

        Tasks = new(_taskService.Tasks);
        Minecrafts = new(_gameService.Minecrafts);
        MinecraftProcesses = new(_gameProcessService.MinecraftProcesses);
        Accounts = _accounts.ToNotifyCollectionChangedSlim();

        WeakReferenceMessenger.Default.Register<PageNotificationMessage>(this, (_, arg) => {
            ActivePageIndex = arg.PageKey is "Home" ? 0 : -1;
            PageKey = arg.PageKey;
        });
    }

    [RelayCommand]
    private async Task OnLoaded() {
        await Task.Delay(160);
        ActivePageIndex = 0;
        _gameService.RefreshGames();
        //_accounts.AddRange(_accountService.Accounts
        //    .Select(x => new AccountModel(x, default)));

        ActiveMinecraft = _gameService.ActiveGame;
        ActiveAccount = Accounts.FirstOrDefault(x => x.Account.ProfileEquals(_accountService.ActiveAccount))
            ?? Accounts.FirstOrDefault();

        WeakReferenceMessenger.Default.Register<ActiveMinecraftChangedMessage>(this, (_, _) => {
            ActiveMinecraft = _gameService.ActiveGame;
        });
    }

    [RelayCommand]
    private async Task OpenGamesPanel() {
        IsTasksPanelOpen = false;
        if (string.IsNullOrEmpty(GamesPageKey)) {
            await Task.Delay(150);
            GamesPageKey = "Game";
        }
    }

    [RelayCommand]
    private void ChangeActivePage() {
        PageKey = ActivePageIndex switch {
            0 => "Home",
            1 => "Game",
            2 => "Download/Navigation",
            3 => "Multiplayer",
            4 => "Setting/Navigation",
            _ => PageKey ?? "Home",
        };

        CloseAllPanel();
    }

    [RelayCommand]
    private async Task OpenTasksPanel() {
        if (string.IsNullOrEmpty(TasksPageKey)) {
            await Task.Delay(150);
            TasksPageKey = "Tasks";
        }
    }

    [RelayCommand]
    private Task Launch() => Task.Run(async () => {
        ActiveTask = await _launchService.LaunchTaskAsync(_gameService.ActiveGame);

        ActiveTask.Completed += async (_, _) => {
            WeakReferenceMessenger.Default.Send(new NotificationMessage($"游戏 {_gameService.ActiveGame.Id} 启动成功，祝您游戏愉快！",
                NotificationType.Success));

            await Task.Delay(1000);
            ActiveTask = null;
        };
    });

    private void CloseAllPanel() {
        IsTasksPanelOpen = false;
    }

    partial void OnActiveAccountChanged(AccountModel value) {
        _accountService.ActivateAccount(value?.Account);
    }

    partial void OnActiveMinecraftChanged(MinecraftEntry value) {
        _gameService.ActivateMinecraft(value);
    }
}