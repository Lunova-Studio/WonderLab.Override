using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Utilities;
using Monet.Shared.Enums;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WonderLab.Classes.Enums;
using WonderLab.Controls;
using WonderLab.Extensions;

namespace WonderLab.Services;

public sealed class ThemeService {
    private readonly SettingService _settingService;
    private readonly ILogger<ThemeService> _logger;

    private WonderWindow _hostWindow;

    public static readonly Lazy<Bitmap> LoadingIcon = new("resm:WonderLab.Assets.Images.doro_loading.jpg".ToBitmap());
    public static readonly Lazy<Bitmap> ReleaseMinecraftIcon = new("resm:WonderLab.Assets.Images.Icons.release_minecraft.png".ToBitmap());

    public ObservableCollection<BackgroundType> BackgroundTypes { get; } = [
        BackgroundType.SolidColor,
        BackgroundType.Bitmap,
        BackgroundType.Voronoi,
        BackgroundType.Bubble
    ];

    public FrozenDictionary<uint, string> MonetColors => new Dictionary<uint, string>() {
        [Colors.Red.ToUInt32()] = "Red",
        [Colors.Orange.ToUInt32()] = "Orange",
        [Colors.Yellow.ToUInt32()] = "Yellow",
        [Colors.Green.ToUInt32()] = "Green",
        [Colors.Cyan.ToUInt32()] = "Cyan",
        [Colors.Blue.ToUInt32()] = "Blue",
        [Colors.Purple.ToUInt32()] = "Purple",
        [Colors.Pink.ToUInt32()] = "Pink",
    }.ToFrozenDictionary();

    public event EventHandler BackgroundTypeChanged;

    public ThemeService(SettingService settingService, ILogger<ThemeService> logger) {
        _logger = logger;
        _settingService = settingService;
    }

    public void Initialize(WonderWindow window) {
        _hostWindow = window;

        if (EnvironmentUtil.IsMac) {
            BackgroundTypes.Add(BackgroundType.Acrylic);
        } else if (EnvironmentUtil.IsWindow) {
            BackgroundTypes.Add(BackgroundType.Mica);
            BackgroundTypes.Add(BackgroundType.Acrylic);
        }

        Dispatcher.UIThread.Post(() => {
            if (_settingService.Setting.IsEnableSystemColor)
                UseSystemColor();
            else if (_settingService.Setting.ActiveColor is 0)
                _settingService.Setting.ActiveColor = Colors.Red.ToUInt32();

            if(!_settingService.Setting.IsEnableSystemColor)
                UpdateColorScheme(_settingService.Setting.ActiveColorVariant);

            UpdateThemeVariant(_settingService.Setting.ActiveTheme);
            UpdateBackgroundType(_settingService.Setting.ActiveBackground, _settingService.Setting.ImagePath);
        });
    }

    public void UseSystemColor() {
        UpdateColorScheme(_settingService.Setting.ActiveColorVariant,
            Application.Current.PlatformSettings.GetColorValues().AccentColor1.ToUInt32());
    }

    public void UpdateColorScheme(Variant variant, uint? color = null) {
        _logger.LogInformation("切换动态颜色变种，类别：{variant}", variant.ToString());
        App.Monet.BuildScheme(variant, Color.FromUInt32(color ?? _settingService.Setting.ActiveColor), 0);
    }

    public void UpdateThemeVariant(ThemeType type) {
        var variant = type switch {
            ThemeType.Dark => ThemeVariant.Dark,
            ThemeType.Light => ThemeVariant.Light,
            ThemeType.Auto => ThemeVariant.Default,
            _ => ThemeVariant.Light,
        };

        Application.Current.RequestedThemeVariant = variant;
    }

    public void UpdateThemeVariant(ThemeVariant variant, Variant colorVariant = Variant.Tonal_Spot) {
        _logger.LogInformation("切换程序主题，类别：{variant}", variant.ToString());

        Application.Current.RequestedThemeVariant = variant;

        App.Monet.IsDarkMode = variant == ThemeVariant.Dark;
        App.Monet.BuildScheme(colorVariant, Color.FromUInt32(_settingService.Setting.ActiveColor), 0);
    }

    public void UpdateBackgroundType(BackgroundType type, string imagePath = default) {
        _logger.LogInformation("切换程序背景主题，类别：{type}", type);

        if (!string.IsNullOrEmpty(imagePath)) {
            _hostWindow.ImagePath = imagePath;
        }

        _hostWindow.BackgroundType = type;
        BackgroundTypeChanged?.Invoke(this, EventArgs.Empty);
    }
}