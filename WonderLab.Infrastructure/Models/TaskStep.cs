using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.Infrastructure.Models;

public sealed partial class TaskStep : ObservableObject {
    [ObservableProperty] private string _stepName;
    [ObservableProperty] private double _maxProgress = 1d;
    [ObservableProperty] private TaskStatus _taskStatus = TaskStatus.WaitingToRun;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    private double _progress;

    public string ProgressText => Progress.ToString("P2");
}