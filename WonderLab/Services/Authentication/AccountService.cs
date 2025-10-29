using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Authentication;
using ObservableCollections;
using System.Collections.Specialized;
using System.Linq;
using WonderLab.Classes.Models.Messaging;

namespace WonderLab.Services.Authentication;

public sealed class AccountService {
    private readonly SettingService _settingService;
    private readonly ILogger<AccountService> _logger;

    public Account ActiveAccount { get; private set; }
    public ObservableList<Account> Accounts { get; private set; }

    public AccountService(SettingService settingService, ILogger<AccountService> logger) {
        _logger = logger;
        _settingService = settingService;

        Accounts = [.. _settingService.Setting.Accounts];
        Accounts.CollectionChanged += Accounts_CollectionChanged;

        ActivateAccount(_settingService.Setting.ActiveAccount);
        _logger.LogInformation("初始化 {name}", nameof(AccountService));
    }

    private void Accounts_CollectionChanged(in NotifyCollectionChangedEventArgs<Account> e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                _settingService.Setting.Accounts.Add(e.NewItem);
                break;
            case NotifyCollectionChangedAction.Remove:
                _settingService.Setting.Accounts.Remove(e.OldItem);
                break;
        }
    }

    public bool RemoveAccount(Account account) => Accounts.Remove(account);

    public void AddAccount(Account account) {
        if (Accounts.Any(x => x.Uuid == account.Uuid))
            return;

        Accounts.Add(account);
    }

    public void ActivateAccount(Account account) {
        _logger.LogInformation("选择账户：{account} - {type}", account?.Name, account?.Type);

        if (account != null && !Accounts.Any(x => x.Uuid == account.Uuid))
            return;

        _settingService.Setting.ActiveAccount = ActiveAccount = account;
        WeakReferenceMessenger.Default.Send(new ActiveAccountChangedMessage());
    }
}