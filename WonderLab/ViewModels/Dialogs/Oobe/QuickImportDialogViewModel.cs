using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.IO;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services;
using WonderLab.Services.Authentication;

namespace WonderLab.ViewModels.Dialogs.Oobe;

public sealed partial class QuickImportDialogViewModel : DialogViewModelBase {
    private static FilePickerFileType Launcher { get; } = new FilePickerFileType("Launcher") {
        Patterns = ["*.*"],
        MimeTypes = ["*/*"],
        AppleUniformTypeIdentifiers = ["public.item"]
    };

    private readonly AccountService _accountService;
    private readonly SettingService _settingService;

    private ISettingImporter _settingImporter;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsImportButtonEnabled))]
    private string _launcherPath;

    public bool IsImportButtonEnabled => !string.IsNullOrWhiteSpace(LauncherPath) && File.Exists(LauncherPath);

    public QuickImportDialogViewModel(SettingService settingService, AccountService accountService) {
        _settingService = settingService;
        _accountService = accountService;

        WeakReferenceMessenger.Default.Register<ImporterChangedMessage>(this, (_, arg) => {
            _settingImporter = arg.SettingImporter;
        });
    }

    [RelayCommand]
    private async Task Import() => await Task.Run(async () => {
        var (Settings, IsSuccess) = await _settingImporter.ImportAsync();
        if (IsSuccess) {
            _accountService.Accounts.Clear();

            foreach (var account in Settings.Accounts)
                _accountService.AddAccount(account);

            _settingService.Setting = Settings;
            WeakReferenceMessenger.Default.Send(new PageNotificationMessage("OOBE/ChooseMinecraft"));
        }

        CloseCommand?.Execute(default);
    }).ConfigureAwait(false);

    [RelayCommand]
    private Task BrowserLauncher() => Dispatcher.UIThread.InvokeAsync(async () => {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
            var result = await lifetime.MainWindow.StorageProvider.OpenFilePickerAsync(new() {
                AllowMultiple = false,
                FileTypeFilter = [Launcher]
            });

            if (result is { Count: > 0 })
                LauncherPath = result[0].Path.LocalPath;
        }
    });

    partial void OnLauncherPathChanged(string value) {
        _settingImporter.LauncherPath = value;
    }
}
