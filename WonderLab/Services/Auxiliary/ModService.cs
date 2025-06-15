using Avalonia.Media;
using Avalonia.Media.Imaging;
using MinecraftLaunch.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Tomlyn;
using Tomlyn.Model;
using WonderLab.Extensions;
using WonderLab.Services.Launch;

namespace WonderLab.Services.Auxiliary;

public sealed class ModService {
    private readonly GameService _gameService;
    private readonly SettingService _settingService;

    private string _workingPath;
    private bool _isEnableIndependency;

    public ObservableCollection<Mod> Mods { get; } = [];

    public ModService(GameService gameService, SettingService settingService) {
        _gameService = gameService;
        _settingService = settingService;
    }

    public void Init() {
        if (_gameService.TryGetMinecraftProfile(out var profile) && profile.IsEnableSpecificSetting)
            _isEnableIndependency = profile.IsEnableIndependency;
        else
            _isEnableIndependency = _settingService.Setting.IsEnableIndependency;

        _workingPath = _gameService.ActiveGameCache.ToWorkingPath(_isEnableIndependency);
    }

    public void LoadAll() {
        Mods.Clear();

        var modsPath = Path.Combine(_workingPath, "mods");
        Directory.CreateDirectory(modsPath);

        var modDatas = Directory.EnumerateFiles(modsPath)
            .Where(x => x.EndsWith(".jar") || x.EndsWith(".jar.disabled"));

        foreach (var mod in modDatas.Select(ParseMod))
            Mods.Add(mod);
    }

    public void ChangeExtension(Mod mod) {
        var originalPath = mod.Path;
        string dirPath = Path.GetDirectoryName(originalPath);
        string newName = Path.GetFileNameWithoutExtension(originalPath) + (mod.IsEnabled ? string.Empty : ".jar.disabled");

        mod.Path = Path.Combine(dirPath, newName);
        mod.IsEnabled = !mod.IsEnabled;
        File.Move(originalPath, mod.Path);
    }

    private Mod ParseMod(string path) {
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

        throw new InvalidDataException();
    }

    private static Mod ParseForgeModByJson(Mod mod, string json) {
        var jsonNode = (json.Replace("\u000a", "") ?? "").AsNode()
            .GetEnumerable()
            .FirstOrDefault();

        mod.DisplayName = jsonNode.GetString("name");
        mod.Version = jsonNode.GetString("version");
        mod.Description = jsonNode.GetString("description")
            .TrimEnd('\n')
            .TrimEnd('\r');

        mod.Authors = (jsonNode.Select("authorList") ?? jsonNode.Select("authors"))
            ?.GetEnumerable<string>();

        return mod;
    }

    private static Mod ParseForgeModByToml(Mod mod, string toml) {
        var tomlTable = ((Toml.ToModel(toml)["mods"] as TomlTableArray)
            ?.FirstOrDefault());

        mod.Version = tomlTable.GetString("version");
        mod.DisplayName = tomlTable.GetString("displayName");
        mod.Description = tomlTable.GetString("description")?.TrimEnd('\n').TrimEnd('\r');
        mod.Authors = tomlTable.GetString("authors")?.Split(",").Select(x => x.Trim(' ')).ToArray();

        if (mod.Version == "${file.jarVersion}")
            mod.Version = null;

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
            .TrimEnd('\r');

        mod.Authors = jsonNode.GetEnumerable<string>("authors");
        return mod;
    }
}

public record Mod {
    public int Format { get; set; }

    public Bitmap Icon { get; set; }

    public string Path { get; set; }
    public string Version { get; set; }
    public string FileName { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }

    public bool IsEnabled { get; set; }

    public IEnumerable<string> Authors { get; set; }
}