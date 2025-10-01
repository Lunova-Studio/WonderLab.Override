using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Base.Models.Network;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class SearchPageViewModel : PageViewModelBase {
    private readonly SearchService _searchService;

    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private string _category = string.Empty;
    [ObservableProperty] private SearchSourceType _searchSource = SearchSourceType.Modrinth;
    [ObservableProperty] private MinecraftVersionType _minecraftVersionType = MinecraftVersionType.Release;
    [ObservableProperty] private ReadOnlyObservableCollection<object> _resources;
    [ObservableProperty] private ReadOnlyObservableCollection<string> _categories;

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

        Resources = new(_searchService.Resources);
        Categories = new(_searchService.Categories);

        Category = _searchService.Category;
        SearchType = _searchService.SearchType;
        SearchSource = _searchService.SearchSource;
        MinecraftVersionType = _searchService.MinecraftVersionType;

        if (SearchType is SearchType.Minecraft)
            await _searchService.InitMinecraftsAsync(default);

        IsLoading = Resources.Count == 0;
        PropertyChanged += OnPropertyChanged;
        _searchService.Resources.CollectionChanged += OnCollectionChanged;
    });

    [RelayCommand]
    private static void JumpToResourcePage(object parameter) {
        if (parameter is VersionManifestEntry minecraft)
            WeakReferenceMessenger.Default.Send(new RequestResourcePageMessage("Download/Minecraft", minecraft.Id, minecraft));
        else if (parameter is ModrinthResource modrinthResource)
            WeakReferenceMessenger.Default.Send(new RequestResourcePageMessage("Download/Minecraft", modrinthResource.Name, modrinthResource)); // WIP
        else if (parameter is CurseforgeResource curseforgeResource)
            WeakReferenceMessenger.Default.Send(new RequestResourcePageMessage("Download/Minecraft", curseforgeResource.Name, curseforgeResource)); // WIP
        else
            throw new NotSupportedException($"Unsupported resource type");
    }

    [RelayCommand]
    private static void Search() {
        WeakReferenceMessenger.Default.Send(new RequestSearchMessage());
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(MinecraftVersionType):
                _searchService.MinecraftVersionType = MinecraftVersionType;
                _searchService.FilterMinecrafts();
                break;
            case nameof(SearchType):
                _searchService.SearchType = SearchType;
                WeakReferenceMessenger.Default.Send(new RequestSearchMessage());
                break;
            case nameof(SearchSource):
                _searchService.SearchSource = SearchSource;
                _searchService.UpdateCategories();
                Category = _searchService.Category;
                break;
            case nameof(Category):
                _searchService.Category = Category;
                break;
        }
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        IsLoading = Resources.Count == 0;
    }
}