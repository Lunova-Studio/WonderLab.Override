using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class SearchPageViewModel : ObservableObject {
    private readonly SearchService _searchService;

    [ObservableProperty] private ObservableCollection<object> _resources;

    public SearchPageViewModel(SearchService searchService) {
        _searchService = searchService;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
#if DEBUG
        Resources = [.. await _searchService.GetMinecraftsAsync(default)];
#endif
    });
}