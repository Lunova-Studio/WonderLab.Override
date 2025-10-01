using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Launch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WonderLab.Services.Auxiliary;
using WonderLab.ViewModels.Windows;

namespace WonderLab.Services.Launch;

public sealed partial class GameProcessService {
    private readonly SaveService _saveService;
    private readonly ILogger<GameProcessService> _logger;

    public ObservableCollection<MinecraftProcessModel> MinecraftProcesses { get; private set; }

    public GameProcessService(SaveService saveService, ILogger<GameProcessService> logger) {
        _logger = logger;
        _saveService = saveService;

        MinecraftProcesses = [];
    }

    public void AddProcess(MinecraftProcess process, MinecraftEntry minecraft) {
        var gameProcess = new MinecraftProcessModel {
            MinecraftProcess = process,
        };

        gameProcess.MinecraftProcess.Started += (_, _) =>
            gameProcess.IsStarted = true;

        gameProcess.MinecraftProcess.Exited += async (_, _) => {
            gameProcess.IsExited = true;

            await Task.Delay(1000);

            MinecraftProcesses.Remove(gameProcess);
            _logger.LogInformation("游戏进程已退出，剩余进程数：{count}", MinecraftProcesses.Count);
        };

        //同时处理服务器加入检测
        gameProcess.MinecraftProcess.OutputLogReceived += (_, args) => {
            gameProcess.GameLogs.Add(args.Data);

            var matchResult = ServerConnectHandleRegex().Match(args.Data.Log);
            if (matchResult.Success)
                _saveService.SaveLastPlayedTime(matchResult.Groups[1].Value, minecraft.Id,
                    minecraft.MinecraftFolderPath, DateTime.Now);
        };

        MinecraftProcesses.Add(gameProcess);
    }

    [GeneratedRegex("Connecting to\\s+([^\\s,]+)")]
    private partial Regex ServerConnectHandleRegex();
}

public sealed partial class MinecraftProcessModel : ObservableObject {
    [ObservableProperty] private bool _isExited;
    [ObservableProperty] private bool _isStarted;

    public MinecraftProcess MinecraftProcess { get; init; }
    public IList<MinecraftLogEntry> GameLogs { get; init; } = [];

    [RelayCommand]
    private void Close() => MinecraftProcess.Close();

    [RelayCommand]
    private void OpenLogWindow() {
        var window = App.Get<MinecraftLogWindow>();
        var windowViewModel = App.Get<MinecraftLogWindowViewModel>();

        windowViewModel.MinecraftProcess = this;
        window.DataContext = windowViewModel;
        window.Show();
    }
}