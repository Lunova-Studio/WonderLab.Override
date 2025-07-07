using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Services.Auxiliary;

namespace WonderLab.ViewModels.Pages.GameSetting;

public sealed partial class ResourcepackSettingPageViewModel : ObservableObject {
    private readonly ResourcepackService _resourcepackService;
    private readonly ILogger<ResourcepackSettingPageViewModel> _logger;

    [ObservableProperty] private bool _hasResourcepacks;
    [ObservableProperty] private ReadOnlyObservableCollection<Resourcepack> _resourcepacks;

    public ResourcepackSettingPageViewModel(ResourcepackService resourcepackService, ILogger<ResourcepackSettingPageViewModel> logger) {
        _resourcepackService = resourcepackService;
        _logger = logger;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        _resourcepackService.Init();
        await _resourcepackService.LoadAllAsync(default);
        Resourcepacks = new(_resourcepackService.Resourcepacks);

        HasResourcepacks = Resourcepacks.Count > 0;
        _logger.LogInformation("Loaded {count} resourcepack", Resourcepacks.Count);
    });

    [RelayCommand]
    private Task Save() => Task.Run(async () => {
        await _resourcepackService.SaveToOptionsAsync(default);
    });
}