using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Models.Install;
using MinecraftLaunch.Components.Installer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Extensions;
using WonderLab.ViewModels.Dialogs.Download;

namespace WonderLab.Classes.Datas.ViewData;

public sealed partial class ModLoaderViewData : ObservableObject {
    private readonly GameInstallDialogViewModel _viewModel;

    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private bool _isCompatibly;
    [ObservableProperty] private bool _isLoaded = true;
    [ObservableProperty] private ModLoaderData _activeEntry;
    [ObservableProperty] private ObservableCollection<ModLoaderData> _entrys = [];

    public LoaderType LoaderType { get; }
    public VersionManifestEntry Entry { get; }

    public bool IsEnabled => IsCompatibly;

    public ModLoaderViewData(LoaderType loaderType, VersionManifestEntry entry, GameInstallDialogViewModel viewModel) {
        Entry = entry;
        LoaderType = loaderType;

        _viewModel = viewModel;

        _ = Task.Run(LoadInstallDatasAsync);
    }

    [RelayCommand]
    private void Remove() => _viewModel.ActiveModLoaderData.Remove(ActiveEntry);

    private async Task LoadInstallDatasAsync() {
        IEnumerable<ModLoaderData> modLoaderViewDatas = LoaderType switch {
            LoaderType.Quilt => (await QuiltInstaller.EnumerableFromVersionAsync(Entry.Id)).Select(x => new ModLoaderData(x.Loader.Version, x, LoaderType)),
            LoaderType.Fabric => (await FabricInstaller.EnumerableFromVersionAsync(Entry.Id)).Select(x => new ModLoaderData(x.Loader.Version, x, LoaderType)),
            LoaderType.OptiFine => (await OptifineInstaller.EnumerableFromVersionAsync(Entry.Id)).Select(x => new ModLoaderData($"{x.Type}_{x.Patch}", x, LoaderType)),
            LoaderType.Forge => (await ForgeInstaller.EnumerableFromVersionAsync(Entry.Id)).Select(x => 
                new ModLoaderData($"{x.ForgeVersion}{(string.IsNullOrEmpty(x.Branch) ? string.Empty : $"-{x.Branch}")}", x, LoaderType)),
            _ => null
        };

        Entrys = modLoaderViewDatas.ToObservableList();
        IsLoaded = Entrys is {
            Count: 0 
        };
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        if (e.PropertyName is nameof(ActiveEntry)) {
            _viewModel.ActiveModLoaderData.Add(ActiveEntry);
        }
    }
}