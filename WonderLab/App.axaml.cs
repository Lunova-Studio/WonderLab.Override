using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Components.Parser;
using Monet.Avalonia;
using Monet.Shared.Enums;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Processors;
using WonderLab.Controls;
using WonderLab.Extensions.Hosting;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Services;
using WonderLab.Services.Game;
using WonderLab.Services.UI;
using WonderLab.Utilities;
using WonderLab.ViewModels.Pages;
using WonderLab.ViewModels.Pages.Download;
using WonderLab.ViewModels.Pages.GameSetting;
using WonderLab.ViewModels.Pages.Settings;
using WonderLab.ViewModels.Windows;
using WonderLab.Views.Pages;
using WonderLab.Views.Pages.Download;
using WonderLab.Views.Pages.GameSetting;

namespace WonderLab;

public partial class App : Application {
    private const string LOG_OUTPUT_TEMPLATE = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] ({SourceContext}): {Message:lj}{NewLine}{Exception}";

    private IHost _appHost;

    public static MonetColors Monet { get; private set; }
    public static IServiceProvider ServiceProvider { get; private set; }
    public static Color SystemColor => Current.PlatformSettings.GetColorValues().AccentColor1;

    public static TKey Get<TKey>() where TKey : class {
        return ServiceProvider.GetRequiredService<TKey>();
    }

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
        Monet = (Styles[0] as MonetColors)!;
        MinecraftParser.DataProcessors.Add("Favorites", new MinecraftFavoritesProcessor());

#if DEBUG
        this.AttachDeveloperTools();
#endif

        Monet.BuildScheme(Variant.TonalSpot, Colors.Blue);
    }

    public override void OnFrameworkInitializationCompleted() {
        DisableAvaloniaDataAnnotationValidation();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            //desktop.MainWindow = new MainWindow {
            //    DataContext = new MainWindowViewModel()
            //};

            desktop.MainWindow = new TestWindow {
                DataContext = new MainWindowViewModel()
            };

            _appHost = ConfigureIoC(out var host, desktop.MainWindow);
            _ = _appHost.RunAsync();

            ServiceProvider = host.Services;
            Frame.PageProvider = ServiceProvider.GetRequiredService<AvaloniaPageProvider>();

            Get<SettingService>().Initialize();
        }

        base.OnFrameworkInitializationCompleted();
    }

    #region Privates

    private static IHost ConfigureIoC(out IHost host, Window window) {
        var builder = new AvaloniaHostBuilder();

        //Window VM
        builder.Services.AddSingleton<SettingsWindowViewModel>();

        //Configure Services
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<SettingService>();
        builder.Services.AddSingleton<MinecraftService>();

        builder.Services.AddSingleton(x => new FileDialogService(window));

        //Configure Pages
        var pageProvider = builder.PageProvider;
        pageProvider.AddPage<HomePage, HomePageViewModel>("Home");
        pageProvider.AddPage<MinecraftPage, MinecraftPageViewModel>("Minecraft");

        //Settings
        pageProvider.AddPage<HelpPage>("Settings/Help");
        pageProvider.AddPage<AboutPage, AboutPageViewModel>("Settings/About");
        pageProvider.AddPage<NetworkPage, NetworkPageViewModel>("Settings/Network");
        pageProvider.AddPage<JavaSettingsPage, JavaSettingsPageViewModel>("Settings/Java");
        pageProvider.AddPage<NavigationPage, NavigationPageViewModel>("Settings/Navigation");
        pageProvider.AddPage<AppearancePage, AppearancePageViewModel>("Settings/Appearance");
        pageProvider.AddPage<LaunchSettingsPage, LaunchSettingsPageViewModel>("Settings/Launch");

        //Download
        pageProvider.AddPage<DownloadNavigationPage, DownloadNavigationPageViewModel>("Download/Navigation");

        //GameSetting
        pageProvider.AddPage<SettingPage, SettingPageViewModel>("GameSetting/Setting");
        pageProvider.AddPage<MinecraftGalleryPage, MinecraftGalleryPageViewModel>("GameSetting/Gallery");

        //Configure Logging
        Log.Logger = new LoggerConfiguration().WriteTo
            .File(Path.Combine(PathUtil.GetLogsFolderPath(), "logs", $"WonderLog.log"), rollingInterval: RollingInterval.Day, outputTemplate: LOG_OUTPUT_TEMPLATE)
            .CreateLogger();

        builder.Logging.AddSerilog(Log.Logger);
        return host = builder.Build();
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        var logger = ServiceProvider.GetRequiredService<ILogger<Application>>();
        if (e.Exception is Exception ex)
            logger.LogError(ex, "An unhandled UI thread exception occurred: {Message}", ex.Message);

        e.Handled = true;
    }

    private async void OnExit(object sender, ControlledApplicationLifetimeExitEventArgs e) {
        var logger = Get<ILogger<Application>>();
        logger.LogInformation("Exiting, exitcode is {exitCode}", e.ApplicationExitCode);

        await _appHost.StopAsync();
        Get<SettingService>().Save();
    }

    private void OnStartup(object sender, ControlledApplicationLifetimeStartupEventArgs e) {
        ActualThemeVariantChanged += OnActualThemeVariantChanged;
        PlatformSettings.ColorValuesChanged += OnColorValuesChanged;
        Dispatcher.UIThread.UnhandledException += OnUnhandledException;
    }

    private void OnColorValuesChanged(object sender, Avalonia.Platform.PlatformColorValues e) {
        //var settings = Get<SettingService>().Setting;
        //if (settings.IsEnableSystemColor)
        //    Get<ThemeService>().UpdateColorScheme(settings.ActiveColorVariant,
        //        settings.ActiveColor = e.AccentColor1.ToUInt32());
    }

    private void OnActualThemeVariantChanged(object sender, EventArgs e) {
        //Get<ThemeService>().UpdateThemeVariant(ActualThemeVariant, Get<SettingService>().Setting.ActiveColorVariant);
    }

    #endregion


    private static void DisableAvaloniaDataAnnotationValidation() {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }
}