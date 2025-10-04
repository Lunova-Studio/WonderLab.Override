using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Models.Network;
using ObservableCollections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class DownloadDashboardPageViewModel : PageViewModelBase {
    private readonly SearchService _searchService;
    private readonly AvaloniaPageProvider _pageProvider;

    [ObservableProperty] private string _keyword;
    [ObservableProperty] private string _activePageKey;
    [ObservableProperty] private bool _isHide = false;
    [ObservableProperty] private bool _hasSearchCache = true;
    [ObservableProperty] private bool _isEnterKeyDown = false;
    [ObservableProperty] private bool _isFeaturedResourcesLoading = true;
    [ObservableProperty] private SearchType _activeSearchType = SearchType.Minecraft;
    [ObservableProperty] private ObservableCollection<FeaturedResourcesItem> _featuredResources;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNextButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsPreviousButtonVisible))]
    private int _featuredResourcesIndex = -1;

    public INotifyCollectionChangedSynchronizedViewList<SearchCache> SearchCaches { get; }

    public AvaloniaPageProvider PageProvider => _pageProvider;
    public bool IsPreviousButtonVisible => FeaturedResourcesIndex is not 0 && FeaturedResources?.Count > 1;
    public bool IsNextButtonVisible => FeaturedResourcesIndex != FeaturedResources?.Count - 1 && FeaturedResources?.Count > 1;

    public DownloadDashboardPageViewModel(SearchService searchService, AvaloniaPageProvider pageProvider) {
        _searchService = searchService;
        _pageProvider = pageProvider;

        _searchService.Reset();
        SearchCaches = _searchService.Caches.ToNotifyCollectionChanged();

        WeakReferenceMessenger.Default.Register<RequestSearchMessage>(this, async (_, _) => {
            await SearchCommand.ExecuteAsync(string.Empty);
        });

        WeakReferenceMessenger.Default.Register<RequestDownloadPageGobackMessage>(this, (_, _) => {
            Keyword = string.Empty;
            ActivePageKey = string.Empty;
            IsEnterKeyDown = true;
            IsHide = false;
        });
    }

    [RelayCommand]
    private void JumpToSearchPage(string flag) {
        if (flag is not null)
            _searchService.Reset();

        IsHide = true;
        ActivePageKey = "Download/Search";
        WeakReferenceMessenger.Default.Send(new RequestPageMessage("Search"));
    }

    [RelayCommand]
    private Task Search(string flag = null) => Task.Run(async () => {
        if (string.IsNullOrEmpty(Keyword) && flag is null)
            return;

        IsEnterKeyDown = true;
        HasSearchCache = _searchService.Caches.Count > 0;
        JumpToSearchPageCommand.Execute(default);

        await _searchService.SearchAsync(Keyword, _searchService.SearchType, default);
    });

    [RelayCommand]
    private Task SearchHistory(SearchCache searchCache) => Task.Run(async () => {
        Keyword = searchCache.Keyword;
        IsEnterKeyDown = true;
        JumpToSearchPageCommand.Execute(default);
        await _searchService.SearchAsync(Keyword, searchCache.SearchType, default);
    });

    [RelayCommand]
    private Task RemoveHistory(SearchCache searchCache) => Task.Run(async () => {
        if (_searchService.Caches.Remove(searchCache)) {
            HasSearchCache = _searchService.Caches.Count > 0;
            await _searchService.SaveAsync();
        }
    });

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(Keyword))
            _searchService.Filter = Keyword;

        if (e.PropertyName is nameof(ActiveSearchType))
            _searchService.SearchType = ActiveSearchType;
    }

    protected override void OnNavigated() => Task.Run(async () => {
        await _searchService.InitSearchCacheContainerAsync();

        var featuredResources = await _searchService.GetFeaturedResourcesAsync(CancellationTokenSource.Token);
        FeaturedResources = [.. featuredResources.Select(x => new FeaturedResourcesItem(x,
            x.Screenshots?.FirstOrDefault() ?? x.IconUrl))];

        FeaturedResourcesIndex = 0;
        IsFeaturedResourcesLoading = false;

        HasSearchCache = _searchService.Caches.Count > 0;
        PropertyChanged += OnPropertyChanged;
    });
}

public record FeaturedResourcesItem(ModrinthResource Resource, string FirstImageUrl);