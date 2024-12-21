using WonderLab.Infrastructure.Interfaces;

namespace WonderLab.Infrastructure.Models;

public record struct TaskProgress(int Step, double Progress, Exception Exception = default, double? Speed = default) : ITaskProgress;