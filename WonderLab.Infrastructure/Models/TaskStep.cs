using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Components.Downloader;

namespace WonderLab.Infrastructure.Models;

public sealed partial class TaskStep : ObservableObject {
    [ObservableProperty] private string _stepName;
    [ObservableProperty] private double _maxProgress = 1d;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressIcon))]
    private TaskStatus _taskStatus = TaskStatus.WaitingToRun;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    private double _progress;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    private double? _speed = null;

    public string ProgressIcon => TaskStatus is TaskStatus.RanToCompletion ? "\uE73E" : "\uE712";
    public string ProgressText => Speed is null
        ? $"{StepName} - {Progress:P2}"
        : $"{StepName} - {Progress:P2} - {FileDownloader.GetSpeedText(Speed.GetValueOrDefault())}";
}