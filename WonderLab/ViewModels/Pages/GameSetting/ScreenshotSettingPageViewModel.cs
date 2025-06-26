using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Services;
using WonderLab.Services.Launch;

namespace WonderLab.ViewModels.Pages.GameSetting;

public sealed partial class ScreenshotSettingPageViewModel : DynamicPageViewModelBase {
    private readonly GameService _gameService;
    private readonly SettingService _settingService;
    private readonly ILogger<ScreenshotSettingPageViewModel> _logger;

    [ObservableProperty] private bool _hasScreenshots;
    [ObservableProperty] private ObservableCollection<KeyValuePair<string, IEnumerable<string>>> _screenshots;

    public ScreenshotSettingPageViewModel(GameService gameService, SettingService settingService, ILogger<ScreenshotSettingPageViewModel> logger) {
        _gameService = gameService;
        _settingService = settingService;
        _logger = logger;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(() => {
        bool isEnableIndependency;
        if (_gameService.TryGetMinecraftProfile(out var profile) && profile.IsEnableSpecificSetting)
            isEnableIndependency = profile.IsEnableIndependency;
        else
            isEnableIndependency = _settingService.Setting.IsEnableIndependency;

        var workingPath = _gameService.ActiveGameCache.ToWorkingPath(isEnableIndependency);
        var screenshots = Directory.EnumerateFiles(Path.Combine(workingPath, "screenshots"))
            .GroupBy(x => File.GetCreationTime(x).ToString("yyyy/M/d"))
            .ToDictionary(x => x.Key, x => x.AsEnumerable());

        Screenshots = new(screenshots);
        HasScreenshots = Screenshots.Count > 0;

        _logger.LogInformation("Loaded {count} screenshots", Screenshots.Count);
    });
}