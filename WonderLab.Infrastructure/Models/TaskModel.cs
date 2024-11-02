using WonderLab.Infrastructure.Interfaces;

namespace WonderLab.Infrastructure.Models;

public sealed record TaskModel(ITaskJob<TaskProgress> TaskJob);