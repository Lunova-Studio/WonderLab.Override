using Avalonia;
using System;
using System.Runtime.InteropServices;

namespace WonderLab.Override {
    internal sealed class Program {
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

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
}