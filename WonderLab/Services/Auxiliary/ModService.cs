using AsyncImageLoader;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Provider;
using MinecraftLaunch.Extensions;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Tomlyn;
using Tomlyn.Model;
using WonderLab.Extensions;
using WonderLab.Services.Launch;
using WonderLab.Utilities;

namespace WonderLab.Services.Auxiliary;

public sealed partial class ModService {
    private readonly GameService _gameService;
    private readonly SettingService _settingService;
    private readonly ModrinthProvider _modrinthProvider;
    private readonly CurseforgeProvider _curseforgeProvider;

    private string _workingPath;
    private bool _isEnableIndependency;

    public ModService(
        GameService gameService,
        SettingService settingService,
        ModrinthProvider modrinthProvider,
        CurseforgeProvider curseforgeProvider) {
        _gameService = gameService;
        _settingService = settingService;
        _modrinthProvider = modrinthProvider;
        _curseforgeProvider = curseforgeProvider;
    }

    public void Init() {
        if (_gameService.TryGetMinecraftProfile(out var profile) && profile.IsEnableSpecificSetting)
            _isEnableIndependency = profile.IsEnableIndependency;
        else
            _isEnableIndependency = _settingService.Setting.IsEnableIndependency;

        _workingPath = _gameService.ActiveGameCache.ToWorkingPath(_isEnableIndependency);
    }

    public void ChangeExtension(Mod mod) {
        var originalPath = mod.Path;
        string dirPath = Path.GetDirectoryName(originalPath);
        string newName = Path.GetFileNameWithoutExtension(originalPath) + (mod.IsEnabled ? string.Empty : ".jar.disabled");

        mod.Path = Path.Combine(dirPath, newName);
        mod.IsEnabled = !mod.IsEnabled;
        File.Move(originalPath, mod.Path);
    }

    public async IAsyncEnumerable<Mod> LoadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken) {
        var modsPath = Path.Combine(_workingPath, "mods");
        Directory.CreateDirectory(modsPath);

        var modDatas = Directory.EnumerateFiles(modsPath)
            .OrderBy(x => x)
            .Where(x => x.EndsWith(".jar") || x.EndsWith(".jar.disabled"));

        foreach (var modData in modDatas) {
            cancellationToken.ThrowIfCancellationRequested();

            var mod = await Task.Run(() => ParseMod(modData), cancellationToken);
            if (mod is not null)
                yield return mod;
        }
    }

    public async Task CheckModsUpdateAsync(IEnumerable<Mod> mods, CancellationToken cancellationToken) {
        var sha1Tasks = mods.Select(async mod => {
            cancellationToken.ThrowIfCancellationRequested();
            var hash = await HashUtil.GetFileSha1HashAsync(mod.Path);
            return (Mod: mod, Hash: hash);
        }).ToList();

        var sha1List = await Task.WhenAll(sha1Tasks);
        var modToSha1 = sha1List.ToDictionary(x => x.Mod, x => x.Hash);
        var hashToMod = sha1List
            .GroupBy(x => x.Hash)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Mod).ToList());

        var minecraft = _gameService.ActiveGameCache;
        var loaderType = (minecraft as ModifiedMinecraftEntry)?.ModLoaders?.FirstOrDefault().Type ?? ModLoaderType.Any;

        var modrinthFiles = await _modrinthProvider.GetModFilesBySha1Async(
            [.. modToSha1.Values], minecraft.Version.VersionId, loaderType, HashType.SHA1, cancellationToken);

        var modrinthInfo = await _modrinthProvider.SearchByProjectIdsAsync(
            modrinthFiles.Select(x => x.Id), cancellationToken);

        var modrinthInfoDict = modrinthInfo.ToDictionary(x => x.Id);
        foreach (var file in modrinthFiles) {
            if (!hashToMod.TryGetValue(file.SourceHash, out var modsMatching)) continue;
            if (!modrinthInfoDict.TryGetValue(file.Id, out var info)) continue;

            foreach (var mod in modsMatching) {
                mod.DisplayName = info.Name;
                mod.Description = info.Summary ?? "dorodorodo?";
                mod.DownloadUrl = file.Files.FirstOrDefault()?.DownloadUrl;
                mod.CanUpdate = file.Files.Any(f => f.Sha1 != file.SourceHash);

                if (!string.IsNullOrEmpty(info.IconUrl))
                    _ = LoadModIconAsync(mod, info.IconUrl, cancellationToken);
            }
        }

        // 3. 筛选未匹配到的 mod，准备 CurseForge 检查
        var matchedSha1 = modrinthFiles.Select(x => x.SourceHash).ToHashSet();
        var cfMods = modToSha1
            .Where(kv => !matchedSha1.Contains(kv.Value))
            .Select(kv => kv.Key)
            .ToList();

        if (cfMods.Count == 0)
            return;

        var murmurTasks = cfMods.Select(async mod => {
            var hash = await HashUtil.GetFileMurmurHash2Async(mod.Path);
            return (Mod: mod, Hash: hash);
        }).ToList();

        var murmurHashesList = await Task.WhenAll(murmurTasks);
        var murmurHashes = murmurHashesList.ToDictionary(x => x.Hash, x => x.Mod);
        var files = await _curseforgeProvider.GetResourceFilesByFingerprintsAsync(murmurHashes.Keys.ToArray(), cancellationToken);
        var resources = await _curseforgeProvider.GetResourcesByModIdsAsync(files.Keys.Select(x => (long)x.ModId), cancellationToken);

        var modIdToLatestFiles = resources.SelectMany(r => r.LatestFiles ?? Enumerable.Empty<dynamic>())
            .GroupBy(f => f.ModId)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        var modIdToResource = resources.ToDictionary(x => x.Id);
        var fileDict = files.ToDictionary(
            kv => kv.Key,
            kv => modIdToLatestFiles.TryGetValue(kv.Key.ModId, out var latestFiles) ? latestFiles : kv.Value
        ).ToFrozenDictionary();

        foreach (var kv in fileDict) {
            var localFile = kv.Key;
            var lastRemoteMod = kv.Value.FirstOrDefault(x => x.MinecraftVersions.Contains(minecraft.Version.VersionId));

            if (!murmurHashes.TryGetValue(localFile.FileFingerprint, out var mod))
                continue;

            if (!modIdToResource.TryGetValue(kv.Key.ModId, out var modInfo))
                continue;

            mod.DisplayName = modInfo.Name;
            mod.Description = modInfo.Summary ?? "dorodorodo?";
            mod.DownloadUrl = lastRemoteMod?.DownloadUrl;
            mod.CanUpdate = kv.Key.Published < lastRemoteMod?.Published
                && !kv.Key.FileFingerprint.Equals(lastRemoteMod?.FileFingerprint);

            if (!string.IsNullOrEmpty(modInfo.IconUrl))
                _ = LoadModIconAsync(mod, modInfo.IconUrl, cancellationToken);
        }
    }

    private static Mod ParseMod(string path) {
        var mod = new Mod {
            Path = path,
            FileName = Path.GetFileName(path),
            IsEnabled = Path.GetExtension(path).Equals(".jar")
        };
        using var zipArchive = ZipFile.OpenRead(path);

        var quiltModJson = zipArchive.GetEntry("quilt.mod.json");
        var fabricModJson = zipArchive.GetEntry("fabric.mod.json");
        var forgeModsToml = zipArchive.GetEntry("META-INF/mods.toml");
        var neoForgeModsToml = zipArchive.GetEntry("META-INF/neoforge.mods.toml");
        var mcmodInfo = zipArchive.GetEntry("mcmod.info");

        if (quiltModJson != null)
            return ParseFabricMod(mod, quiltModJson.ReadAsString(), true);
        if (fabricModJson != null)
            return ParseFabricMod(mod, fabricModJson.ReadAsString(), false);

        if (mcmodInfo != null)
            return ParseForgeModByJson(mod, mcmodInfo.ReadAsString());

        if (neoForgeModsToml != null)
            return ParseForgeModByToml(mod, neoForgeModsToml.ReadAsString());

        if (forgeModsToml != null)
            return ParseForgeModByToml(mod, forgeModsToml.ReadAsString());

        mod.Description = "dorodorodo?";
        mod.DisplayName = Path.GetFileName(mod.Path);
        return mod;
    }

    private static Mod ParseForgeModByJson(Mod mod, string json) {
        try {
            var jsonNode = (json.Replace("\u000a", "") ?? "")
                .AsNode();

            var jsonArray = jsonNode?.AsArray()
                ?.FirstOrDefault();

            mod.DisplayName = jsonArray.GetString("name");
            mod.Version = jsonArray.GetString("version");
            mod.Description = jsonArray.GetString("description")
                ?.TrimEnd('\n')
                ?.TrimEnd('\r') ?? "dorodorodo?";

            mod.Authors = (jsonArray.Select("authorList") ?? jsonArray.Select("authors"))
                ?.GetEnumerable<string>();

            return mod;

        } catch (Exception) { }

        return null;
    }

    private static Mod ParseForgeModByToml(Mod mod, string toml) {
        var tomlTable = (Toml.ToModel(toml)["mods"] as TomlTableArray)
            ?.FirstOrDefault();

        mod.Version = tomlTable.GetString("version");
        mod.DisplayName = tomlTable.GetString("displayName");
        mod.Authors = tomlTable.GetString("authors")?.Split(",").Select(x => x.Trim(' ')).ToArray();
        mod.Description = tomlTable.GetString("description")
            ?.Trim()
            ?.Replace('\n', char.MinValue)
            ?.Replace('\r', char.MinValue);

        if (mod.Version == "${file.jarVersion}")
            mod.Version = null;

        if (string.IsNullOrEmpty(mod.Description))
            mod.Description = "dorodorodo?";

        return mod;
    }

    private static Mod ParseFabricMod(Mod mod, string json, bool isQuilt) {
        var jsonNode = json.FixJson().AsNode();

        if (isQuilt)
            jsonNode = jsonNode?.Select("quilt_loader");

        if (jsonNode is null)
            throw new InvalidDataException($"Invalid {nameof(json)}");

        mod.DisplayName = jsonNode.GetString("name");
        mod.Description = jsonNode.GetString("description")
            ?.TrimEnd('\n')
            ?.TrimEnd('\r') ?? "dorodorodo?";

        return mod;
    }

    private static Task LoadModIconAsync(Mod mod, string iconUrl, CancellationToken token) {
        return Task.Run(async () => {
            try {
                var icon = await ImageLoader.AsyncImageLoader.ProvideImageAsync(iconUrl);
                await Dispatcher.UIThread.InvokeAsync(() => mod.Icon = icon);
            } catch (Exception) { }
        }, token);
    }
}

public sealed partial class Mod : ObservableObject {
    [ObservableProperty] private string _path;
    [ObservableProperty] private string _version;
    [ObservableProperty] private string _fileName;
    [ObservableProperty] private string _displayName;
    [ObservableProperty] private string _description;
    [ObservableProperty] private Bitmap _icon = ThemeService.LoadingIcon.Value;

    public bool IsEnabled { get; set; }
    public bool CanUpdate { get; set; }
    public string DownloadUrl { get; set; }
    public IEnumerable<string> Authors { get; set; }
}