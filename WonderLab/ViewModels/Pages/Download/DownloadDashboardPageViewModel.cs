using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Base.Models.Network;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class DownloadDashboardPageViewModel : ObservableObject {
    private readonly SearchService _searchService;
    private readonly AvaloniaPageProvider _pageProvider;

    [ObservableProperty] private string _activePageKey;
    [ObservableProperty] private bool _isHide = false;
    [ObservableProperty] private bool _hasSearchCache = true;
    [ObservableProperty] private bool _isEnterKeyDown = false;
    [ObservableProperty] private bool _isFeaturedResourcesLoading = true;
    [ObservableProperty] private SearchType _activeSearchType = SearchType.Minecraft;
    [ObservableProperty] private ObservableCollection<FeaturedResourcesItem> _featuredResources;
    [ObservableProperty] private ReadOnlyObservableCollection<SearchCache> _searchCaches;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNextButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsPreviousButtonVisible))]
    private int _featuredResourcesIndex = -1;

    public AvaloniaPageProvider PageProvider => _pageProvider;
    public bool IsPreviousButtonVisible => FeaturedResourcesIndex is not 0 && FeaturedResources?.Count > 1;
    public bool IsNextButtonVisible => FeaturedResourcesIndex != FeaturedResources?.Count - 1 && FeaturedResources?.Count > 1;

    public DownloadDashboardPageViewModel(SearchService searchService, AvaloniaPageProvider pageProvider) {
        _searchService = searchService;
        _pageProvider = pageProvider;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        await Task.Delay(200);
        await _searchService.InitSearchCacheContainerAsync();

        SearchCaches = new(_searchService.Caches);
        var featuredResources = await _searchService.GetFeaturedResourcesAsync(default);
        FeaturedResources = [.. featuredResources.Select(x => new FeaturedResourcesItem(x,
            x.ScreenshotUrls?.FirstOrDefault() ?? x.IconUrl))];

        FeaturedResourcesIndex = 0;
        IsFeaturedResourcesLoading = false;

        HasSearchCache = SearchCaches.Count > 0;
    });

    [RelayCommand]
    private void JumpToSearchPage() {
        IsHide = true;
        ActivePageKey = "Download/Search";
    }

    [RelayCommand]
    private Task Search(string text) => Task.Run(async () => {
        IsEnterKeyDown = true;
        if (string.IsNullOrEmpty(text)) {
            ActivePageKey = string.Empty;
            return;
        }

        JumpToSearchPageCommand.Execute(default);
        await _searchService.SearchAsync(text, ActiveSearchType, default);
    });

    [RelayCommand]
    private Task RemoveHistory(SearchCache searchCache) => Task.Run(async () => {
        if (_searchService.Caches.Remove(searchCache)) {
            HasSearchCache = SearchCaches.Count > 0;
            await _searchService.SaveAsync();
        }
    });
}

public record FeaturedResourcesItem(ModrinthResource Resource, string FirstImageUrl);