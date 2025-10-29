using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Base.Interfaces;
using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Components.Installer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using WonderLab.Classes.Models.Messaging;
using WonderLab.ViewModels.Tasks;

namespace WonderLab.Services.Download;

public sealed class DownloadService {
    private readonly TaskService _taskService;
    private readonly SettingService _settingService;

    public DownloadService(TaskService taskService, SettingService settingService) {
        _taskService = taskService;
        _settingService = settingService;
    }

    public Task<InstallMinecraftTaskViewModel> InstallMinecraftTaskAsync(IEnumerable<IInstallEntry> entries, string customId) {
        InstallMinecraftTaskViewModel task = new() {
            JobName = $"游戏 {customId} 的安装任务" // TODO: I18n
        };

        _taskService.QueueJob(task);

        WeakReferenceMessenger.Default.Send(new NotificationMessage($"已将游戏 {customId} 的安装任务添加至任务队列",
            NotificationType.Information));

        Task.Run(() => InstallMinecraftAsync(entries, customId, task));

        return Task.FromResult(task);

    }

    private async Task InstallMinecraftAsync(IEnumerable<IInstallEntry> entries, string customId, InstallMinecraftTaskViewModel progress) {
        var cancellationToken = progress.TaskCancellationToken;

        try {
            progress.Report(new(0d, TaskStatus.Created));

            var installer = CompositeInstaller.Create(entries, _settingService.Setting.ActiveMinecraftFolder,
                _settingService.Setting.ActiveJava?.JavaPath, customId);

            installer.ProgressChanged += (_, e) => {
                if (e.StepName is InstallStep.Interrupted) {
                    Debug.WriteLine("Fuck you");
                }
                Debug.WriteLine($"{(e.PrimaryStepName is InstallStep.Undefined ? "" : $"{e.PrimaryStepName} - ")}{e.StepName} - {e.FinishedStepTaskCount}/{e.TotalStepTaskCount} - {(e.IsStepSupportSpeed ? $"{DefaultDownloader.FormatSize(e.Speed, true)} - {e.Progress * 100:0.00}%" : $"{e.Progress * 100:0.00}%")}");
                progress.Report(new TaskProgress {
                    Speed = e.Speed,
                    TaskStatus = TaskStatus.Running,
                    IsSupportSpeed = e.IsStepSupportSpeed,
                    Progress = e.Progress,
                }, e.TotalStepTaskCount, e.FinishedStepTaskCount);
            };

            installer.Completed += (_, args) => {
                if (args.IsSuccessful) {
                    WeakReferenceMessenger.Default.Send(new NotificationMessage($"游戏 {customId} 安装完成", NotificationType.Success));
                } else {
                    WeakReferenceMessenger.Default.Send(new NotificationMessage($"游戏 {customId} 安装时遭遇了意料之外的异常：\n{args.Exception}", NotificationType.Error));
                }
            };

            await installer.InstallAsync(cancellationToken);
            progress.ReportCompleted();
        } catch (TaskCanceledException) {
            progress.Report(new(1d, TaskStatus.Canceled));
            WeakReferenceMessenger.Default.Send(new NotificationMessage($"已中断游戏 {customId} 的安装任务", NotificationType.Information));
        } catch (Exception ex) {
            progress.Report(new(1d, TaskStatus.Faulted));
            WeakReferenceMessenger.Default.Send(new NotificationMessage($"游戏 {customId} 安装时遭遇了意料之外的异常：\n{ex}", NotificationType.Error));
        }
    }
}