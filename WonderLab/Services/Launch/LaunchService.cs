using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Launch;
using System;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services.Authentication;
using WonderLab.ViewModels.Tasks;

namespace WonderLab.Services.Launch;

public sealed class LaunchService {
    private readonly GameService _gameService;
    private readonly TaskService _taskService;
    private readonly SettingService _settingService;
    private readonly AccountService _accountService;
    private readonly GameProcessService _gameProcessService;
    private readonly AuthenticationService _authenticationService;
    private readonly ILogger<LaunchService> _logger;

    public LaunchService(
        GameService gameService,
        TaskService taskService,
        SettingService settingService,
        GameProcessService gameProcessService,
        AccountService accountService,
        AuthenticationService authenticationService,
        ILogger<LaunchService> logger) {
        _logger = logger;
        _gameService = gameService;
        _taskService = taskService;
        _settingService = settingService;
        _accountService = accountService;
        _gameProcessService = gameProcessService;
        _authenticationService = authenticationService;
    }

    public Task<LaunchTaskViewModel> LaunchTaskAsync(MinecraftEntry entry) {
        LaunchTaskViewModel task = new() {
            JobName = entry?.Id // TODO: I18n
        };

        Task.Run(() => LaunchAsync(_settingService.Setting
            .ActiveAccount, entry, task));

        return Task.FromResult(task);
    }

    private async Task LaunchAsync(
        Account account,
        MinecraftEntry entry,
        LaunchTaskViewModel progress) {
        var cancellationToken = progress.TaskCancellationToken;

        try {
            progress.Report(new(0d, TaskStatus.Created));

            var settings = _settingService.Setting;
            progress.Report(new(0.35d, TaskStatus.Running));
            cancellationToken.ThrowIfCancellationRequested();

            //Auth
            progress.Report(new(0.5d, TaskStatus.Running));

            if (_accountService.ActiveAccount is MicrosoftAccount microsoftAccount)
                await _authenticationService.RefreshMicrosoftAccountAsync(microsoftAccount, cancellationToken);
            else if (_accountService.ActiveAccount is YggdrasilAccount yggdrasilAccount)
                await _authenticationService.RefreshYggdrasilAccountAsync(yggdrasilAccount, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            //Complete
            progress.Report(new(0.65d, TaskStatus.Running));

            await Task.Run(async () => {
                MinecraftResourceDownloader downloader = new(entry);
                downloader.ProgressChanged += (_, e) => {
                    progress.Report(new TaskProgress {
                        Speed = e.Speed,
                        IsSupportSpeed = true,
                        TaskStatus = TaskStatus.Running,
                        Progress = ((double)e.CompletedCount / (double)e.TotalCount).ToPercentage(0.65d, 0.95d)
                    });
                };

                var result = await downloader.VerifyAndDownloadDependenciesAsync(10, cancellationToken);
            });

            cancellationToken.ThrowIfCancellationRequested();

            //Launch
            progress.Report(new(0.95d, TaskStatus.Running));

            MinecraftRunner runner = new(new() {
                Account = account,
                MinMemorySize = 512,
                JavaPath = settings.ActiveJava,
                MaxMemorySize = settings.MaxMemorySize,
                IsEnableIndependency = settings.IsEnableIndependency,
                LauncherName = "WonderLab-Pre-Alpha"
            }, _gameService.MinecraftParser);

            if (settings.IsAutoSelectJava || settings.ActiveJava is null)
                runner.LaunchConfig.JavaPath = entry.GetAppropriateJava(settings.Javas);

            var gameProcess = await runner.RunAsync(entry, cancellationToken);
            _gameProcessService.AddProcess(gameProcess, entry);

            progress.ReportCompleted();
        } catch (TaskCanceledException) {
            progress.Report(new(1d, TaskStatus.Canceled));
            WeakReferenceMessenger.Default.Send(new NotificationMessage($"已中断游戏 {entry.Id} 的启动任务", NotificationType.Information));
        } catch (Exception ex) {
            progress.Report(new(1d, TaskStatus.Faulted));
            WeakReferenceMessenger.Default.Send(new NotificationMessage($"游戏 {entry.Id} 启动时遭遇了意料之外的异常：\n{ex}", NotificationType.Error));
        }
    }
}
