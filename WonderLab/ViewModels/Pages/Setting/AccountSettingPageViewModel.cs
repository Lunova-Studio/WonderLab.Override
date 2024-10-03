using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Datas.MessageData;
using WonderLab.Classes.Datas.TaskData;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Extensions;
using WonderLab.Services;
using WonderLab.Services.Auxiliary;
using WonderLab.Services.UI;
using WonderLab.ViewModels.Dialogs.Setting;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class AccountSettingPageViewModel : ViewModelBase {
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;
    private readonly NotificationService _notificationService;

    [ObservableProperty] private AccountViewData _activeAccount;
    [ObservableProperty] private ObservableCollection<AccountViewData> _accounts = [];

    public AccountSettingPageViewModel(
        DialogService dialogService,
        AccountService accountService,
        SettingService settingService,
        NotificationService notificationService,
        TaskService taskService) {
        _dialogService = dialogService;
        _settingService = settingService;
        _notificationService = notificationService;

        if (_settingService.Data.Accounts.Count != 0) {
            RunBackgroundWork(() => {
                var list =  accountService.InitializeAccountsAsync()
                                          .ToBlockingEnumerable()
                                          .ToObservableList();

                Accounts.Load(list);
            });
        }

        WeakReferenceMessenger.Default.Register<AccountMessage>(this, AccountHandle);
        WeakReferenceMessenger.Default.Register<AccountChangeNotificationMessage>(this, AccountChangeHandle);
    }

    [RelayCommand]
    private void OpenDialog() {
        _dialogService.ShowContentDialog<ChooseAccountTypeDialogViewModel>();
    }

    partial void OnActiveAccountChanged(AccountViewData value) {
        _settingService.Data.ActiveAccount = value?.Account;
    }

    private void AccountHandle(object obj, AccountMessage accountMessage) {
        RunBackgroundWork(async () => {
            foreach (var item in accountMessage.Accounts.Select(x => new AccountViewData(x))) {
                Accounts.Add(item);
                await Task.Delay(5);
            }
        });
    }

    private void AccountViewHandle(object obj, AccountViewMessage accountMessage) {
        RunBackgroundWork(async () => {
            foreach (var item in accountMessage.Accounts) {
                Accounts.Add(item);
                await Task.Delay(5);
            }
        });
    }

    private void AccountChangeHandle(object obj, AccountChangeNotificationMessage accountMessage) {
        if (Accounts.Remove(accountMessage.Account)) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "Success",
                Content = $"成功移除 {accountMessage.Account.Account.Name} 账户",
                NotificationType = NotificationType.Success
            });
        }
    }
}