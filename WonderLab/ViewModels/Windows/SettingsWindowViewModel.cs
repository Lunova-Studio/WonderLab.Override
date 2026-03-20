using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WonderLab.Models;
using WonderLab.Services;

namespace WonderLab.ViewModels.Windows;

public sealed partial class SettingsWindowViewModel : ViewModelBase {
    private readonly FileDialogService _fileDialogService;

    [ObservableProperty] private ListItemModel _activePage;

    public SettingsWindowViewModel(FileDialogService fileDialogService) {
        _fileDialogService = fileDialogService;
    }
}
