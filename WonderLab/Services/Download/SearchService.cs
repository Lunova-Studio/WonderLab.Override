using MinecraftLaunch.Base.Models.Network;
using MinecraftLaunch.Components.Provider;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Services.Download;

public sealed class SearchService {
    private readonly ModrinthProvider _modrinthProvider;

    public SearchService(ModrinthProvider modrinthProvider) {
        _modrinthProvider = modrinthProvider;
    }

    public Task<IEnumerable<ModrinthResource>> GetFeaturedResourcesAsync(CancellationToken cancellationToken) {
        return _modrinthProvider.GetFeaturedResourcesAsync(cancellationToken);
    }
}