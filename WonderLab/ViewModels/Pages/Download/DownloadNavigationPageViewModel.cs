using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Base.Models.Network;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class DownloadNavigationPageViewModel : ObservableObject {
    private readonly SearchService _searchService;

    [ObservableProperty] private bool _isFeaturedResourcesLoading = true;
    [ObservableProperty] private ObservableCollection<string> _headerItems = ["Download"];
    [ObservableProperty] private ObservableCollection<FeaturedResourcesItem> _featuredResources;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNextButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsPreviousButtonVisible))]
    private int _featuredResourcesIndex = -1;

    public bool IsPreviousButtonVisible => FeaturedResourcesIndex is not 0 && FeaturedResources?.Count > 1;
    public bool IsNextButtonVisible => FeaturedResourcesIndex != FeaturedResources?.Count - 1 && FeaturedResources?.Count > 1;

    public DownloadNavigationPageViewModel(SearchService searchService) {
        _searchService = searchService;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        var featuredResources = await _searchService.GetFeaturedResourcesAsync(default);
        FeaturedResources = [.. featuredResources.Select(x => new FeaturedResourcesItem(x,
            x.ScreenshotUrls?.FirstOrDefault() ?? x.IconUrl))];

        FeaturedResourcesIndex = 0;
        IsFeaturedResourcesLoading = false;
    });
}

public record FeaturedResourcesItem(ModrinthResource Resource, string FirstImageUrl);