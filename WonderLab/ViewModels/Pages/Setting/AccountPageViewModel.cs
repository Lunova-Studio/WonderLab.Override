using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ObservableCollections;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using WonderLab.Services;
using WonderLab.Services.Authentication;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class AccountPageViewModel : PageViewModelBase {
    private readonly DialogService _dialogService;
    private readonly AccountService _accountService;
    private readonly SettingService _settingService;
    private readonly ObservableList<AccountModel> _accounts = [];

    [ObservableProperty] private bool _isAutoRefreshAccount;
    [ObservableProperty] private bool _hasAccounts = true;
    [ObservableProperty] private AccountModel _activeAccount;

    public INotifyCollectionChangedSynchronizedViewList<AccountModel> Accounts { get; }

    public AccountPageViewModel(SettingService settingService, AccountService accountService, DialogService dialogService) {
        _dialogService = dialogService;
        _accountService = accountService;
        _settingService = settingService;

        Accounts = _accounts.ToNotifyCollectionChangedSlim();
        IsAutoRefreshAccount = _settingService.Setting.IsAutoRefreshAccount;
    }

    [RelayCommand]
    private void RemoveAccount(AccountModel account) {
        if (account is null)
            return;

        _accounts.Remove(account);
        _accountService.RemoveAccount(account.Account);
    }

    [RelayCommand]
    private async Task ShowAddAccountDialog() {
        await _dialogService.ShowDialogAsync("ChooseAccountType");
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        HasAccounts = _accounts.Count is > 0;
    }

    protected override void OnNavigated() {
        _accounts.AddRange(_accountService.Accounts
            .Select(x => new AccountModel(x, default)));

        ActiveAccount = Accounts.FirstOrDefault(x => x.Account.Equals(_accountService.ActiveAccount));
        HasAccounts = _accounts.Count is > 0;
        _accountService.Accounts.CollectionChanged += OnCollectionChanged;
    }

    protected override void OnUnNavigated() {
        base.OnUnNavigated();
        _accountService.Accounts.CollectionChanged -= OnCollectionChanged;
    }

    partial void OnActiveAccountChanged(AccountModel value) {
        _accountService.ActivateAccount(value.Account);
    }

    partial void OnIsAutoRefreshAccountChanged(bool value) {
        _settingService.Setting.IsAutoRefreshAccount = value;
    }
}