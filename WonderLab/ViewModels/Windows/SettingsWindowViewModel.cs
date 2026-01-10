using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WonderLab.Models;
using WonderLab.Services;

namespace WonderLab.ViewModels.Windows;

public sealed partial class SettingsWindowViewModel : ViewModelBase {
    private readonly FileDialogService _fileDialogService;

    [ObservableProperty] private ListItemModel _activePage;

    public ObservableCollection<ListItemModel> ListItems { get; } = [
        new ListItemModel() { I18nKey = "Launch", PageKey = "Settings/Launch", Icon = "\uE945" },
        new ListItemModel() { I18nKey = "Java Virtual Machine", PageKey = "Settings/Java", Icon = "\uEC32" },
        new ListItemModel() { I18nKey = "Appearance", PageKey = "Settings/Launch", Icon = "\uE790" },
        new ListItemModel() { I18nKey = "Network", PageKey = "Settings/Launch", Icon = "\uE774" },
        new ListItemModel() { I18nKey = "Help & Support", PageKey = "Settings/Launch", Icon = "\uE95B" },
        new ListItemModel() { I18nKey = "About", PageKey = "Settings/Launch", Icon = "\uE946" },
    ];

    public SettingsWindowViewModel(FileDialogService fileDialogService) {
        _fileDialogService = fileDialogService;
        ActivePage = ListItems[0];
    }
}
