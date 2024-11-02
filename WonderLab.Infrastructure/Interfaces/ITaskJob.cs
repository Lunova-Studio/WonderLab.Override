using CommunityToolkit.Mvvm.Input;
using System.Collections.Immutable;
using WonderLab.Infrastructure.Models;

namespace WonderLab.Infrastructure.Interfaces;

public interface ITaskJob<in T> : IProgress<T> {
    string JobName { get; }
    double MaxProgress { get; }
    string ProgressText { get; }
    double Progress { get; set; }
    bool IsIndeterminate { get; set; }
    Exception Exception { get; }
    TaskStatus TaskStatus { get; set; }
    IRelayCommand CancelTaskCommand { get; }
    CancellationToken TaskCancellationToken { get; }

    ImmutableArray<TaskStep> TaskSteps { get; }

    event EventHandler Completed;
}