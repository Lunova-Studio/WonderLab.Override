using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Authentication;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using WonderLab.Services.Authentication;
using WonderLab.Services.Auxiliary;
using WonderLab.Services.Launch;

namespace WonderLab.ViewModels.Pages;

public sealed partial class DashboardPageViewModel : DynamicPageViewModelBase {
    private readonly SaveService _saveService;
    private readonly AccountService _accountService;
    private readonly ILogger<DashboardPageViewModel> _logger;

    [ObservableProperty] private bool _hasSaves;
    [ObservableProperty] private Account _activeAccount;
    [ObservableProperty] private ReadOnlyObservableCollection<Account> _accounts;
    [ObservableProperty] private ReadOnlyObservableCollection<SaveModel> _lastSaves;

    public DashboardPageViewModel(AccountService accountService, SaveService saveService, ILogger<DashboardPageViewModel> logger) {
        _logger = logger;
        _saveService = saveService;
        _accountService = accountService;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        await Task.Delay(400);
        await _saveService.RefreshSavesAsync();

        LastSaves = new(_saveService.Saves);
        Accounts = new(_accountService.Accounts);
        ActiveAccount = _accountService.ActiveAccount;

        HasSaves = LastSaves.Count > 0;
        _logger.LogInformation("Loaded {count} save", LastSaves.Count);
    });

    partial void OnActiveAccountChanged(Account value) => _accountService.ActivateAccount(value);
}