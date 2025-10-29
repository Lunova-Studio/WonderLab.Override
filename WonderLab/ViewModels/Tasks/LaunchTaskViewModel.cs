using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Components.Downloader;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using WonderLab.Classes.Models;
using Timer = System.Timers.Timer;

namespace WonderLab.ViewModels.Tasks;

public sealed partial class LaunchTaskViewModel : ObservableObject, ITaskJob<TaskProgress> {
    private readonly CancellationTokenSource _launchCancellationTokenSource = new();

    public string ProgressText => Progress.ToString("P2");
    public string SpeedText => Speed is null ? string.Empty : DefaultDownloader.FormatSize(Speed.Value, true);

    [ObservableProperty] private TaskStatus _taskStatus;
    [ObservableProperty] private bool _isSupportSpeed;
    [ObservableProperty] private bool _isIndeterminate = true;
    [ObservableProperty] private string _runningTimeText = "00:00";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SpeedText))]
    private double? _speed = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    private double _progress;

    public Exception Exception { get; set; }
    public required string JobName { get; set; }

    public CancellationToken TaskCancellationToken => _launchCancellationTokenSource.Token;

    public event EventHandler Completed;
    public event EventHandler ProgressChanged;

    public LaunchTaskViewModel() {
        Stopwatch stopwatch = new();
        Timer timer = new(TimeSpan.FromSeconds(1));
        timer.Elapsed += (_, e) => {
            RunningTimeText = stopwatch.Elapsed.ToString("mm\\:ss");

            if (TaskStatus is TaskStatus.RanToCompletion or TaskStatus.Canceled or TaskStatus.Faulted) {
                if (stopwatch.IsRunning)
                    stopwatch.Stop();

                timer.Stop();
                timer.Dispose();
            }
        };

        stopwatch.Start();
        _ = Task.Run(timer.Start);
    }

    public void Report(TaskProgress value) => Dispatcher.UIThread.InvokeAsync(() => {
        Speed = value.Speed;
        Progress = value.Progress;
        TaskStatus = value.TaskStatus;
        IsSupportSpeed = value.IsSupportSpeed;
        ProgressChanged?.Invoke(this, EventArgs.Empty);
    });

    public void ReportCompleted() {
        Report(new(1d, TaskStatus.RanToCompletion));
        Completed?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void CancelTask() {
        _launchCancellationTokenSource.Cancel();
        ReportCompleted();
    }
}