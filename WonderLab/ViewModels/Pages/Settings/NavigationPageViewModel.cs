using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Models;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Settings;

public sealed partial class NavigationPageViewModel : ViewModelBase {
    private readonly FileDialogService _fileDialogService;

    public static ObservableCollection<ListItemModel> ListItems { get; } = [
        new ListItemModel() { I18nKey = "Launch", PageKey = "Settings/Launch", Icon = MaterialIconKind.RocketLaunchOutline },
        new ListItemModel() { I18nKey = "Java Virtual Machine", PageKey = "Settings/Java", Icon = MaterialIconKind.CoffeeOutline },
        new ListItemModel() { I18nKey = "Appearance", PageKey = "Settings/Appearance", Icon = MaterialIconKind.PaletteOutline },
        new ListItemModel() { I18nKey = "Network", PageKey = "Settings/Network", Icon = MaterialIconKind.Web },
        new ListItemModel() { I18nKey = "Help & Support", PageKey = "Settings/Help", Icon = MaterialIconKind.HelpCircleOutline },
        new ListItemModel() { I18nKey = "About", PageKey = "Settings/About", Icon = MaterialIconKind.InfoCircleOutline },
    ];

    public NavigationPageViewModel(FileDialogService fileDialogService) {
        _fileDialogService = fileDialogService;
    }
}