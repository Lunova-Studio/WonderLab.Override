using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Models.Authentication;
using ObservableCollections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services;
using WonderLab.Services.Authentication;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class AddAccountPageViewModel : PageViewModelBase {
    private readonly DialogService _dialogService;
    private readonly AccountService _accountService;
    private readonly SettingService _settingService;
    private readonly ObservableList<AccountModel> _accounts = [];

    [ObservableProperty] private AccountModel _activeAccount;

    public INotifyCollectionChangedSynchronizedViewList<AccountModel> Accounts { get; }

    public AddAccountPageViewModel(SettingService settingService, AccountService accountService, DialogService dialogService) {
        _dialogService = dialogService;
        _accountService = accountService;
        _settingService = settingService;

        Accounts = _accounts.ToNotifyCollectionChangedSlim();
    }

    [RelayCommand]
    private async Task OpenAddAccountDialog() {
        await _dialogService.ShowDialogAsync("ChooseAccountType");
    }

    private void SendEnabledMessage() {
        WeakReferenceMessenger.Default.Send(new EnabledChangedMessage(_accounts.Count > 0 && ActiveAccount != null));
    }

    private void OnCollectionChanged(in NotifyCollectionChangedEventArgs<Account> e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                Accounts.Add(new AccountModel(e.NewItem, default));
                break;
            case NotifyCollectionChangedAction.Remove:
                Accounts.Remove(e.OldItem);
                break;
        }

        SendEnabledMessage();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(ActiveAccount)) {
            SendEnabledMessage();
        }
    }

    protected override void OnNavigated() {
        _accounts.AddRange(_accountService.Accounts
            .Select(x => new AccountModel(x, default)));

        ActiveAccount = Accounts.FirstOrDefault(x => x.Account.ProfileEquals(_accountService.ActiveAccount));
        _accountService.Accounts.CollectionChanged += OnCollectionChanged;
        PropertyChanged += OnPropertyChanged;

        SendEnabledMessage();
    }

    protected override void OnUnNavigated() {
        base.OnUnNavigated();
        _accountService.Accounts.CollectionChanged -= OnCollectionChanged;
    }

    partial void OnActiveAccountChanged(AccountModel value) {
        _accountService.ActivateAccount(value.Account);
    }
}