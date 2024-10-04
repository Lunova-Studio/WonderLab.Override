using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using DialogHostAvalonia;
using WonderLab.ViewModels;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using WonderLab.Views.Dialogs.Setting;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Views.Dialogs;
using WonderLab.Views.Dialogs.Multiplayer;
using Avalonia.Threading;
using System.Threading;

namespace WonderLab.Services.UI;

public sealed class DialogService {
    private readonly Dispatcher _dispatcher;
    private readonly WindowService _windowService;
    private readonly Dictionary<string, Func<object>> _dialogs = new() {
        { nameof(FileDropDialog), App.ServiceProvider.GetRequiredService<FileDropDialog> },
        { nameof(GameInstallDialog), App.ServiceProvider.GetRequiredService<GameInstallDialog> },
        { nameof(AccountDropDialog), App.ServiceProvider.GetRequiredService<AccountDropDialog> },
        { nameof(TestUserCheckDialog), App.ServiceProvider.GetRequiredService<TestUserCheckDialog> },
        { nameof(RecheckToOobeDialog), App.ServiceProvider.GetRequiredService<RecheckToOobeDialog> },
        { nameof(JoinMutilplayerDialog), App.ServiceProvider.GetRequiredService<JoinMutilplayerDialog> },
        { nameof(ChooseAccountTypeDialog), App.ServiceProvider.GetRequiredService<ChooseAccountTypeDialog> },
        { nameof(CreateMutilplayerDialog), App.ServiceProvider.GetRequiredService<CreateMutilplayerDialog> },
        { nameof(OfflineAuthenticateDialog), App.ServiceProvider.GetRequiredService<OfflineAuthenticateDialog> },
        { nameof(YggdrasilAuthenticateDialog), App.ServiceProvider.GetRequiredService<YggdrasilAuthenticateDialog> },
        { nameof(MicrosoftAuthenticateDialog), App.ServiceProvider.GetRequiredService<MicrosoftAuthenticateDialog> },
        { nameof(JoinMutilplayerRequestDialog) , App.ServiceProvider.GetRequiredService <JoinMutilplayerRequestDialog> },
    };

    public bool IsDialogOpen => DialogHost.IsDialogOpen("dialogHost");

    public DialogService(WindowService windowService, Dispatcher dispatcher) {
        _dispatcher = dispatcher;
        _windowService = windowService;
    }

    public async ValueTask<FileInfo> OpenFilePickerAsync(IEnumerable<FilePickerFileType> filters, string title) {
        var result = await _windowService.GetStorageProvider().OpenFilePickerAsync(new() {
            AllowMultiple = false,
            FileTypeFilter = filters.ToList(),
            Title = title
        });

        if (result is null || !result.Any()) {
            return null!;
        }

        return new(result.First().Path.LocalPath);
    }

    public async ValueTask<DirectoryInfo> OpenFolderPickerAsync(string title) {
        var result = await _windowService.GetStorageProvider().OpenFolderPickerAsync(new() {
            AllowMultiple = false,
            Title = title
        });

        if (result is null || !result.Any()) {
            return null;
        }

        return new(result[0].Path.LocalPath);
    }

    public async ValueTask<FileInfo> SaveFilePickerAsync(string title, string fileName) {
        var result = await _windowService.GetStorageProvider().SaveFilePickerAsync(new() {
            Title = title,
            SuggestedFileName = fileName,
        });

        if (result is null) {
            return null;
        }

        return new(result.Path.LocalPath);
    }

    public Task ShowContentDialog<TViewModel>() where TViewModel : DialogViewModelBase {
        if (DialogHost.IsDialogOpen("dialogHost")) {
            throw new InvalidOperationException("Open");
        }

        var viewName = typeof(TViewModel).Name.Replace("ViewModel", "");

        if (_dialogs.TryGetValue(viewName, out var contentFunc)) {
            var dialogObject = contentFunc() as UserControl;
            dialogObject!.DataContext = App.ServiceProvider!.GetRequiredService<TViewModel>();
            return DialogHost.Show(dialogObject, "dialogHost");
        }

        throw new InvalidOperationException("找不到这个对话框");
    }

    public async void ShowContentDialog<TViewModel>(object parameter) where TViewModel : DialogViewModelBase {
        if (DialogHost.IsDialogOpen("dialogHost")) {
            return;
        }

        var viewName = typeof(TViewModel).Name.Replace("ViewModel", "");

        if (_dialogs.TryGetValue(viewName, out var contentFunc)) {
            await _dispatcher.InvokeAsync(async () => {
                var dialogObject = contentFunc() as UserControl;
                dialogObject!.DataContext = App.ServiceProvider!.GetRequiredService<TViewModel>();
                (dialogObject.DataContext as DialogViewModelBase).Initialize(parameter);

                await DialogHost.Show(dialogObject, "dialogHost");
            });
        }
    }

    public void CloseContentDialog() {
        if (DialogHost.IsDialogOpen("dialogHost")) {
            _dispatcher.Invoke(() => DialogHost.Close("dialogHost"));
        }
    }
}