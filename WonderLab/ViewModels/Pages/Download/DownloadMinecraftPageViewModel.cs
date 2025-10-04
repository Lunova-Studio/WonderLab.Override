using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Base.Interfaces;
using MinecraftLaunch.Base.Models.Network;
using MinecraftLaunch.Components.Installer;
using ObservableCollections;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class DownloadMinecraftPageViewModel : PageViewModelBase {
    private string _versionId;

    private readonly DownloadService _downloadService;

    private readonly List<IInstallEntry> _allLoaderDatas = [];
    private readonly ObservableList<IInstallEntry> _loaderDatas = [];
    private readonly ObservableList<IInstallEntry> _selectedLoaderDatas = [];

    [ObservableProperty] private bool _isSupportForge = false;
    [ObservableProperty] private bool _isSupportQuilt = false;
    [ObservableProperty] private bool _isSupportFabric = false;
    [ObservableProperty] private bool _isSupportOptiFine = false;
    [ObservableProperty] private bool _isSupportNeoForge = false;

    [ObservableProperty] private bool _isForgeOptional = true;
    [ObservableProperty] private bool _isQuiltOptional = true;
    [ObservableProperty] private bool _isFabricOptional = true;
    [ObservableProperty] private bool _isOptiFineOptional = true;
    [ObservableProperty] private bool _isNeoForgeOptional = true;

    [ObservableProperty] private bool _isForgeLoaded;
    [ObservableProperty] private bool _isLoaderLoading;
    [ObservableProperty] private bool _isSelectedLoaderVisible;

    [ObservableProperty] private string _minecraftId;

    [ObservableProperty] private IInstallEntry _selectedLoader;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ReleaseTime))]
    [NotifyPropertyChangedFor(nameof(VersionType))]
    private VersionManifestEntry _minecraftData;

    public string VersionType => MinecraftData?.Type;
    public DateTime ReleaseTime => MinecraftData?.ReleaseTime ?? DateTime.MinValue;

    public INotifyCollectionChangedSynchronizedViewList<IInstallEntry> Loaders { get; }
    public INotifyCollectionChangedSynchronizedViewList<IInstallEntry> SelectedLoaders { get; }

    public DownloadMinecraftPageViewModel(DownloadService downloadService) {
        _downloadService = downloadService;

        WeakReferenceMessenger.Default.Register<MinecraftResponseMessage>(this, (_, args) => {
            if (string.IsNullOrEmpty(args.MinecraftId))
                return;

            MinecraftData = args.MinecraftData;
            MinecraftId = _versionId = args.MinecraftId;
        });

        IsForgeLoaded = false;
        IsLoaderLoading = true;
        Loaders = _loaderDatas.ToNotifyCollectionChangedSlim();
        SelectedLoaders = _selectedLoaderDatas.ToNotifyCollectionChangedSlim();

        SelectedLoaders.CollectionChanged += (_, _) => {
            IsSelectedLoaderVisible = _selectedLoaderDatas.Count > 0;
            MinecraftId = _versionId + (_selectedLoaderDatas.Count > 0 ? "-" + string.Join("-", _selectedLoaderDatas.Select(x => $"{x.ModLoaderType}_{x.DisplayVersion}")) : "");
        };
    }

    [RelayCommand]
    private void ChangeActiveLoader(string type) {
        IsLoaderLoading = true;

        var loaderType = type switch {
            "Forge" => ModLoaderType.Forge,
            "Quilt" => ModLoaderType.Quilt,
            "Fabric" => ModLoaderType.Fabric,
            "Optifine" => ModLoaderType.OptiFine,
            "Neoforge" => ModLoaderType.NeoForge,
            _ => throw new NotSupportedException($"Unsupported loader type: {type}"),
        };

        _loaderDatas?.Clear();
        _loaderDatas?.AddRange(_allLoaderDatas?.Where(x => x.ModLoaderType == loaderType));
        var loader = SelectedLoaders.FirstOrDefault(x => x.ModLoaderType == loaderType);

        SelectedLoader = loader;
        IsLoaderLoading = false;
    }

    [RelayCommand]
    private void RemoveLoader(IInstallEntry entry) {
        _selectedLoaderDatas.Remove(entry);

        if (_selectedLoaderDatas.Count is 0)
            IsForgeOptional = IsNeoForgeOptional = IsFabricOptional = IsQuiltOptional = IsOptiFineOptional = true;
        else
            ChangeOtherLoaderState(_selectedLoaderDatas.Last());

        if (entry.ModLoaderType == SelectedLoader?.ModLoaderType)
            SelectedLoader = null;
    }

    [RelayCommand]
    private Task Install() => Task.Run(async () => {
        List<IInstallEntry> entries = [.. _selectedLoaderDatas];
        entries.Add(MinecraftData);
        await _downloadService.InstallMinecraftTaskAsync(entries, MinecraftId);
    });

    private void ChangeOtherLoaderState(IInstallEntry entry) {
        if (entry.ModLoaderType is ModLoaderType.Forge) {
            IsOptiFineOptional = true;
            IsFabricOptional = IsQuiltOptional = IsNeoForgeOptional = false;
        } else if (entry.ModLoaderType is ModLoaderType.OptiFine) {
            IsForgeOptional = true;
            IsFabricOptional = IsQuiltOptional = IsNeoForgeOptional = false;
        } else if (entry.ModLoaderType is ModLoaderType.NeoForge)
            IsForgeOptional = IsFabricOptional = IsQuiltOptional = IsOptiFineOptional = false;
        else if (entry.ModLoaderType is ModLoaderType.Fabric)
            IsForgeOptional = IsOptiFineOptional = IsQuiltOptional = IsNeoForgeOptional = false;
        else if (entry.ModLoaderType is ModLoaderType.Quilt)
            IsForgeOptional = IsOptiFineOptional = IsFabricOptional = IsNeoForgeOptional = false;
    }

    protected override void OnNavigated() {
        var loaderActions = new Dictionary<ModLoaderType, Func<CancellationToken, Task<IEnumerable<object>>>> {
            [ModLoaderType.Forge] = async ct => await ForgeInstaller.EnumerableForgeAsync(MinecraftId, false, ct),
            [ModLoaderType.Quilt] = async ct => await QuiltInstaller.EnumerableQuiltAsync(MinecraftId, ct),
            [ModLoaderType.Fabric] = async ct => await FabricInstaller.EnumerableFabricAsync(MinecraftId, ct),
            [ModLoaderType.OptiFine] = async ct => await OptifineInstaller.EnumerableOptifineAsync(MinecraftId, ct),
            [ModLoaderType.NeoForge] = async ct => await ForgeInstaller.EnumerableForgeAsync(MinecraftId, true, ct),
        }.ToFrozenDictionary();

        _loaderDatas?.Clear();
        foreach (var (loader, fetchFunc) in loaderActions) {
            Task.Run(async () => {
                try {
                    var data = (await fetchFunc(CancellationTokenSource.Token))
                        .OfType<IInstallEntry>()
                        .ToArray();

                    if (data.Length is 0)
                        return;

                    _allLoaderDatas.AddRange(data);
                    switch (loader) {
                        case ModLoaderType.Forge:
                            IsForgeLoaded = true;
                            IsSupportForge = true;
                            IsLoaderLoading = false;

                            _loaderDatas.AddRange(data);
                            break;
                        case ModLoaderType.Quilt:
                            IsSupportQuilt = true;
                            break;
                        case ModLoaderType.Fabric:
                            IsSupportFabric = true;
                            break;
                        case ModLoaderType.OptiFine:
                            IsSupportOptiFine = true;
                            break;
                        case ModLoaderType.NeoForge:
                            IsSupportNeoForge = true;
                            break;
                    }
                } catch (Exception) { }
            });
        }
    }

    partial void OnSelectedLoaderChanged(IInstallEntry oldValue, IInstallEntry newValue) {
        if (newValue is null)
            return;

        var removeValue = oldValue is null ? newValue : oldValue;
        var index = _selectedLoaderDatas.IndexOf(removeValue);

        _selectedLoaderDatas.Remove(removeValue);
        if (index is -1)
            _selectedLoaderDatas.Add(newValue);

        else
            _selectedLoaderDatas.Insert(index, newValue);

        ChangeOtherLoaderState(newValue);
    }
}