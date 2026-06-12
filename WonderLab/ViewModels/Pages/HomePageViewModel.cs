using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Base.Utilities;
using System;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ViewModelBase {
    public SettingsService Settings { get; }
    
    public HomePageViewModel(SettingsService settingsService) {
        Settings = settingsService;
    }
}