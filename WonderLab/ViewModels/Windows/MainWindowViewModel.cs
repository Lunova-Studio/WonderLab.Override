using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ObservableCollections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WonderLab.Models;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase {
    private readonly ObservableList<TabItemModel> _tabItems = [];
    private readonly List<TabItemModel> _mainTabItems = [
        new TabItemModel() { I18nKey="Game", PageKey = "Minecraft"},
        new TabItemModel() { I18nKey="Download", PageKey = "Download/Navigation" },
        new TabItemModel() { I18nKey="Community", PageKey = "Minecraft" },
        new TabItemModel() { I18nKey="Settings", PageKey = "Setting/Navigation" },
    ];

    [ObservableProperty] private TabItemModel _currentTab;

    public INotifyCollectionChangedSynchronizedViewList<TabItemModel> TabItems { get; }

    public MainWindowViewModel() {
        TabItems = _tabItems.ToNotifyCollectionChangedSlim();

        _tabItems.AddRange(_mainTabItems);
        CurrentTab = _tabItems[0];
    }
}