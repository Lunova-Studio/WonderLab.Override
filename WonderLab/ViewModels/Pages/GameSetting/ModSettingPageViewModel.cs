using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Services.Auxiliary;

namespace WonderLab.ViewModels.Pages.GameSetting;

public sealed partial class ModSettingPageViewModel : ObservableObject {
    private readonly ModService _modService;
    private readonly ILogger<ModSettingPageViewModel> _logger;

    private ObservableCollection<Mod> _mod = [];
    private CancellationTokenSource _cancellationTokenSource = new();

    [ObservableProperty] private bool _hasMods;
    [ObservableProperty] private ReadOnlyObservableCollection<Mod> _mods;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUpdateMods))]
    private int _updateModCount;

    public bool HasUpdateMods => UpdateModCount > 0;

    public ModSettingPageViewModel(ModService modService, ILogger<ModSettingPageViewModel> logger) {
        _logger = logger;
        _modService = modService;

        Mods = new(_mod);
        _mod.CollectionChanged += OnCollectionChanged;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        try {
            await Task.Delay(150);

            _modService.Init();
            await foreach (var item in _modService.LoadAllAsync(_cancellationTokenSource.Token))
                _mod.Add(item);

            _logger.LogInformation("Loaded {count} local mod", Mods.Count);
            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            _logger.LogInformation("checking mod update");
            await _modService.CheckModsUpdateAsync(_mod, _cancellationTokenSource.Token);

            UpdateModCount = _mod.Where(x => x.CanUpdate).Count();
        } catch (Exception) { }

        UpdateModCount = Mods.Where(x => x.CanUpdate).Count();
    }, _cancellationTokenSource.Token);

    [RelayCommand]
    private Task Refresh() => OnLoaded();

    [RelayCommand]
    private void OnDetachedFromVisualTree() {
        _cancellationTokenSource.Cancel();
    }

    [RelayCommand]
    private Task Save(Mod mod) => Task.Run(() => {
        _modService.ChangeExtension(mod);
    });

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        HasMods = Mods.Count > 0;
    }
}