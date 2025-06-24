using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Nodes;
using WonderLab.Services.Launch;

namespace WonderLab.Services.Auxiliary;

public sealed class ShaderpackService {
    private readonly GameService _gameService;
    private readonly SettingService _settingService;

    private string _configPath;
    private string _workingPath;

    private bool _isEnableIndependency;

    private ConfigNode _configNode;

    public bool IsIris { get; set; }
    public ObservableCollection<Shaderpack> Shaderpacks { get; } = [];

    public bool IsEnableShaders => _configNode.GetValue<bool>("enableShaders");

    public ShaderpackService(GameService gameService, SettingService settingService) {
        _gameService = gameService;
        _settingService = settingService;
    }

    public void Init() {
        if (_gameService.TryGetMinecraftProfile(out var profile) && profile.IsEnableSpecificSetting)
            _isEnableIndependency = profile.IsEnableIndependency;
        else
            _isEnableIndependency = _settingService.Setting.IsEnableIndependency;

        var minecraft = _gameService.ActiveGameCache;

        _workingPath = minecraft.ToWorkingPath(_isEnableIndependency);
        IsIris = !(minecraft as ModifiedMinecraftEntry)?.ModLoaders
            ?.Any(x => x.Type is ModLoaderType.OptiFine) ?? false;

        _configPath = IsIris
            ? Path.Combine(_workingPath, "config", "iris.properties")
            : Path.Combine(_workingPath, "optionsshaders.txt");
    }

    public void ChangeEnableStatus(bool isEnable) {
        _configNode["enableShaders"] = isEnable;
    }

    public async Task LoadAllAsync(CancellationToken cancellationToken) {
        Shaderpacks.Clear();

        var packs = new List<Shaderpack>();
        if (!File.Exists(_configPath))
            await File.WriteAllTextAsync(_configPath, "shaderPack=", cancellationToken);

        var configStr = await File.ReadAllTextAsync(_configPath, cancellationToken);
        _configNode = ConfigNode.Parse(configStr);

        var shaderPackId = _configNode.GetValue<string>("shaderPack");
        var shaderPacksPath = Path.Combine(_workingPath, "shaderpacks");
        Directory.CreateDirectory(shaderPacksPath);

        var entries = Directory.EnumerateFiles(shaderPacksPath);
        foreach (var path in entries) {
            var fileName = Path.GetFileName(path);
            var pack = new Shaderpack(path, fileName) {
                IsEnabled = fileName.Equals(shaderPackId)
            };

            packs.Add(pack);
        }

        // 启用的在前，未启用的按文件名排序
        var shaderpacks = packs.OrderByDescending(p => p.IsEnabled)
            .ThenBy(p => p.FileName, StringComparer.OrdinalIgnoreCase);

        foreach (var item in shaderpacks)
            Shaderpacks.Add(item);
    }

    public async Task SaveToConfigAsync(CancellationToken cancellationToken) {
        if (_configNode is null)
            return;

        _configNode["shaderPack"] = Shaderpacks
            ?.FirstOrDefault(x => x.IsEnabled)?.FileName ?? string.Empty;

        var options = _configNode.ToString();
        await File.WriteAllTextAsync(_configPath, options, cancellationToken);
    }
}

public record Shaderpack(string Path, string FileName) {
    public bool IsEnabled { get; set; }
}