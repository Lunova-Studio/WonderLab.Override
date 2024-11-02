using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using System;
using WonderLab.Extensions;
using WonderLab.Infrastructure.Enums;

namespace WonderLab.Services.UI;

public sealed class ThemeService {
    private readonly ConfigService _configService;
    private readonly ILogger<ThemeService> _logger;

    public static readonly Lazy<Bitmap> OldMinecraftIcon = new("resm:WonderLab.Assets.Image.Icon.old_minecraft.png".ToBitmap());
    public static readonly Lazy<Bitmap> LoaderMinecraftIcon = new("resm:WonderLab.Assets.Image.Icon.loader_minecraft.png".ToBitmap());
    public static readonly Lazy<Bitmap> ReleaseMinecraftIcon = new("resm:WonderLab.Assets.Image.Icon.release_minecraft.png".ToBitmap());
    public static readonly Lazy<Bitmap> SnapshotMinecraftIcon = new("resm:WonderLab.Assets.Image.Icon.snapshot_minecraft.png".ToBitmap());

    public ThemeService(ConfigService configService, ILogger<ThemeService> logger) {
        _logger = logger;
        _configService = configService;
    }

    public void ApplyTheme(ThemeType themeType) {
        Application.Current.RequestedThemeVariant = themeType switch {
            ThemeType.Auto => ThemeVariant.Default,
            ThemeType.Dark => ThemeVariant.Dark,
            ThemeType.Light => ThemeVariant.Light,
            _ => ThemeVariant.Default,
        };
    }

    public void ApplyAccentColor(Color color) {
        Application.Current.Resources["NormalAccentBrush"] = color.ToBrush();

        Application.Current.Resources["DarkAccentBrush1"] =
            color.GetColorAfterLuminance(-0.15f).ToBrush();

        Application.Current.Resources["DarkAccentBrush2"] =
            color.GetColorAfterLuminance(-0.30f).ToBrush();

        Application.Current.Resources["LightAccentBrush1"] =
            color.GetColorAfterLuminance(0.15f).ToBrush();

        Application.Current.Resources["LightAccentBrush2"] =
            color.GetColorAfterLuminance(0.30f).ToBrush();
    }

    public void ApplyWindowEffect(BackgroundType backgroundType) {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
            Dispatcher.UIThread.Post(() => {
                var window = lifetime.MainWindow;

                switch (backgroundType) {
                    case BackgroundType.Mica:
                        window.Background = Brushes.Transparent;
                        window.TransparencyLevelHint = [WindowTransparencyLevel.Mica];
                        break;
                    case BackgroundType.Image:
                        if (string.IsNullOrEmpty(_configService.Entries.ActiveImagePath)) {
                            return;
                        }

                        window.Background = new ImageBrush {
                            Stretch = Stretch.UniformToFill,
                            Source = new Bitmap(_configService.Entries.ActiveImagePath),
                        };
                        break;
                    case BackgroundType.Colour:
                        window.Background = Application.Current.Resources["ColourBackground"] as LinearGradientBrush;
                        break;
                    case BackgroundType.AcrylicBlur:
                        window.Background = Brushes.Transparent;
                        window.TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
                        break;
                    default:
                        break;
                }
            }, DispatcherPriority.ApplicationIdle);
        }
    }
}