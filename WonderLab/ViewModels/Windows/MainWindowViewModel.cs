using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Models;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Controls;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Services;
using WonderLab.Services.Launch;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ObservableObject {
    private readonly TaskService _taskService;
    private readonly SettingService _settingService;
    private readonly GameProcessService _gameProcessService;

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
    [ObservableProperty] private bool _isDynamicPageDataLoading;
    [ObservableProperty] private bool _isGamesPanelOpen;
    [ObservableProperty] private bool _isTasksPanelOpen;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShieldBackgroundOpacity))]
    private int _activePageIndex = -1;

    public ReadOnlyObservableCollection<MinecraftProcessModel> MinecraftProcesses { get; }

    public MainWindowViewModel(AvaloniaPageProvider avaloniaPageProvider,
        SettingService settingService,
        GameProcessService gameProcessService,
        TaskService taskService) {
        _taskService = taskService;
        _settingService = settingService;
        _gameProcessService = gameProcessService;

        PageProvider = avaloniaPageProvider;

        Tasks = new(_taskService.Tasks);
        MinecraftProcesses = new(_gameProcessService.MinecraftProcesses);

        WeakReferenceMessenger.Default.Register<PageNotificationMessage>(this, (_, arg) => {
            ActivePageIndex = arg.PageKey is "Home" ? 0 : -1;
            PageKey = arg.PageKey;
        });

        WeakReferenceMessenger.Default.Register<DynamicPageNotificationMessage>(this, (_, arg) => {
            DynamicPageKey = string.Empty;
            DynamicPageKey = arg.PageKey;
            IsDynamicPageDataLoading = false;
        });

        WeakReferenceMessenger.Default.Register<PageDataLoadingMessage>(this, (_, arg) => {
            IsDynamicPageDataLoading = arg.IsLoading;
        });
    }

    [RelayCommand]
    private async Task OnLoaded() {
        await Task.Delay(160);
        ActivePageIndex = 0;
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
            1 => "Download/Navigation",
            2 => "Multiplayer",
            3 => "Setting/Navigation",
            _ => PageKey ?? "Home",
        };

        CloseAllPanel();
    }

    [RelayCommand]
    private async Task OpenTasksPanel() {
        IsGamesPanelOpen = false;
        if (string.IsNullOrEmpty(TasksPageKey)) {
            await Task.Delay(150);
            TasksPageKey = "Tasks";
        }
    }

    private void CloseAllPanel() {
        IsGamesPanelOpen = false;
        IsTasksPanelOpen = false;
    }
}