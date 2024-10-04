using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Models.Install;
using System.Collections.Generic;
using WonderLab.ViewModels.Dialogs.Download;

namespace WonderLab.Classes.Datas.ViewData;

public sealed partial class ModLoaderViewData : ObservableObject {
    private readonly GameInstallDialogViewModel _viewModel;

    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private bool _isCompatibly;
    [ObservableProperty] private bool _activeEntry;
    [ObservableProperty] private IEnumerable<object> _entrys;

    public LoaderType LoaderType { get; }
    public VersionManifestEntry Entry { get; }

    public bool IsEnabled => IsCompatibly;
    
    public ModLoaderViewData(LoaderType loaderType, VersionManifestEntry entry, GameInstallDialogViewModel viewModel) {
        Entry = entry;
        LoaderType = loaderType;

        _viewModel = viewModel;
    }

    [RelayCommand]
    private void Remove() => _viewModel.ActiveModLoaderData.Remove(ActiveEntry);
}