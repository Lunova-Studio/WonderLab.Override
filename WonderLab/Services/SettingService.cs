using Microsoft.Extensions.Logging;
using MinecraftLaunch;
using MinecraftLaunch.Extensions;
using System;
using System.IO;
using WonderLab.Classes.Models;
using WonderLab.Utilities;

namespace WonderLab.Services;

public sealed class SettingService {
    private readonly ILogger<SettingService> _logger;
    private readonly FileInfo _settingFileInfo = new(Path.Combine(PathUtil.GetDataFolderPath(), "settings.json"));

    public SettingModel Setting { get; set; }

    public SettingService(ILogger<SettingService> logger) =>
        _logger = logger;

    public void Load() {
        _logger.LogInformation("读取设置项");

        try {
            var json = File.ReadAllText(_settingFileInfo.FullName);
            Setting = json.Deserialize(SettingModelJsonContext.Default.SettingModel);

            DownloadMirrorManager.MaxThread = Setting.MaxThread;
            DownloadMirrorManager.IsEnableMirror = Setting.IsEnableMirror;
        } catch (Exception ex) {
            _logger.LogError(ex, "遭遇错误：{ex}", ex.ToString());
        }
    }

    public void Save() {
        _logger.LogInformation("保存设置项");

        try {
            File.WriteAllText(_settingFileInfo.FullName, (Setting ??= new()).Serialize(SettingModelJsonContext.Default.SettingModel));
        } catch (Exception ex) {
            _logger.LogError(ex, "遭遇错误：{ex}", ex.ToString());
        }
    }

    public void Initialize() {
        _logger.LogInformation("初始化设置项");

        try {
            if (!_settingFileInfo.Exists) {
                _logger.LogInformation("数据文件不存在或丢失");
                _settingFileInfo.Directory.Create();

                using var fs = _settingFileInfo.Create();
                fs.Close();
                Save();
            }

            Load();
        } catch (Exception ex) {
            _logger.LogError(ex, "遭遇错误：{ex}", ex.ToString());
        }
    }
}