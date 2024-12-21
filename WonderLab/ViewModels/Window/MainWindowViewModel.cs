using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Classes.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Infrastructure.Models;
using WonderLab.Infrastructure.Models.Launch;
using WonderLab.Infrastructure.Models.Messaging;
using WonderLab.Services;
using WonderLab.Services.Launch;

namespace WonderLab.ViewModels.Window;

public sealed partial class MainWindowViewModel : ObservableObject {
    private readonly TaskService _taskService;

    public double BackgroundOpacity => ActivePageIndex is 0 ? 0 : 0.45;

    public AvaloniaPageProvider PageProvider { get; }
    public ReadOnlyObservableCollection<TaskModel> Tasks { get; }
    public ReadOnlyObservableCollection<GameProcess> GameProcesses { get; }

    [ObservableProperty] private string _pageKey;
    [ObservableProperty] private object _activePage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BackgroundOpacity))]
    private int _activePageIndex;

    public MainWindowViewModel(AvaloniaPageProvider avaloniaPageProvider, TaskService taskService, LaunchService launchService) {
        _taskService = taskService;
        PageProvider = avaloniaPageProvider;
        Tasks = _taskService.Tasks;
        GameProcesses = new(launchService.GameProcesses);

        WeakReferenceMessenger.Default.Register<PageNotificationMessage>(this, (_, arg) => {
            ActivePageIndex = arg.PageKey is "Home" ? 0 : - 1;
            PageKey = arg.PageKey;
        });
    }

    [RelayCommand]
    private void GoToTaskList() {
        ActivePageIndex = -1;
        PageKey = "TaskList";
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
    }

    [RelayCommand]
    private void KillGameProcess(GameProcess gameProcess) {
        gameProcess.ProcessWatcher.Process.Kill();
    }
}