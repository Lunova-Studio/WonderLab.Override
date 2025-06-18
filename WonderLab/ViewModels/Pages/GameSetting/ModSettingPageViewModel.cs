using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Services.Auxiliary;

namespace WonderLab.ViewModels.Pages.GameSetting;

public sealed partial class ModSettingPageViewModel : ObservableObject {
    private readonly ModService _modService;
    private readonly ILogger<ModSettingPageViewModel> _logger;

    private CancellationTokenSource _cancellationTokenSource = new();

    [ObservableProperty] private bool _hasMods;
    [ObservableProperty] private ReadOnlyObservableCollection<Mod> _mods;

    public ModSettingPageViewModel(ModService modService, ILogger<ModSettingPageViewModel> logger) {
        _logger = logger;
        _modService = modService;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        try {
            _modService.Init();
            Mods = new(_modService.Mods);

            _modService.LoadAll(_cancellationTokenSource.Token);

            HasMods = Mods.Count > 0;
            _logger.LogInformation("Loaded {count} local mod", Mods.Count);

            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            _logger.LogInformation("checking mod update");
            await _modService.CheckModsUpdateAsync(_cancellationTokenSource.Token);
        } catch (System.Exception) {}
    }, _cancellationTokenSource.Token);

    [RelayCommand]
    private void OnDetachedFromVisualTree() {
        _cancellationTokenSource.Cancel();
    }

    [RelayCommand]
    private Task Save(Mod mod) => Task.Run(() => {
        _modService.ChangeExtension(mod);
    });
}