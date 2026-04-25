using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Monet.Avalonia;
using Monet.Shared.Enums;
using System;
using System.IO;
using WonderLab.Override.Utilities;
using WonderLab.Services.Navigation;
using WonderLab.ViewModels.Pages;
using WonderLab.ViewModels.Pages.Settings;
using WonderLab.ViewModels.Windows;
using WonderLab.Views.Pages.Settings;
using WonderLab.Views.Windows;
using ZLogger;

namespace WonderLab;

public partial class App : Application {
    public static MonetColors Monet { get; private set; }
    public static IServiceProvider ServiceProvider { get; private set; }

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
        
        Monet = (Styles[0] as MonetColors)!;
        Monet.BuildScheme(Variant.Content, Colors.Green);
    }

    public override void RegisterServices() {
        base.RegisterServices();

        _ = ConfigureIoC().RunAsync();
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            var mainWindowVM = ServiceProvider.GetRequiredService<MainWindowViewModel>();

            mainWindow.DataContext = mainWindowVM;
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IHost ConfigureIoC() {
        var builder = new AvaloniaHostBuilder();
        var services = builder.Services;
        
        //Logger
        services.AddLogging(configure => {
            configure.AddZLoggerConsole();
            configure.AddZLoggerRollingFile(logConfigure => {
                logConfigure.FilePathSelector = (dt, index) => Path.Combine(PathUtil.GetLogsFolderPath(), "logs", $"{dt:yyyy-MM-dd}_{index}.log");
                logConfigure.UsePlainTextFormatter(formatter => {
                    formatter.SetPrefixFormatter($"[{0}] [{1:short}] ({2}): ", (in template, in info) => template.Format(info.Timestamp, info.LogLevel, info.Category));
                    formatter.SetExceptionFormatter((writer, ex) => Utf8StringInterpolation.Utf8String.Format(writer, $"{ex.Message}"));
                });
            });

            configure.SetMinimumLevel(LogLevel.Debug);
        });

        //View
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        var pages = builder.PageProvider;
        pages.Register<HomePage, HomePageViewModel>();
        pages.Register<MinecraftPage, MinecraftPageViewModel>();
        pages.Register<NavigationPage, NavigationPageViewModel>();

        var appHost = builder.Build();
        ServiceProvider = appHost.Services;

        return appHost;
    }
}