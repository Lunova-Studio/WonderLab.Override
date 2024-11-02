using Avalonia;
using System;
using System.Runtime.InteropServices;
using WonderLab.Extensions;

namespace WonderLab.Desktop;

public static class Program {
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .WithFont()
            .With(new Win32PlatformOptions {
                RenderingMode = RuntimeInformation.ProcessArchitecture == Architecture.Arm || RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                    ? [Win32RenderingMode.Wgl]
                    : [Win32RenderingMode.AngleEgl, Win32RenderingMode.Software]!,
            })
            .With(new MacOSPlatformOptions {
                DisableAvaloniaAppDelegate = true,
                DisableDefaultApplicationMenuItems = true,
            })
            .With(new X11PlatformOptions {
                OverlayPopups = true,
            }).With(new SkiaOptions {
                MaxGpuResourceSizeBytes = 1073741824L
            });
}