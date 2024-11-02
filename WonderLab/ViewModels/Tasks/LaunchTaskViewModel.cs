using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Infrastructure.Interfaces;
using WonderLab.Infrastructure.Models;

namespace WonderLab.ViewModels.Tasks;

public sealed partial class LaunchTaskViewModel : ObservableObject, ITaskJob<TaskProgress> {
    public readonly CancellationTokenSource LaunchCancellationTokenSource = new();

    [ObservableProperty] private double _progress;
    [ObservableProperty] private string _progressText;
    [ObservableProperty] private TaskStatus _taskStatus;
    [ObservableProperty] private bool _isIndeterminate = true;

    public Exception Exception { get; set; }
    public required string JobName { get; set; }

    public double MaxProgress => 1d;
    public CancellationToken TaskCancellationToken => LaunchCancellationTokenSource.Token;

    public ImmutableArray<TaskStep> TaskSteps { get; } = [
        new TaskStep { StepName = "Checking required items", TaskStatus = TaskStatus.Running },
        new TaskStep { StepName = "Verifying the account" },
        new TaskStep { StepName = "Completing the game dependents" },
        new TaskStep { StepName = "Launching the game" },
    ];

    public event EventHandler Completed;

    public void Report(TaskProgress value) => Dispatcher.UIThread.InvokeAsync(() => {
        if (value.Step is 1) {
            TaskStatus = TaskStatus.Running;
            IsIndeterminate = false;
            TaskSteps[0].Progress = value.Progress;
        } else if (value.Step is 2) {
            TaskSteps[0].TaskStatus = TaskStatus.RanToCompletion;
            TaskSteps[1].TaskStatus = TaskStatus.Running;
            TaskSteps[1].Progress = value.Progress;
        } else if (value.Step is 3) {
            TaskSteps[1].TaskStatus = TaskStatus.RanToCompletion;
            TaskSteps[2].TaskStatus = TaskStatus.Running;
            TaskSteps[2].Progress = value.Progress;
        } else if (value.Step is 4) {
            TaskSteps[2].TaskStatus = TaskStatus.RanToCompletion;
            TaskSteps[3].TaskStatus = TaskStatus.Running;
            TaskSteps[3].Progress = value.Progress;

            TaskStatus = TaskStatus.RanToCompletion;
            Completed?.Invoke(this, EventArgs.Empty);
        }

        Progress = TaskSteps.Select(x => x.Progress).Sum() / (double)TaskSteps.Length;
        ProgressText = Progress.ToString("P2");
    }, DispatcherPriority.ApplicationIdle);

    [RelayCommand]
    private void CancelTask() {
        LaunchCancellationTokenSource.Cancel();
    }
}