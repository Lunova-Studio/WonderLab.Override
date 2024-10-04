using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Classes.Enums;
using System.Collections.Generic;
using WonderLab.Classes.Datas.ViewData;

namespace WonderLab.ViewModels.Dialogs.Download;
public sealed partial class GameInstallDialogViewModel : DialogViewModelBase {
    [ObservableProperty] private MinecraftViewData _minecraft;
    [ObservableProperty] private List<object> _activeModLoaderData;
    [ObservableProperty] private List<ModLoaderViewData> _modLoaderDatas;

    public GameInstallDialogViewModel() {
        Initialized += OnInitialized;
    }

    private void OnInitialized(object sender, System.EventArgs e) {
        Minecraft = Parameter as MinecraftViewData;

        ModLoaderDatas = [
            new(LoaderType.OptiFine, Minecraft.Minecraft, this),
            new(LoaderType.Fabric, Minecraft.Minecraft, this),
            new(LoaderType.Forge, Minecraft.Minecraft, this),
            new(LoaderType.Quilt, Minecraft.Minecraft, this),
        ];
    }
}