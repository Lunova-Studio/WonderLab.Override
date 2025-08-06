using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class QuickImportPageViewModel : PageViewModelBase {
    private readonly DialogService _dialogService;

    public IEnumerable<ISettingImporter> Importers { get; }

    public QuickImportPageViewModel(DialogService dialogService, IEnumerable<ISettingImporter> importers) {
        _dialogService = dialogService;

        Importers = importers;
    }

    [RelayCommand]
    private Task OpenImportDialog(ISettingImporter settingImporter) => Dispatcher.UIThread.InvokeAsync(async () => {
        await _dialogService.ShowDialogAsync("QuickImport");
        WeakReferenceMessenger.Default.Send(new ImporterChangedMessage(settingImporter));
    });
}
