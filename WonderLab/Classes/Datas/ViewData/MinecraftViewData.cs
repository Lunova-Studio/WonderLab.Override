using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Classes.Models.Install;
using WonderLab.Services.UI;
using WonderLab.ViewModels.Dialogs.Download;

namespace WonderLab.Classes.Datas.ViewData;

public sealed partial class MinecraftViewData : ObservableObject {
    public VersionManifestEntry Minecraft { get; }

    public MinecraftViewData(VersionManifestEntry versionManifestEntry) {
        Minecraft = versionManifestEntry;
    }

    [RelayCommand]
    private void NavigateToDialog() {
        var service = App.GetService<DialogService>();
        service.ShowContentDialog<GameInstallDialogViewModel>(this);
    }
}