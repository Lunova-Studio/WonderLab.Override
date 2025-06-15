using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Services.Auxiliary;

namespace WonderLab.ViewModels.Pages.GameSetting;

public sealed partial class ModSettingPageViewModel : ObservableObject {
    private readonly ModService _modService;
    private readonly ILogger<ModSettingPageViewModel> _logger;

    [ObservableProperty] private ReadOnlyObservableCollection<Mod> _mods;

    public ModSettingPageViewModel(ModService modService, ILogger<ModSettingPageViewModel> logger) {
        _logger = logger;
        _modService = modService;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(() => {
        _modService.Init();
        _modService.LoadAll();
        Mods = new(_modService.Mods);

        _logger.LogInformation("Loaded {count} mod", Mods.Count);
    });

    [RelayCommand]
    private Task Save(Mod mod) => Task.Run(() => {
        _modService.ChangeExtension(mod);
    });
}
