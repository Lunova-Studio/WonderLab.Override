using System;
using System.IO;
using System.Threading;
using WonderLab.Services.UI;
using System.Threading.Tasks;
using WonderLab.Classes.Datas;
using MinecraftLaunch.Utilities;
using MinecraftLaunch.Extensions;
using Microsoft.Extensions.Hosting;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.ApplicationInsights;
using Avalonia.Platform;
using System.IO.Compression;

namespace WonderLab.Services;

/// <summary>
/// 设置项服务类
/// </summary>
/// <remarks>
/// 用于管理启动器产生的持久化数据
/// </remarks>
public sealed class SettingService {
    public static bool IsInitialize { get; } = GetIsInitialized();
    public SettingData Data { get; private set; }
    
    public SettingService(WeakReferenceMessenger weakReferenceMessenger) {
        weakReferenceMessenger.Register<SettingDataChangedMessage>(this, (_, args) => {
            Data = args.Data;
        });
    }

    private static bool GetIsInitialized() {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        FileInfo path = new(Path.Combine(documentsPath, "Blessing-Studio", "wonderlab", "settingData.json"));
        return !path.Exists;
    }
}

internal sealed class SettingBackgroundService : BackgroundService {
    private SettingData _settingData;

    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<SettingBackgroundService> _logger;

    private readonly ThemeService _themeService;
    private readonly WindowService _windowService;
    private readonly LanguageService _languageService;

    private readonly Dispatcher _dispatcher;
    private readonly FileInfo _settingDataFilePath;
    private readonly DirectoryInfo _backendDirectory;
    private readonly WeakReferenceMessenger _weakReferenceMessenger;

    public SettingBackgroundService(
        Dispatcher dispatcher,
        ThemeService themeService,
        WindowService windowService,
        //TelemetryClient telemetryClient,
        LanguageService languageService,
        ILogger<SettingBackgroundService> logger,
        WeakReferenceMessenger weakReferenceMessenger) {
        _logger = logger;
        _themeService = themeService;
        _windowService = windowService;
        _languageService = languageService;

        _dispatcher = dispatcher;
        //_telemetryClient = telemetryClient;
        _weakReferenceMessenger = weakReferenceMessenger;

        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _settingDataFilePath = new(Path
            .Combine(documentsPath, "Blessing-Studio", "wonderlab", "settingData.json"));

        _backendDirectory = new(Path
            .Combine(_settingDataFilePath.Directory.FullName, "backend"));
    }

    private void Save() {
        //_telemetryClient.Flush();
        var json = _settingData.Serialize(typeof(SettingData), new SettingDataContext(JsonConverterUtil.DefaultJsonOptions));
        File.WriteAllText(_settingDataFilePath.FullName, json);
    }

    private void Initialize() {
        _logger.LogInformation("开始初始化设置数据服务");

        if (!_settingDataFilePath.Directory!.Exists) {
            _settingDataFilePath.Directory.Create();
        }

        if (!_backendDirectory.Exists) {
            _backendDirectory.Create();

            using var stream = AssetLoader.Open(new Uri($"resm:WonderLab.Assets.WonderLab.Backend"));
            ZipFile.ExtractToDirectory(stream, _backendDirectory.FullName);
        }

        if (_settingDataFilePath.Exists) {
            try {
                _settingData = File.ReadAllText(_settingDataFilePath.FullName).AsJsonEntry<SettingData>();
            } catch (JsonException) {
                _logger.LogError("Json 序列化时出现故障，开始重置设置");
                _settingData = new();
                Save();
            }
        } else {
            _settingData = new();
            Save();
        }

        //_logger.LogInformation("是否需要进入 OOBE：{IsOOBE}", _isInitialize);

        _weakReferenceMessenger.Send(new SettingDataChangedMessage(_settingData));
        _dispatcher.Post(ViewDataInitialize);
        
    }

    private void ViewDataInitialize() {
        if (SettingService.IsInitialize) {
            _languageService.SetLanguage(0);
            _themeService.SetCurrentTheme(3);
            return;
        }

        _themeService.SetCurrentTheme(_settingData.ThemeIndex);
        _languageService.SetLanguage(_settingData.LanguageIndex);
    }

    public override Task StopAsync(CancellationToken cancellationToken) {
        Save();
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await Task.Run(Initialize, stoppingToken);
    }
}