using Avalonia.Threading;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Models.Download;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Components.Checker;
using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Components.Launcher;
using MinecraftLaunch.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Infrastructure.Models;
using WonderLab.Infrastructure.Models.Launch;
using WonderLab.Services.Accounts;
using WonderLab.Services.UI;
using WonderLab.ViewModels.Tasks;

namespace WonderLab.Services.Launch;

public sealed class LaunchService {
    private readonly TaskService _taskService;
    private readonly GameService _gameService;
    private readonly ConfigService _configService;
    private readonly AccountService _accountService;
    private readonly NotificationService _notificationService;

    public ObservableCollection<GameProcess> GameProcesses { get; }

    public LaunchService(
        GameService gameService,
        TaskService taskService,
        ConfigService configService,
        AccountService accountService,
        NotificationService notificationService) {
        _taskService = taskService;
        _gameService = gameService;
        _configService = configService;
        _accountService = accountService;
        _notificationService = notificationService;

        GameProcesses = [];
    }

    public async Task LaunchTaskAsync(GameModel game) {
        LaunchTaskViewModel task = new() {
            JobName = $"游戏 {game.Entry.Id} 的启动任务"
        };

        _taskService.QueueJob(task);
        await Task.Run(async () => await LaunchAsync(_configService.Entries.ActiveAccount, task, task.TaskCancellationToken));
    }

    private async Task LaunchAsync(
        MinecraftLaunch.Classes.Models.Auth.Account account,
        IProgress<TaskProgress> progress,
        CancellationToken cancellationToken) {
        double progressCache = 0d;
        double? speed = null;
        DispatcherTimer dispatcherTimer = new(DispatcherPriority.ApplicationIdle) {
            Interval = TimeSpan.FromSeconds(0.2d),
        };

        dispatcherTimer.Tick += (_, _) => {
            progress.Report(new(3, progressCache, Speed: speed));
        };

        try {
            var config = _configService.Entries;

            progress.Report(new(1, 0d));

            if (config.ActiveJava is null) {
                throw new ArgumentException("未选择任何 Java 实例");
            }

            var javaPath = config.IsAutoSelectJava ? config.ActiveJava : config.ActiveJava!;
            progress.Report(new(1, 1d));
            cancellationToken.ThrowIfCancellationRequested();

            //Auth
            progress.Report(new(2, 0d));

            //await RefreshAccountAsync(account);

            progress.Report(new(2, 1d));
            cancellationToken.ThrowIfCancellationRequested();

            //Complete
            progress.Report(new(3, 0d));

            var checker = new ResourceChecker(_gameService.ActiveGame.Entry);
            var canComplete = await checker.CheckAsync();
            if (!canComplete) {
                dispatcherTimer.Start();
                await checker.MissingResources.DownloadResourceEntrysAsync(new DownloaderConfiguration {
                    IsEnableFragmentedDownload = true,
                    MaxThread = 256,
                }, e => {
                    speed = e.Speed;
                    progressCache = (double)e.CompletedCount / (double)checker.MissingResources.Count;
                }, cancellationToken);
                dispatcherTimer.Stop();
            }


            progress.Report(new(3, 1d));
            cancellationToken.ThrowIfCancellationRequested();

            //Launch
            progress.Report(new(4, 0d));

            Launcher launcher = new(_gameService.GameResolver, new() {
                JvmConfig = new JvmConfig(javaPath.JavaPath) {
                    MaxMemory = config.MaxMemory,
                },
                Account = account ?? new OfflineAuthenticator("Steve").Authenticate(),
                IsEnableIndependencyCore = config.IsGameIndependent,
                LauncherName = "WonderLab"
            });

            var gameProcessWatcher = await launcher.LaunchAsync(_gameService.ActiveGame.Entry.Id);
            var gameProcess = new GameProcess {
                Game = _gameService.ActiveGame,
                ProcessWatcher = gameProcessWatcher,
            };

            gameProcess.ProcessWatcher.Exited += (_, _) => {
                GameProcesses.Remove(gameProcess);
            };

            GameProcesses.Add(gameProcess);

            progress.Report(new(4, 1d));
        } catch (Exception ex) {
            progress.Report(new(-1, 1d, ex));
        }
    }
}