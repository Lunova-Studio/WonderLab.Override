using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class SearchPageViewModel : ObservableObject {
    private readonly SearchService _searchService;

    [ObservableProperty] private ReadOnlyObservableCollection<object> _resources;
    [ObservableProperty] private ReadOnlyObservableCollection<string> _categories;
    [ObservableProperty] private string _category = string.Empty;
    [ObservableProperty] private SearchSourceType _searchSource = SearchSourceType.Modrinth;
    [ObservableProperty] private MinecraftVersionType _minecraftVersionType = MinecraftVersionType.Release;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCommunityResourcesFilterVisible))]
    private SearchType _searchType = SearchType.Minecraft;

    public bool IsCommunityResourcesFilterVisible => SearchType is not SearchType.Minecraft;

    public SearchPageViewModel(SearchService searchService) {
        _searchService = searchService;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        _searchService.UpdateCategories();
        if (SearchType is SearchType.Minecraft)
            await _searchService.InitMinecraftsAsync(default);

        Resources = new(_searchService.Resources);
        Categories = new(_searchService.Categories);

        Category = _searchService.Category;
        SearchType = _searchService.SearchType;
        SearchSource = _searchService.SearchSource;
        MinecraftVersionType = _searchService.MinecraftVersionType;

        PropertyChanged += OnPropertyChanged;
    });

    [RelayCommand]
    private static void Search() {
        WeakReferenceMessenger.Default.Send(new RequestSearchMessage());
    }

    private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(MinecraftVersionType):
                _searchService.MinecraftVersionType = MinecraftVersionType;
                _searchService.FilterMinecrafts();
                break;
            case nameof(SearchType):
                _searchService.SearchType = SearchType;
                await _searchService.SearchResourcesAsync(default);
                break;
            case nameof(SearchSource):
                _searchService.SearchSource = SearchSource;
                _searchService.UpdateCategories();
                Category = _searchService.Category;
                await _searchService.SearchResourcesAsync(default);
                break;
            case nameof(Category):
                _searchService.Category = Category;
                await _searchService.SearchResourcesAsync(default);
                break;
        }
    }
}