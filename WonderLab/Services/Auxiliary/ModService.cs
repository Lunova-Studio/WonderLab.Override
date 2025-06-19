using AsyncImageLoader;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Components.Provider;
using MinecraftLaunch.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

public sealed class ModService {
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
        //Modrinth
        var sha1HashList = new List<(Mod Mod, string Hash)>();
        foreach (var mod in mods) {
            cancellationToken.ThrowIfCancellationRequested();
            var hash = await HashUtil.GetFileSha1HashAsync(mod.Path);
            sha1HashList.Add((mod, hash));
        }

        var modSha1HashDict = sha1HashList.ToDictionary(x => x.Hash, x => x.Mod);
        var mModfiles = await _modrinthProvider.GetModFilesBySha1Async([.. modSha1HashDict.Keys],
            _gameService.ActiveGameCache.Version.VersionId, HashType.SHA1, cancellationToken);

        var mModInfoDict = (await _modrinthProvider.SearchByProjectIdsAsync(
            mModfiles.Select(x => x.Id),
            cancellationToken)).ToDictionary(info => info.Id);

        foreach (var modfile in mModfiles) {
            if (!modSha1HashDict.TryGetValue(modfile.SourceHash, out var mod))
                continue;

            if (!mModInfoDict.TryGetValue(modfile.Id, out var modInfo))
                continue;

            mod.DisplayName = modInfo.Name;
            mod.Description = modInfo.Summary ?? "dorodorodo?";
            mod.DownloadUrl = modfile.Files.FirstOrDefault()?.DownloadUrl;
            mod.CanUpdate = modfile.Files.Any(f => !modfile.SourceHash.Equals(f.Sha1));

            if (!string.IsNullOrEmpty(modInfo.IconUrl)) {
                _ = Task.Run(async () => {
                    mod.Icon = await ImageLoader.AsyncImageLoader.ProvideImageAsync(modInfo.IconUrl);
                }, cancellationToken);
            }
        }

        var validHashes = mModfiles.Select(x => x.SourceHash)
            .ToHashSet();

        var cfMods = modSha1HashDict
            .Where(kv => !validHashes.Contains(kv.Key))
            .Select(kv => kv.Value);

        //Curseforge
        var murmur2HashList = new List<(Mod Mod, uint Hash)>();
        foreach (var mod in cfMods) {
            cancellationToken.ThrowIfCancellationRequested();
            var hash = await HashUtil.GetFileMurmurHash2Async(mod.Path);
            murmur2HashList.Add((mod, hash));
        }

        var modHash2Dict = murmur2HashList.ToDictionary(x => x.Hash, x => x.Mod);
        var cfModfiles = await _curseforgeProvider.GetResourceFilesByFingerprintsAsync([.. modHash2Dict.Keys],
            cancellationToken);

        if (!cfModfiles.Any())
            return;

        var cfModInfo = await _curseforgeProvider.GetResourcesByModIdsAsync(cfModfiles.Select(x => (long)x.ModId),
            cancellationToken);

        var cfModInfoDict = cfModInfo.ToDictionary(info => info.Id);
        foreach (var modfile in cfModfiles) {
            if (!modHash2Dict.TryGetValue(modfile.FileFingerprint, out var mod))
                continue;

            if (!cfModInfoDict.TryGetValue(modfile.ModId, out var modInfo))
                continue;

            mod.DisplayName = modInfo.Name;
            mod.Description = modInfo.Summary ?? "dorodorodo?";
            mod.DownloadUrl = modfile.DownloadUrl;
            var localHash = modHash2Dict.FirstOrDefault(kv => kv.Value == mod).Key;
            if (localHash != 0)
                mod.CanUpdate = !modfile.FileFingerprint.Equals(localHash);

            if (!string.IsNullOrEmpty(modInfo.IconUrl)) {
                _ = Task.Run(async () => {
                    mod.Icon = await ImageLoader.AsyncImageLoader.ProvideImageAsync(modInfo.IconUrl);
                }, cancellationToken);
            }
        }
    }

    private static Mod ParseMod(string path) {
        var mod = new Mod { Path = path, IsEnabled = Path.GetExtension(path).Equals(".jar") };
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
        var jsonNode = json.AsNode();

        if (isQuilt)
            jsonNode = jsonNode?.Select("quilt_loader")
                ?.Select("metadata");

        if (jsonNode is null)
            throw new InvalidDataException($"Invalid {nameof(json)}");

        mod.DisplayName = jsonNode.GetString("name");
        mod.Description = jsonNode.GetString("description")
            .TrimEnd('\n')
            .TrimEnd('\r') ?? "dorodorodo?";

        return mod;
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