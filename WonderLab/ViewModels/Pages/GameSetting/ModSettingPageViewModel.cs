using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Models.Messaging;
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
    private void OnLoaded() => RefreshCommand.ExecuteAsync(default);

    [RelayCommand]
    private Task Refresh() => Task.Run(async () => {
        try {
            await Task.Delay(150);
            WeakReferenceMessenger.Default.Send(new PageDataLoadingMessage(true));

            _mod?.Clear();
            _modService.Init();
            await foreach (var item in _modService.LoadAllAsync(_cancellationTokenSource.Token))
                _mod.Add(item);

            _logger.LogInformation("Loaded {count} local mod", Mods.Count);
            if (!NetworkInterface.GetIsNetworkAvailable() || _mod.Count is 0) {
                WeakReferenceMessenger.Default.Send(new PageDataLoadingMessage(false));
                return;
            }

            _logger.LogInformation("checking mod update");
            await _modService.CheckModsUpdateAsync(_mod, _cancellationTokenSource.Token);

        } catch (Exception) { }

        UpdateModCount = _mod.Where(x => x.CanUpdate).Count();
        WeakReferenceMessenger.Default.Send(new PageDataLoadingMessage(false));
    }, _cancellationTokenSource.Token);

    [RelayCommand]
    private void OnUnNavigated() {
        _cancellationTokenSource.Cancel();
        WeakReferenceMessenger.Default.Send(new PageDataLoadingMessage(false));
    }

    [RelayCommand]
    private Task Save(Mod mod) => Task.Run(() => {
        _modService.ChangeExtension(mod);
    });

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        HasMods = Mods.Count > 0;
    }
}