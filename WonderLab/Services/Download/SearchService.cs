using MinecraftLaunch.Base.Models.Network;
using MinecraftLaunch.Components.Provider;
using MinecraftLaunch.Extensions;
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
    private readonly ModrinthProvider _modrinthProvider;
    private readonly FileInfo _searchCacheFileInfo = new(Path.Combine(PathUtil.GetDataFolderPath(), "search_caches.json"));

    public ObservableCollection<SearchCache> Caches { get; private set; }

    public SearchService(ModrinthProvider modrinthProvider) {
        _modrinthProvider = modrinthProvider;
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

    public Task<IEnumerable<ModrinthResource>> GetFeaturedResourcesAsync(CancellationToken cancellationToken) {
        return _modrinthProvider.GetFeaturedResourcesAsync(cancellationToken);
    }

    public async Task SearchAsync(string keyword, SearchType searchType, CancellationToken cancellationToken) {
        if (!string.IsNullOrEmpty(keyword) && !Caches.Any(x => x.Keyword == keyword && x.SearchType == searchType))
            Caches.Add(new(keyword, searchType));

        await SaveAsync();
    }
}

public record SearchCache(
    [property: JsonPropertyName("keyword")]
    string Keyword,
    [property: JsonPropertyName("searchType")]
    SearchType SearchType);

[JsonSerializable(typeof(IEnumerable<SearchCache>))]
public sealed partial class SearchCacheContext : JsonSerializerContext;