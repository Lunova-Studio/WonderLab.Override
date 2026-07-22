using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Base.Utilities;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ViewModelBase {
    private readonly DialogService _dialogService;
    
    public SettingsService Settings { get; }
    
    public HomePageViewModel(SettingsService settingsService, DialogService dialogService) {
        Settings = settingsService;
        
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async Task Show() {
        await _dialogService.ShowDialogAsync();
    }
}