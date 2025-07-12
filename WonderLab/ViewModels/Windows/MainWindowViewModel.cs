using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Models;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Controls;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Override.I18n;
using WonderLab.Services;
using WonderLab.Services.Launch;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ObservableObject {
    private readonly TaskService _taskService;
    private readonly SettingService _settingService;
    private readonly GameProcessService _gameProcessService;

    private bool _isManualTrigger;

    public double ShieldBackgroundOpacity => ActivePageIndex is 0
        ? 0
        : _settingService?.Setting?.ActiveBackground
            is BackgroundType.Bitmap or BackgroundType.Voronoi ? 1 : 0;

    public AvaloniaPageProvider PageProvider { get; }
    public ReadOnlyObservableCollection<TaskModel> Tasks { get; private set; }

    [ObservableProperty] private string _pageKey;
    [ObservableProperty] private string _dynamicPageKey;
    [ObservableProperty] private bool _isDynamicPageDataLoading;
    [ObservableProperty] private BarState _barState = BarState.Collapsed;

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
            _isManualTrigger = true;
            DynamicPageKey = string.Empty;
            DynamicPageKey = arg.PageKey;
            BarState = BarState.Expanded;
            IsDynamicPageDataLoading = false;
            _isManualTrigger = false;
        });

        WeakReferenceMessenger.Default.Register<DynamicPageCloseNotificationMessage>(this, (_, arg) => {
            BarState = PageKey is "Home" ? BarState.Collapsed : BarState.Hidden;
        });

        WeakReferenceMessenger.Default.Register<PageDataLoadingMessage>(this, (_, arg) => {
            IsDynamicPageDataLoading = arg.IsLoading;
        });
    }

    [RelayCommand]
    private async Task OnLoaded() {
        await Task.Delay(160);
        ActivePageIndex = 0;

        PropertyChanged += OnPropertyChanged;
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

        BarState = ActivePageIndex is 0 ? BarState.Collapsed : BarState.Hidden;
    }

    [RelayCommand]
    private void ShowTaskCenter() {
        _isManualTrigger = true;
        BarState = BarState.Expanded;
        DynamicPageKey = "Tasks";
        _isManualTrigger = false;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(BarState) && BarState is BarState.Expanded && !_isManualTrigger)
            DynamicPageKey = "Dashboard";
    }
}