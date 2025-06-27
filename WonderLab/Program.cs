using Avalonia;
using System;
using System.IO;
using System.Runtime.InteropServices;
using WonderLab.Utilities;

namespace WonderLab;

internal sealed class Program {
    [STAThread]
    public static void Main(string[] args) {
        try {
            var app = BuildAvaloniaApp();
            app.StartWithClassicDesktopLifetime(args);
        } catch (Exception ex) {
            var localDirPath = PathUtil.GetLogsFolderPath();
            var crashesDirPath = Path.Combine(localDirPath, "crashes");

            Directory.CreateDirectory(crashesDirPath);
            File.WriteAllText(Path.Combine(crashesDirPath, $"log-{DateTime.Now:yyyyMMdd}.log"), ex.ToString());
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .With(new Win32PlatformOptions {
                WinUICompositionBackdropCornerRadius = 8,
                RenderingMode = RuntimeInformation.ProcessArchitecture == Architecture.Arm || RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                    ? [Win32RenderingMode.Wgl]
                    : [Win32RenderingMode.AngleEgl, Win32RenderingMode.Wgl, Win32RenderingMode.Software]!,
            }).With(new MacOSPlatformOptions {
                DisableAvaloniaAppDelegate = true,
                DisableDefaultApplicationMenuItems = true,
            }).With(new X11PlatformOptions {
                OverlayPopups = true,
            }).With(new SkiaOptions {
                MaxGpuResourceSizeBytes = null
            });
}