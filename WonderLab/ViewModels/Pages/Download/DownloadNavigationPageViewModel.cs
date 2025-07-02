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

public sealed partial class DownloadNavigationPageViewModel : ObservableObject {
    private readonly SearchService _searchService;
    private readonly AvaloniaPageProvider _pageProvider;

    [ObservableProperty] private string _activePageKey;
    [ObservableProperty] private bool _hasSearchCache = true;
    [ObservableProperty] private bool _isEnterKeyDown = false;
    [ObservableProperty] private bool _isFeaturedResourcesLoading = true;
    [ObservableProperty] private SearchType _activeSearchType = SearchType.Minecraft;
    [ObservableProperty] private ObservableCollection<string> _headerItems = ["下载"];
    [ObservableProperty] private ObservableCollection<FeaturedResourcesItem> _featuredResources;
    [ObservableProperty] private ReadOnlyObservableCollection<SearchCache> _searchCaches;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNextButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsPreviousButtonVisible))]
    private int _featuredResourcesIndex = -1;

    public AvaloniaPageProvider PageProvider => _pageProvider;
    public bool IsPreviousButtonVisible => FeaturedResourcesIndex is not 0 && FeaturedResources?.Count > 1;
    public bool IsNextButtonVisible => FeaturedResourcesIndex != FeaturedResources?.Count - 1 && FeaturedResources?.Count > 1;

    public DownloadNavigationPageViewModel(SearchService searchService, AvaloniaPageProvider pageProvider) {
        _searchService = searchService;
        _pageProvider = pageProvider;
    }

    [RelayCommand]
    private void JumpToSearchPage() {
        ActivePageKey = "Download/Search";
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
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
    private Task Search(string text) => Task.Run(async () => {
        IsEnterKeyDown = true;
        if (string.IsNullOrEmpty(text)) {
            ActivePageKey = string.Empty;
            return;
        }

        JumpToSearchPageCommand.Execute(default);
        await _searchService.SearchAsync(text, ActiveSearchType, default);
    });
}

public record FeaturedResourcesItem(ModrinthResource Resource, string FirstImageUrl);