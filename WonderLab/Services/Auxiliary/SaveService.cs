using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Extensions;
using NbtToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using WonderLab.Services.Launch;
using WonderLab.Utilities;

namespace WonderLab.Services.Auxiliary;

public sealed class SaveService {
    private readonly GameService _gameService;
    private readonly SettingService _settingService;
    private readonly FileInfo _serversFileInfo = new(Path.Combine(PathUtil.GetDataFolderPath(), "servers.json"));

    public ObservableCollection<SaveModel> Saves { get; } = [];
    public List<MultiPlayerTimeModel> MultiPlayerSaveTimes { get; private set; } = [];

    public SaveService(SettingService settingService, GameService gameService) {
        _gameService = gameService;
        _settingService = settingService;
    }

    public void SaveLastPlayedTime(string address, string minecraftId, string minecraftFolder, DateTime time) {
        var model = MultiPlayerSaveTimes.FirstOrDefault(x =>
            x.IPAddress == address &&
            x.MinecraftId == minecraftId &&
            x.MinecraftFolder == minecraftFolder);

        if (model != null) {
            model.LastPlayedTime = time;
        } else {
            MultiPlayerSaveTimes.Add(new MultiPlayerTimeModel {
                IPAddress = address,
                MinecraftId = minecraftId,
                MinecraftFolder = minecraftFolder,
                LastPlayedTime = time
            });
        }

        File.WriteAllText(_serversFileInfo.FullName,
            MultiPlayerSaveTimes.Serialize(MultiPlayerTimeModelJsonContext.Default.IEnumerableMultiPlayerTimeModel));
    }

    public async Task RefreshSavesAsync(CancellationToken cancellationToken = default) {
        var workingFolders = _gameService.Minecrafts
            .Select(x => (Path.Combine(x.ToWorkingPath(_settingService.Setting.IsEnableIndependency)), x.Id, x.MinecraftFolderPath));

        var minecrafts = _gameService.Minecrafts;
        var serverPaths = workingFolders.Select(x => (Path.Combine(x.Item1, "servers.dat"), x.Id, x.MinecraftFolderPath));

        var saves = await GetSinglePlayerEntrysAsync(minecrafts, cancellationToken)
            .ToListAsync(cancellationToken: cancellationToken);

        InitializeTime();

        var servers = GetMultiPlayerEntrys(serverPaths);
        var allSaves = saves.Union(servers);

        Saves.Clear();
        foreach (var save in allSaves.OrderByDescending(x => x.LastPlayedTime).Take(5))
            Saves.Add(save);
    }

    private void InitializeTime() {
        try {
            if (!_serversFileInfo.Exists)
                File.WriteAllText(_serversFileInfo.FullName,
                    new List<MultiPlayerTimeModel>().Serialize(MultiPlayerTimeModelJsonContext.Default.IEnumerableMultiPlayerTimeModel));

            var json = File.ReadAllText(_serversFileInfo.FullName);
            MultiPlayerSaveTimes = json
                .Deserialize(MultiPlayerTimeModelJsonContext.Default.IEnumerableMultiPlayerTimeModel)?
                .ToList() ?? [];
        } catch (JsonException) {
            MultiPlayerSaveTimes = [];
            File.WriteAllText(_serversFileInfo.FullName,
                MultiPlayerSaveTimes.Serialize(MultiPlayerTimeModelJsonContext.Default.IEnumerableMultiPlayerTimeModel));

        }
    }

    private IEnumerable<SaveModel> GetMultiPlayerEntrys(IEnumerable<(string path, string id, string folder)> servers) {
        foreach (var (path, id, folder) in servers) {
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
                continue;

            var rootTag = fileInfo.FullName.GetNBTParser().GetReader().ReadRootTag();
            var entries = rootTag["servers"]?.AsTagList<TagCompound>();
            if (entries is { Count: <= 0 })
                continue;

            foreach (var x in entries) {
                var model = new MultiPlayerSaveModel {
                    MinecraftId = id,
                    Name = x["name"]?.AsString(),
                    IPAddress = x["ip"]?.AsString(),
                    Icon = x.ContainsKey("icon") ? x["icon"]?.AsString() : string.Empty,
                    MinecraftFolder = folder,
                    LastPlayedTime = fileInfo.LastWriteTime,
                };

                if (TryGetLastPlayedTime(model.IPAddress, id, folder, MultiPlayerSaveTimes, out var time))
                    model.LastPlayedTime = time ?? DateTime.MinValue;

                yield return model;
            }
        }
    }

    private async IAsyncEnumerable<SaveModel> GetSinglePlayerEntrysAsync(
        IEnumerable<MinecraftEntry> minecrafts,
        [EnumeratorCancellation] CancellationToken cancellationToken) {
        foreach (var entry in minecrafts) {
            var saveFolder = new DirectoryInfo(Path.Combine(entry.ToWorkingPath(_settingService.Setting.IsEnableIndependency), "saves"));
            if (!saveFolder.Exists)
                continue;

            var nbtParser = entry.GetNBTParser();
            foreach (var save in saveFolder.EnumerateDirectories()) {
                var metaData = await nbtParser.ParseSaveAsync(save.Name, _settingService.Setting.IsEnableIndependency, cancellationToken);

                yield return new SinglePlayerSaveModel {
                    MetaData = metaData,
                    MinecraftId = entry.Id,
                    IconPath = metaData.IconFilePath
                };
            }
        }
    }

    private static bool TryGetLastPlayedTime(
        string address,
        string minecraftId,
        string minecraftFolder,
        IList<MultiPlayerTimeModel> times,
        out DateTime? time) {
        var timeModel = times.FirstOrDefault(x =>
            x.IPAddress == address &&
            x.MinecraftId == minecraftId &&
            x.MinecraftFolder == minecraftFolder);

        time = timeModel?.LastPlayedTime;
        return timeModel != null;
    }
}

//由于私募 bugjang 不提供多人游戏的上次游玩时间数据，需要手动保存
public record MultiPlayerTimeModel {
    [JsonPropertyName("ipAddress")] public string IPAddress { get; init; }
    [JsonPropertyName("minecraftId")] public string MinecraftId { get; init; }
    [JsonPropertyName("lastPlayedTime")] public DateTime LastPlayedTime { get; set; }
    [JsonPropertyName("minecraftFolder")] public string MinecraftFolder { get; init; }
}

[JsonSerializable(typeof(IEnumerable<MultiPlayerTimeModel>))]
public sealed partial class MultiPlayerTimeModelJsonContext : JsonSerializerContext;