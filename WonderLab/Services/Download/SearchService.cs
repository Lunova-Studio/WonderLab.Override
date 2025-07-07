using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Base.Interfaces;
using MinecraftLaunch.Base.Models.Network;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Components.Provider;
using MinecraftLaunch.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Utilities;

namespace WonderLab.Services.Download;

public sealed class SearchService {
    private List<VersionManifestEntry> _minecrafts = [];

    private readonly ModrinthProvider _modrinthProvider;
    private readonly CurseforgeProvider _curseforgeProvider;
    private readonly FileInfo _searchCacheFileInfo = new(Path.Combine(PathUtil.GetDataFolderPath(), "search_caches.json"));

    public string Filter { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string MinecraftVersion { get; set; } = string.Empty;
    public SearchType SearchType { get; set; } = SearchType.Minecraft;
    public SearchSourceType SearchSource { get; set; } = SearchSourceType.Modrinth;
    public MinecraftVersionType MinecraftVersionType { get; set; } = MinecraftVersionType.Release;

    public ObservableCollection<object> Resources { get; private set; } = [];
    public ObservableCollection<SearchCache> Caches { get; private set; } = [];

    public SearchService(ModrinthProvider modrinthProvider, CurseforgeProvider curseforgeProvider) {
        _modrinthProvider = modrinthProvider;
        _curseforgeProvider = curseforgeProvider;
    }

    public void Reset() {
        Filter = string.Empty;
        Category = string.Empty;
        MinecraftVersion = string.Empty;
        SearchType = SearchType.Minecraft;
        SearchSource = SearchSourceType.Modrinth;
        MinecraftVersionType = MinecraftVersionType.Release;
    }

    public async Task SaveAsync() {
        var json = Caches?.Serialize(SearchCacheContext.Default.IEnumerableSearchCache);
        await File.WriteAllTextAsync(_searchCacheFileInfo.FullName, json);
    }

    public async Task InitSearchCacheContainerAsync() {
        Caches = [];

        if (!_searchCacheFileInfo.Exists)
            await _searchCacheFileInfo.Create().DisposeAsync();

        var json = await File.ReadAllTextAsync(_searchCacheFileInfo.FullName);
        if (string.IsNullOrEmpty(json))
            await SaveAsync();
        else
            Caches = [.. json.Deserialize(SearchCacheContext.Default.IEnumerableSearchCache)];
    }

    public async Task InitMinecraftsAsync(CancellationToken cancellationToken) {
        if (_minecrafts is { Count: > 0 }) {
            FilterMinecrafts();
            return;
        }

        var minecrafts = await VanillaInstaller.EnumerableMinecraftAsync(cancellationToken);
        _minecrafts = [.. minecrafts];

        if (SearchType is SearchType.Minecraft)
            FilterMinecrafts();
    }

    public Task<IEnumerable<ModrinthResource>> GetFeaturedResourcesAsync(CancellationToken cancellationToken) {
        return _modrinthProvider.GetFeaturedResourcesAsync(cancellationToken);
    }

    public async Task SearchAsync(string keyword, SearchType searchType, CancellationToken cancellationToken) {
        if (!string.IsNullOrEmpty(keyword) && !Caches.Any(x => x.Keyword == keyword && x.SearchType == searchType))
            Caches.Insert(0, new(keyword, searchType));

        Filter = keyword;
        SearchType = searchType;

        await SaveAsync();

        if (searchType is SearchType.Minecraft)
            FilterMinecrafts();
        else
            await SearchResourcesAsync(cancellationToken);
    }

    public void FilterMinecrafts() {
        Resources?.Clear();

        var minecrafts = _minecrafts.Where(x => x.Type == GetVersionType(MinecraftVersionType)
            && (string.IsNullOrEmpty(Filter) || x.Id.Contains(Filter)));

        foreach (var item in minecrafts)
            Resources.Add(item);
    }

    public async Task SearchResourcesAsync(CancellationToken cancellationToken) {
        Resources?.Clear();

        IEnumerable<IResource> resources = SearchSource is SearchSourceType.Curseforge
            ? await _curseforgeProvider.SearchResourcesAsync(Filter,
                GetClassId(SearchType), 0, MinecraftVersion)
            : await _modrinthProvider.SearchAsync(Filter, MinecraftVersion, Category,
                GetProjectType(SearchType), ModrinthSearchIndex.Relevance, cancellationToken);

        foreach (var resource in resources) {
            resource.Summary.Replace(Environment.NewLine, " ");
            Resources.Add(resource);
        }
    }

    private static int GetClassId(SearchType searchType) {
        return searchType switch {
            SearchType.Mod => 6,
            SearchType.Mpdpack => 4471,
            SearchType.Datapack => 6945,
            SearchType.Shaderpack => 6552,
            SearchType.Resourcepack => 12,
            _ => 6
        };
    }

    private static string GetProjectType(SearchType searchType) {
        return searchType switch {
            SearchType.Mod => "mod",
            SearchType.Mpdpack => "modpack",
            SearchType.Datapack => "datapack",
            SearchType.Shaderpack => "shader",
            SearchType.Resourcepack => "resourcepack",
            _ => "mod"
        };
    }

    private static string GetVersionType(MinecraftVersionType minecraftType) {
        return minecraftType switch {
            MinecraftVersionType.OldBeta => "old_beta",
            MinecraftVersionType.OldAlpha => "old_alpha",
            MinecraftVersionType.Release => "release",
            MinecraftVersionType.Snapshot => "snapshot",
            _ => "release",
        };
    }
}

public record SearchCache(
    [property: JsonPropertyName("keyword")]
    string Keyword,
    [property: JsonPropertyName("searchType")]
    SearchType SearchType);

[JsonSerializable(typeof(IEnumerable<SearchCache>))]
public sealed partial class SearchCacheContext : JsonSerializerContext;