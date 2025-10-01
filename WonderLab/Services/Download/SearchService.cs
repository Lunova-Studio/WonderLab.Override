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

    private readonly string[] ModrinthCategories = [
        "all", "adventure", "cursed", "decoration", "economy", "equipment", "food",
        "game-mechanics", "library", "magic", "management", "minigame", "mobs", "optimization",
        "social", "storage", "technology", "transportation", "utility", "worldgen"
    ];

    private readonly Dictionary<string, int> CurseForgeCategories = new() {
        { "All", 0 }, { "Map and Information", 423 }, { "Armor, Tools, and Weapons", 434 }, { "API and Library", 421 }, { "Adventure and RPG", 422 }, { "Processing", 413 },
        { "Utility & QoL", 5191 }, { "Education", 5299 }, { "Miscellaneous", 425 }, { "Server Utility", 435 }, { "Technology", 412 }, { "Food", 436 }, { "World Gen", 406 },
        { "Storage", 420 }, { "Structures", 409 }, { "Addons", 426 }, { "Tinker's Construct", 428 }, { "Blood Magic", 4485 }, { "Bug Fixes", 6821 }, { "Industrial Craft", 429 },
        { "Galacticraft", 5232 }, { "Farming", 416 }, { "Magic", 419 }, { "Automation", 4843 }, { "Applied Energistics 2", 4545 }, { "Twitch Integration", 4671 },
        { "CraftTweaker", 4773 }, { "Integrated Dynamics", 6954 }, { "Create", 6484 }, { "Mobs", 411 }, { "Skyblock", 6145 }, { "MCreator", 4906 }, { "Cosmetic", 424 },
        { "KubeJS", 5314 }, { "Redstone", 4558 }, { "Performance", 6814 }, { "Player Transport", 414 }, { "Biomes", 407 }, { "Ores and Resources", 408 },
        { "Energy, Fluid, and Item Transport", 415 }, { "Buildcraft", 432 }, { "Thaumcraft", 430 }, { "Thermal Expansion", 427 }, { "Dimensions", 410 }, { "Energy", 417 },
        { "Twilight Forest", 7669 }, { "Genetics", 418 }, { "Forestry", 433 }
    };

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
    public ObservableCollection<string> Categories { get; private set; } = [];
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

    public void UpdateCategories() {
        Categories.Clear();
        var categories = SearchSource is SearchSourceType.Modrinth
            ? ModrinthCategories
            : [.. CurseForgeCategories.Keys];

        foreach (var category in categories)
            Categories.Add(category);

        Category = Categories?.FirstOrDefault();
    }

    public void FilterMinecrafts() {
        var minecrafts = _minecrafts.Where(x => x.Type == GetVersionType(MinecraftVersionType)
            && (string.IsNullOrEmpty(Filter) || x.Id.Contains(Filter)));

        foreach (var item in minecrafts)
            Resources.Add(item);
    }

    public async Task SaveAsync() {
        try {
            var json = Caches?.Serialize(SearchCacheContext.Default.IEnumerableSearchCache);
            await File.WriteAllTextAsync(_searchCacheFileInfo.FullName, json);
        } catch (Exception) { }
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

        Resources?.Clear();
        if (searchType is SearchType.Minecraft)
            FilterMinecrafts();
        else
            await SearchResourcesAsync(cancellationToken);
    }

    public async Task SearchResourcesAsync(CancellationToken cancellationToken) {
        IEnumerable<IResource> resources = SearchSource is SearchSourceType.Curseforge
            ? await _curseforgeProvider.SearchResourcesAsync(Filter,
                GetClassId(SearchType), GetCurseforgeCategory(Category), MinecraftVersion)
            : await _modrinthProvider.SearchAsync(Filter, MinecraftVersion, GetModrinthCategory(Category),
                GetProjectType(SearchType), ModrinthSearchIndex.Relevance, cancellationToken);

        foreach (var resource in resources)
            Resources.Add(resource);
    }

    private int GetCurseforgeCategory(string category) {
        if (string.IsNullOrEmpty(category))
            return 0;

        return CurseForgeCategories[category];
    }

    private static string GetModrinthCategory(string category) {
        // 此情况不会返回任何数据
        return category is "all" ? string.Empty : category;
    }

    private static int GetClassId(SearchType searchType) {
        return searchType switch {
            SearchType.Mod => 6,
            SearchType.Modpack => 4471,
            SearchType.Datapack => 6945,
            SearchType.Shaderpack => 6552,
            SearchType.Resourcepack => 12,
            _ => 6
        };
    }

    private static string GetProjectType(SearchType searchType) {
        return searchType switch {
            SearchType.Mod => "mod",
            SearchType.Modpack => "modpack",
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