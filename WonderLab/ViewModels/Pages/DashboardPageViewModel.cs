using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Authentication;
using ObservableCollections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using WonderLab.Services.Authentication;
using WonderLab.Services.Auxiliary;

namespace WonderLab.ViewModels.Pages;

public sealed partial class DashboardPageViewModel : DynamicPageViewModelBase {
    private readonly SaveService _saveService;
    private readonly AccountService _accountService;
    private readonly ILogger<DashboardPageViewModel> _logger;

    [ObservableProperty] private bool _hasSaves;
    [ObservableProperty] private Account _activeAccount;
    [ObservableProperty] private ReadOnlyObservableCollection<SaveModel> _lastSaves;

    public INotifyCollectionChangedSynchronizedViewList<Account> Accounts { get; private set; }

    public DashboardPageViewModel(AccountService accountService, SaveService saveService, ILogger<DashboardPageViewModel> logger) {
        _logger = logger;
        _saveService = saveService;
        _accountService = accountService;

        Accounts = _accountService.Accounts.ToNotifyCollectionChangedSlim();
    }

    partial void OnActiveAccountChanged(Account value) => _accountService.ActivateAccount(value);

    protected override void OnNavigated() => Task.Run(async () => {
        await _saveService.RefreshSavesAsync();

        LastSaves = new(_saveService.Saves);
        ActiveAccount = _accountService.ActiveAccount;

        HasSaves = LastSaves.Count > 0;
        _logger.LogInformation("Loaded {count} save", LastSaves.Count);
    });
}