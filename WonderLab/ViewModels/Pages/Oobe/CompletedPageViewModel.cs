using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class CompletedPageViewModel : PageViewModelBase {
    private readonly SettingService _settingService;

    [ObservableProperty] private string _lottiePath;

    public CompletedPageViewModel(SettingService settingService) {
        _settingService = settingService;
    }

    protected override void OnNavigated() {
        LottiePath = "resm:WonderLab.Assets.Lotties.OOBE_Confetti.json";
    }

    [RelayCommand]
    private void Restart() {
        _settingService.Setting.IsCompletedOOBE = true;
        _settingService.Save();

        Process.Start(Environment.ProcessPath).Dispose();
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            lifetime.Shutdown();
    }
}