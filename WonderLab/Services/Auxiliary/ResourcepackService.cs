using Avalonia.Media.Imaging;
using MinecraftLaunch.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Nodes;
using WonderLab.Services.Launch;

namespace WonderLab.Services.Auxiliary;

public sealed partial class ResourcepackService {
    private readonly GameService _gameService;
    private readonly SettingService _settingService;

    private string _workingPath;
    private bool _isEnableIndependency;
    private OptionsNode _optionsNode;

    public ObservableCollection<Resourcepack> Resourcepacks { get; } = [];

    public ResourcepackService(GameService gameService, SettingService settingService) {
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

    public async Task LoadAllAsync(CancellationToken cancellationToken) {
        Resourcepacks.Clear();

        var optionsPath = Path.Combine(_workingPath, "options.txt");
        var packs = new List<Resourcepack>();
        if (!File.Exists(optionsPath))
            await File.WriteAllTextAsync(optionsPath, "resourcePacks:[]", cancellationToken);

        var optionsStr = await File.ReadAllTextAsync(optionsPath, cancellationToken);
        _optionsNode = OptionsNode.Parse(optionsStr);

        var resourcePacksId = _optionsNode.GetValue<IEnumerable<string>>("resourcePacks") ?? [];
        var resourcePacksPath = Path.Combine(_workingPath, "resourcepacks");
        Directory.CreateDirectory(resourcePacksPath);

        var entries = Directory.EnumerateFileSystemEntries(resourcePacksPath);

        foreach (var path in entries) {
            var pack = await ParseResourcepackAsync(path, resourcePacksId, cancellationToken);

            if (pack != null)
                packs.Add(pack);
        }

        // 启用的在前，未启用的按文件名排序
        var resourcepacks = packs.OrderByDescending(p => p.IsEnabled)
            .ThenBy(p => p.FileName, StringComparer.OrdinalIgnoreCase);

        foreach (var item in resourcepacks)
            Resourcepacks.Add(item);
    }

    public async Task SaveToOptionsAsync(CancellationToken cancellationToken) {
        if (_optionsNode is null)
            return;

        _optionsNode["resourcePacks"] = Resourcepacks?.Where(x => x.IsEnabled)
            .Select(x => x.FileName);

        var optionsPath = Path.Combine(_workingPath, "options.txt");
        var options = _optionsNode.ToString();

        await File.WriteAllTextAsync(optionsPath, options, cancellationToken);
    }

    private static async Task<Resourcepack> ParseResourcepackAsync(string path, IEnumerable<string> enabledPacksIds, CancellationToken cancellationToken) {
        string id = Path.GetFileName(path);
        bool isZip = path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

        JsonNode jsonNode = null;
        using var iconStream = new MemoryStream();

        if (isZip) {
            using var archive = ZipFile.OpenRead(path);
            var infoEntry = archive.GetEntry("pack.mcmeta");
            if (infoEntry == null)
                return null;

            using (var reader = new StreamReader(infoEntry.Open()))
                jsonNode = (await reader.ReadToEndAsync(cancellationToken)).AsNode();

            var imgEntry = archive.GetEntry("pack.png");
            if (imgEntry != null) {
                using var imgStream = imgEntry.Open();
                await imgStream.CopyToAsync(iconStream, cancellationToken);
            }
        } else {
            string infoFile = Path.Combine(path, "pack.mcmeta");
            if (!File.Exists(infoFile))
                return null;

            var json = await File.ReadAllTextAsync(infoFile, cancellationToken);
            jsonNode = json.AsNode();

            string imgFile = Path.Combine(path, "pack.png");
            if (File.Exists(imgFile)) {
                using var imgStream = File.OpenRead(imgFile);
                await imgStream.CopyToAsync(iconStream, cancellationToken);
            }
        }

        var pack = jsonNode?.Select("pack");
        if (pack == null)
            return null;

        iconStream.Position = 0;
        return new Resourcepack {
            FileName = id,
            Path = path,
            IsEnabled = enabledPacksIds?.Contains(id) ?? false,
            IsExtracted = !isZip,
            Format = pack.GetInt32("pack_format"),
            Description = DescriptionRegex().Replace(pack.GetString("description") ?? string.Empty, string.Empty),
            Icon = iconStream.Length > 0 ? new Bitmap(iconStream) : null,
        };
    }

    [GeneratedRegex("§.")]
    private static partial Regex DescriptionRegex();
}

public record Resourcepack {
    public int Format { get; set; }

    public Bitmap Icon { get; set; }

    public string Path { get; set; }
    public string FileName { get; set; }
    public string Description { get; set; }

    public bool IsEnabled { get; set; }
    public bool IsExtracted { get; set; }
}