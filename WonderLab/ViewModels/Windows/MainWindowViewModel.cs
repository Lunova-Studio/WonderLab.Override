using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WonderLab.Classes.Datas.MessageData;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Interfaces;
using WonderLab.Services;
using WonderLab.Services.Game;
using WonderLab.Services.Navigation;
using WonderLab.Services.UI;
using WonderLab.ViewModels.Pages;
using WonderLab.ViewModels.Pages.Navigation;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase {
    private GameService _gameService;
    private readonly TaskService _taskService;
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;
    private readonly LanguageService _languageService;
    private readonly NotificationService _notificationService;
    private readonly WeakReferenceMessenger _weakReferenceMessenger;

    public readonly HostNavigationService _navigationService;

    [ObservableProperty] private string _imagePath;

    [ObservableProperty] private int _blurRadius;

    [ObservableProperty] private object _activePage;
    [ObservableProperty] private ParallaxMode _parallaxMode;
    [ObservableProperty] private GameViewData _activeGameEntry;

    [ObservableProperty] private bool _isEnableBlur;
    [ObservableProperty] private bool _isAlignCenter;
    [ObservableProperty] private bool _isOpenGamePanel;
    [ObservableProperty] private bool _isOpenTaskListPanel;
    [ObservableProperty] private bool _isOpenBackgroundPanel;

    [ObservableProperty] private ReadOnlyObservableCollection<TaskViewData> _tasks;
    [ObservableProperty] private ReadOnlyObservableCollection<GameViewData> _gameEntries;
    [ObservableProperty] private ReadOnlyObservableCollection<INotification> _notifications;

    [ObservableProperty] private Control _homePage;

    public MainWindowViewModel(
        TaskService taskService,
        DialogService dialogService,
        SettingService settingService,
        LanguageService languageService,
        HostNavigationService navigationService,
        NotificationService notificationService,
        WeakReferenceMessenger weakReferenceMessenger) {
        _taskService = taskService;
        _dialogService = dialogService;
        _settingService = settingService;
        _languageService = languageService;
        _navigationService = navigationService;
        _notificationService = notificationService;
        _weakReferenceMessenger = weakReferenceMessenger;

        weakReferenceMessenger.Register<BlurEnableMessage>(this, (_, arg) => {
            IsEnableBlur = arg.IsEnableBlur;
        });

        weakReferenceMessenger.Register<BlurRadiusChangeMessage>(this, (_, arg) => {
            BlurRadius = arg.BlurRadius;
        });

        weakReferenceMessenger.Register<AlignCenterChangeMessage>(this, (_, arg) => {
            IsAlignCenter = arg.IsAlignCenter;
        });

        weakReferenceMessenger.Register<ParallaxModeChangeMessage>(this, (_, arg) => {
            ParallaxMode = arg.ParallaxMode switch {
                0 => ParallaxMode.None,
                1 => ParallaxMode.Flat,
                2 => ParallaxMode.Solid,
                _ => ParallaxMode.None
            };
        });

        weakReferenceMessenger.Register<SettingDataChangedMessage>(this, (_, _) => {
            _gameService = App.GetService<GameService>();
        });

        weakReferenceMessenger.Register<GameManagerMessage>(this, (_, _) => {
            IsOpenGamePanel = !IsOpenGamePanel;
            GameEntries = _gameService.GameEntries;
        });
    }

    [RelayCommand]
    private void ControlTaskListPanel() {
        IsOpenTaskListPanel = !IsOpenTaskListPanel;
    }

    [RelayCommand]
    private void NavigationTo(string pageKey) {
        IsOpenBackgroundPanel = pageKey switch {
            "HomePage" => false,
            "MultiplayerPage" => true,
            "SettingNavigationPage" => true,
            "DownloadNavigationPage" => true,
            _ => false
        };

        switch (pageKey) {
            case "HomePage":
                HomePage = _navigationService.NavigationToHome();
                break;
            case "MultiplayerPage":
                _navigationService.NavigationTo<MultiplayerPageViewModel>();
                break;
            case "SettingNavigationPage":
                _navigationService.NavigationTo<SettingNavigationPageViewModel>();
                break;
            case "DownloadNavigationPage":
                _navigationService.NavigationTo<DownloadNavigationPageViewModel>();
                break;
        }
    }

    public void OnLoaded() {
        Tasks = new(_taskService.DisplayTasks);
        Notifications = new(_notificationService.Notifications);

        IsAlignCenter = _settingService.Data.IsAlignCenter;
        ParallaxMode = _settingService.Data.ParallaxMode switch {
            0 => ParallaxMode.None,
            1 => ParallaxMode.Flat,
            2 => ParallaxMode.Solid,
            _ => ParallaxMode.None,
        };

        HomePage = _navigationService.NavigationToHome();
        GameEntries = _gameService.GameEntries;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        if (e.PropertyName is nameof(ActiveGameEntry)) {
            _gameService.ActivateGameViewEntry(ActiveGameEntry);
        }
    }
}