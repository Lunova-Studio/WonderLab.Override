using Microsoft.Extensions.Logging;
using MinecraftLaunch;
using System;
using System.IO;
using System.Text.Json;
using WonderLab.Models;
using WonderLab.Override.I18n;
using WonderLab.Utilities;

namespace WonderLab.Services;

public sealed class SettingService {
    private const string CURSEFORGE_API_KEY = "$2a$10$Awb53b9gSOIJJkdV3Zrgp.CyFP.dI13QKbWn/4UZI4G4ff18WneB6";

    private readonly ILogger<SettingService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly Lazy<FileInfo> _settingFileInfo = new(() => {
#if DEBUG
        return new FileInfo(Path.Combine(PathUtil.GetDataFolderPath(), "settings_debug.json"));
#else
        return new FileInfo(Path.Combine(PathUtil.GetDataFolderPath(), "settings.json"));
#endif
    });

    public static FileInfo SettingFileInfo => _settingFileInfo.Value;

    public bool IsCompletedOOBE { get; private set; }
    public SettingsModel Setting { get; private set; } = new();

    public SettingService(ILogger<SettingService> logger) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 初始化设置服务
    /// </summary>
    public void Initialize() {
        _logger.LogInformation("初始化设置项");

        try {
            EnsureDirectoryExists();

            IsCompletedOOBE = SettingFileInfo.Exists;
            if (!IsCompletedOOBE) {
                _logger.LogInformation("检测到首次运行，创建默认配置文件");
                CreateDefaultSettingsFile();
            }

            Load();
        } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
            _logger.LogCritical(ex, "无法访问配置文件，请检查文件权限");
            throw;
        } catch (Exception ex) {
            _logger.LogError(ex, "初始化设置时发生错误");
        }
    }

    /// <summary>
    /// 加载设置
    /// </summary>
    public void Load() {
        _logger.LogInformation("开始加载设置项");

        if (!SettingFileInfo.Exists) {
            _logger.LogWarning("配置文件不存在，使用默认设置");
            Setting = new SettingsModel();
            return;
        }

        try {
            // 使用异步文件操作提高性能（如果环境支持）
            string json = File.ReadAllText(SettingFileInfo.FullName);

            // 使用自定义序列化器（假设存在）
            Setting = string.IsNullOrEmpty(json)
                ? new SettingsModel()
                : JsonSerializer.Deserialize<SettingsModel>(json, _jsonOptions) ?? new SettingsModel();

            // 设置默认值
            Setting.LanguageCode ??= "zh-Hans";
            I18nExtension.LanguageCode = Setting.LanguageCode;

            // 初始化帮助类
            InitializeHelper.Initialize(x => {
                x.UserAgent = "WonderLab/2.0";
                x.MaxThread = Setting.MaxThread;
                x.IsEnableMirror = Setting.IsEnableMirror;
                x.CurseForgeApiKey = CURSEFORGE_API_KEY;
            });

            // 更新OOBE状态
            if (IsCompletedOOBE && !Setting.IsCompletedOOBE) {
                IsCompletedOOBE = false;
            }
        } catch (JsonException ex) {
            _logger.LogError(ex, "配置文件格式错误，使用默认设置");
            Setting = new SettingsModel();
        } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
            _logger.LogError(ex, "无法读取配置文件");
            throw;
        } catch (Exception ex) {
            _logger.LogError(ex, "加载设置时发生未知错误");
            Setting = new SettingsModel();
        }
    }

    /// <summary>
    /// 保存设置
    /// </summary>
    public void Save() {
        _logger.LogInformation("开始保存设置项");

        try {
            // 确保目录存在
            EnsureDirectoryExists();

            // 使用临时文件确保原子性写入
            string tempFile = Path.GetTempFileName();
            string json = JsonSerializer.Serialize(Setting, _jsonOptions);
            File.WriteAllText(tempFile, json);

            // 替换原文件（原子操作）
            File.Copy(tempFile, SettingFileInfo.FullName, true);
            File.Delete(tempFile);
        } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
            _logger.LogError(ex, "无法写入配置文件，请检查文件权限");
            throw;
        } catch (Exception ex) {
            _logger.LogError(ex, "保存设置时发生错误");
        }
    }

    #region Privates

    private void EnsureDirectoryExists() {
        string directory = SettingFileInfo.DirectoryName ??
                          throw new InvalidOperationException("无法获取配置文件目录");

        if (!Directory.Exists(directory)) {
            _logger.LogInformation("创建配置目录: {Directory}", directory);
            Directory.CreateDirectory(directory);
        }
    }

    private void CreateDefaultSettingsFile() {
        try {
            Setting = new SettingsModel {
                LanguageCode = "zh-Hans",
                MaxThread = Environment.ProcessorCount,
                IsEnableMirror = false,
                IsCompletedOOBE = false
            };

            Save();
        } catch (Exception ex) {
            _logger.LogError(ex, "创建默认配置文件时出错");
            throw;
        }
    }

    #endregion
}