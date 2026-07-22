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
using WonderLab.Services;
using WonderLab.Utilities;
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
        Monet.BuildScheme(Variant.TonalSpot, Color.Parse("#ffde3f")); // #769cdf 4294958655
    }

    public override void RegisterServices() {
        base.RegisterServices();

        _ = ConfigureIoC().RunAsync();
    }
    
    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.Exit += OnExit;
            desktop.Startup += OnStartup;
            
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            var mainWindowVM = ServiceProvider.GetRequiredService<MainWindowViewModel>();

            mainWindow.DataContext = mainWindowVM;
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static async void OnExit(object sender, ControlledApplicationLifetimeExitEventArgs e) {
        var logger = ServiceProvider.GetService<ILogger<Application>>();
        
        try {
            await ServiceProvider.GetService<SettingsService>()
                .SaveAsync();
        }
        catch (Exception ex) {
            logger.LogError(ex, "An error occured during saving");
        }
    }

    private static async void OnStartup(object sender, ControlledApplicationLifetimeStartupEventArgs e) {
        var logger = ServiceProvider.GetService<ILogger<Application>>();
        
        try {
            await ServiceProvider.GetService<SettingsService>()
                .LoadAsync();
        }
        catch (Exception ex) {
            logger.LogError(ex, "An error occured during loading");
        }
    }
    
    private static IHost ConfigureIoC() {
        var builder = new AvaloniaHostBuilder();
        var services = builder.Services;
        
        // Logger
        services.AddLogging(configure => {
            configure.AddZLoggerConsole();
            configure.AddZLoggerRollingFile(logConfigure => {
                logConfigure.FilePathSelector = (dt, index) => Path.Combine(PathUtil.GetLogsDirectory(), $"{dt:yyyy-MM-dd}_{index}.log");
                logConfigure.UsePlainTextFormatter(formatter => {
                    formatter.SetPrefixFormatter($"[{0}] [{1:short}] ({2}): ", (in template, in info) => template.Format(info.Timestamp, info.LogLevel, info.Category));
                    formatter.SetExceptionFormatter((writer, ex) => Utf8StringInterpolation.Utf8String.Format(writer, $"{ex.Message}"));
                });
            });

            configure.SetMinimumLevel(LogLevel.Debug);
        });

        // Service
        services.AddSingleton<DialogService>();
        services.AddSingleton<SettingsService>();
        
        // View
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        var pages = builder.PageProvider;
        pages.Register<HomePage, HomePageViewModel>();
        pages.Register<MinecraftPage, MinecraftPageViewModel>();

        // Settings
        pages.Register<AboutPage, AboutPageViewModel>();
        pages.Register<NavigationPage, NavigationPageViewModel>();
        pages.Register<JavaSettingsPage, JavaSettingsPageViewModel>();
        pages.Register<LaunchSettingsPage, LaunchSettingsPageViewModel>();
        pages.Register<AccountSettingsPage, AccountSettingsPageViewModel>();
        pages.Register<NetworkSettingsPage, NetworkSettingsPageViewModel>();
        pages.Register<AppearanceSettingsPage, AppearanceSettingsPageViewModel>();
        
        var appHost = builder.Build();
        ServiceProvider = appHost.Services;

        return appHost;
    }
}