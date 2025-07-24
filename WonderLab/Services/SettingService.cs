using Microsoft.Extensions.Logging;
using MinecraftLaunch;
using MinecraftLaunch.Extensions;
using System;
using System.IO;
using WonderLab.Classes.Models;
using WonderLab.Override.I18n;
using WonderLab.Utilities;

namespace WonderLab.Services;

public sealed class SettingService {
    private readonly ILogger<SettingService> _logger;
    public static readonly FileInfo SettingFileInfo = new(Path.Combine(PathUtil.GetDataFolderPath(), "settings.json"));

    public SettingModel Setting { get; set; }

    public SettingService(ILogger<SettingService> logger) =>
        _logger = logger;

    public void Load() {
        _logger.LogInformation("读取设置项");

        try {
            var json = File.ReadAllText(SettingFileInfo.FullName);
            Setting = json.Deserialize(SettingModelJsonContext.Default.SettingModel);

            DownloadMirrorManager.MaxThread = Setting.MaxThread;
            DownloadMirrorManager.IsEnableMirror = Setting.IsEnableMirror;
            I18nExtension.LanguageCode = Setting.LanguageCode ??= "zh-Hans";
        } catch (Exception ex) {
            _logger.LogError(ex, "遭遇错误：{ex}", ex.ToString());
        }
    }

    public void Save() {
        _logger.LogInformation("保存设置项");

        try {
            File.WriteAllText(SettingFileInfo.FullName, (Setting ??= new()).Serialize(SettingModelJsonContext.Default.SettingModel));
        } catch (Exception ex) {
            _logger.LogError(ex, "遭遇错误：{ex}", ex.ToString());
        }
    }

    public void Initialize() {
        _logger.LogInformation("初始化设置项");

        try {
            if (!SettingFileInfo.Exists) {
                _logger.LogInformation("数据文件不存在或丢失");
                SettingFileInfo.Directory.Create();

                using var fs = SettingFileInfo.Create();
                fs.Close();
                Save();
            }

            Load();
        } catch (Exception ex) {
            _logger.LogError(ex, "遭遇错误：{ex}", ex.ToString());
        }
    }
}