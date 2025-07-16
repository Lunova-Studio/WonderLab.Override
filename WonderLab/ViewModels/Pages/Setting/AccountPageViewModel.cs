using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Base.Models.Authentication;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using WonderLab.Services;
using WonderLab.Services.Authentication;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class AccountPageViewModel : PageViewModelBase {
    private readonly DialogService _dialogService;
    private readonly AccountService _accountService;

    [ObservableProperty] private bool _hasAccounts;
    [ObservableProperty] private ReadOnlyObservableCollection<Account> _accounts;

    public AccountPageViewModel(AccountService accountService, DialogService dialogService) {
        _dialogService = dialogService;
        _accountService = accountService;
    }

    [RelayCommand]
    private async Task ShowAddAccountDialog() {
        await _dialogService.ShowDialogAsync("ChooseAccountType");
    }

    [RelayCommand]
    private void ActiveAccount(Account account) {
        _accountService.ActivateAccount(account);
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        HasAccounts = Accounts.Count is > 0;
    }

    protected override void OnNavigated() {
        Accounts = new(_accountService.Accounts);
        HasAccounts = Accounts.Count is > 0;

        _accountService.Accounts.CollectionChanged += OnCollectionChanged;
    }

    protected override void OnUnNavigated() {
        base.OnUnNavigated();
        _accountService.Accounts.CollectionChanged -= OnCollectionChanged;
    }
}