using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Parser;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using WonderLab.Classes.Models;

namespace WonderLab.Services.Launch;

public sealed class GameService {
    private readonly ILogger<GameService> _logger;
    private readonly SettingService _settingService;

    public MinecraftEntry ActiveGameCache { get; set; }
    public MinecraftParser MinecraftParser { get; set; }
    public MinecraftEntry ActiveGame { get; private set; }
    public ObservableCollection<MinecraftEntry> Minecrafts { get; private set; }

    public GameService(ILogger<GameService> logger, SettingService settingService) {
        _logger = logger;
        _settingService = settingService;

        Minecrafts = [];
        _logger.LogInformation("初始化 {name}", nameof(GameService));
    }

    public void RefreshGames() {
        _logger.LogInformation("刷新游戏列表，指定的文件夹路径为：{folder}",
            _settingService.Setting?.ActiveMinecraftFolder);
        Minecrafts.Clear();

        if (MinecraftParser is null && string.IsNullOrEmpty(_settingService.Setting?.ActiveMinecraftFolder))
            return;

        MinecraftParser ??= _settingService.Setting.ActiveMinecraftFolder;
        foreach (var minecraft in MinecraftParser.GetMinecrafts())
            Minecrafts.Add(minecraft);

        if (Minecrafts.Count == 0)
            return;

        var entry = _settingService.Setting.ActiveGameId is not null
            ? Minecrafts.FirstOrDefault(x => x.Id == _settingService.Setting.ActiveGameId)
            : Minecrafts.FirstOrDefault();

        ActivateMinecraft(entry);
    }

    public void ActivateMinecraftFolder(string dir) {
        _logger.LogInformation("选择 .minecraft 目录：{folder}", dir);

        if (!_settingService.Setting.MinecraftFolders.Contains(dir))
            return;

        MinecraftParser = _settingService.Setting.ActiveMinecraftFolder = dir;
    }

    public void ActivateMinecraft(string id) {
        var minecraft = Minecrafts.FirstOrDefault(x => x.Id == id);
        if (string.IsNullOrWhiteSpace(id) || minecraft is null)
            return;

        ActivateMinecraft(minecraft);
    }

    public void ActivateMinecraft(MinecraftEntry entry) {
        _logger.LogInformation("选择游戏实例：{entry} - {isVanilla}", entry?.Id, entry?.IsVanilla);

        if (entry != null && !Minecrafts.Contains(entry))
            throw new ArgumentException("The specified minecraft entry does not exist.");

        ActiveGame = entry;
        _settingService.Setting.ActiveGameId = entry?.Id;
        WeakReferenceMessenger.Default.Send(new ActiveMinecraftChangedMessage());
    }

    public bool TryGetMinecraftProfile(out SpecificSettingModel profile) {
        var datas = MinecraftParser.DataProcessors.FirstOrDefault().Datas
            .ToDictionary(x => x.Key, x1 => x1.Value as SpecificSettingModel);

        return datas.TryGetValue(ActiveGameCache.Id, out profile);
    }
}

internal record ActiveMinecraftChangedMessage;