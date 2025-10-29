using Avalonia.Threading;
using DialogHostAvalonia;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Extensions.Hosting.UI;

namespace WonderLab.Services;

public sealed class DialogService {
    private const string MAIN_DIALOG_IDENTIFIER = "Host";
    private readonly AvaloniaDialogProvider _dialogProvider;

    public DialogService(AvaloniaDialogProvider dialogProvider) {
        _dialogProvider = dialogProvider;
    }

    public async Task CloseDialogAsync(CancellationToken cancellationToken = default) {
        if (DialogHost.IsDialogOpen(MAIN_DIALOG_IDENTIFIER)) {
            DialogHost.Close(MAIN_DIALOG_IDENTIFIER);
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);//wait close animation complete.
        }
    }

    public async Task ShowDialogAsync(string key) {
        if (DialogHost.IsDialogOpen(MAIN_DIALOG_IDENTIFIER))
            await CloseDialogAsync();

        var dialog = await Dispatcher.UIThread
            .InvokeAsync(() => _dialogProvider.GetDialog(key));

        _ = DialogHost.Show(dialog, MAIN_DIALOG_IDENTIFIER);
    }
}