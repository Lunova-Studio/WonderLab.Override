﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Components.Parser;
using MinecraftLaunch.Components.Provider;
using MinecraftLaunch.Utilities;
using Monet.Avalonia;
using Serilog;
using System;
using System.IO;
using System.Linq;
using WonderLab.Classes.Processors;
using WonderLab.Controls;
using WonderLab.Extensions;
using WonderLab.Extensions.Hosting;
using WonderLab.Services;
using WonderLab.Services.Authentication;
using WonderLab.Services.Auxiliary;
using WonderLab.Services.Download;
using WonderLab.Services.Launch;
using WonderLab.Utilities;
using WonderLab.ViewModels.Dialogs.Setting;
using WonderLab.ViewModels.Pages;
using WonderLab.ViewModels.Pages.Download;
using WonderLab.ViewModels.Pages.GameSetting;
using WonderLab.ViewModels.Pages.Setting;
using WonderLab.ViewModels.Windows;
using WonderLab.Views.Pages;
using WonderLab.Views.Pages.Download;
using WonderLab.Views.Pages.Setting;
using WonderLab.Views.Windows;

namespace WonderLab;

public sealed partial class App : Application {
    private const string LOG_OUTPUT_TEMPLATE = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] ({SourceContext}): {Message:lj}{NewLine}{Exception}";

    public static MonetColors Monet { get; private set; }
    public static IServiceProvider ServiceProvider { get; private set; }

    public static TKey Get<TKey>() where TKey : class {
        return ServiceProvider.GetRequiredService<TKey>();
    }

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
        Monet = (Styles[1] as MonetColors)!;
        MinecraftParser.DataProcessors.Add(new SpecificSettingProcessor());
    }

    public override void RegisterServices() {
        base.RegisterServices();

        _ = ConfigureIoC(out var host).RunAsync();
        ServiceProvider = host.Services;
        CurseforgeProvider.CurseforgeApiKey = "$2a$10$Awb53b9gSOIJJkdV3Zrgp.CyFP.dI13QKbWn/4UZI4G4ff18WneB6";
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            DisableAvaloniaDataAnnotationValidation();

            desktop.Exit += OnExit;
            desktop.Startup += OnStartup;

            desktop.MainWindow = Get<MainWindow>();
            desktop.MainWindow.DataContext = Get<MainWindowViewModel>();

            if (desktop.MainWindow is WonderWindow)
                Get<ThemeService>().Initialize(desktop.MainWindow as WonderWindow);
        }

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        base.OnFrameworkInitializationCompleted();
    }

    #region Privates

    private static IHost ConfigureIoC(out IHost host) {
        var builder = new AvaloniaHostBuilder();

        builder.Services.AddSingleton<ModrinthProvider>();
        builder.Services.AddSingleton<CurseforgeProvider>();

        //Configure Service
        builder.Services.AddSingleton<ModService>();
        builder.Services.AddSingleton<SaveService>();
        builder.Services.AddSingleton<TaskService>();
        builder.Services.AddSingleton<GameService>();
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<SearchService>();
        builder.Services.AddSingleton<DialogService>();
        builder.Services.AddSingleton<LaunchService>();
        builder.Services.AddSingleton<SettingService>();
        builder.Services.AddSingleton<AccountService>();
        builder.Services.AddSingleton<ShaderpackService>();
        builder.Services.AddSingleton<GameProcessService>();
        builder.Services.AddSingleton<ResourcepackService>();
        builder.Services.AddSingleton<AuthenticationService>();

        //Configure Window
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddSingleton<MainWindowViewModel>();

        //Configure Dialog
        var dialogProvider = builder.DialogProvider;
        dialogProvider.AddDialog<OfflineAuthDialog, OfflineAuthDialogViewModel>("OfflineAuth");
        dialogProvider.AddDialog<MicrosoftAuthDialog, MicrosoftAuthDialogViewModel>("MicrosoftAuth");
        dialogProvider.AddDialog<YggdrasilAuthDialog, YggdrasilAuthDialogViewModel>("YggdrasilAuth");
        dialogProvider.AddDialog<ChooseAccountTypeDialog, ChooseAccountTypeDialogViewModel>("ChooseAccountType");

        //Configure Page
        var pageProvider = builder.PageProvider;
        pageProvider.AddPage<HomePage, HomePageViewModel>("Home");
        pageProvider.AddPage<GamePage, GamePageViewModel>("Game");
        pageProvider.AddPage<TaskPage, TaskPageViewModel>("Tasks");
        pageProvider.AddPage<MultiplayerPage, MultiplayerPageViewModel>("Multiplayer");

        //Setting
        pageProvider.AddPage<SettingNavigationPage, SettingNavigationPageViewModel>("Setting/Navigation");
        pageProvider.AddPage<LaunchPage, LaunchPageViewModel>("Setting/Launch");
        pageProvider.AddPage<JavaPage, JavaPageViewModel>("Setting/Java");
        pageProvider.AddPage<AccountPage, AccountPageViewModel>("Setting/Account");
        pageProvider.AddPage<NetworkPage, NetworkPageViewModel>("Setting/Network");
        pageProvider.AddPage<AppearancePage, AppearancePageViewModel>("Setting/Appearance");
        pageProvider.AddPage<AboutPage, AboutPageViewModel>("Setting/About");

        //GameSetting
        pageProvider.AddPage<ModSettingPage, ModSettingPageViewModel>("GameSetting/Mod");
        pageProvider.AddPage<GameSettingPage, GameSettingPageViewModel>("GameSetting/Setting");
        pageProvider.AddPage<ScreenshotSettingPage, ScreenshotSettingPageViewModel>("GameSetting/Screenshot");
        pageProvider.AddPage<ShaderpackSettingPage, ShaderpackSettingPageViewModel>("GameSetting/Shaderpack");
        pageProvider.AddPage<ResourcepackSettingPage, ResourcepackSettingPageViewModel>("GameSetting/Resourcepack");
        pageProvider.AddPage<GameSettingNavigationPage, GameSettingNavigationPageViewModel>("GameSetting/Navigation");

        //Dashboard
        pageProvider.AddPage<DashboardPage, DashboardPageViewModel>("Dashboard");

        //Download
        pageProvider.AddPage<DownloadNavigationPage, DownloadNavigationPageViewModel>("Download/Navigation");
        pageProvider.AddPage<DownloadDashboardPage, DownloadDashboardPageViewModel>("Download/Dashboard");
        pageProvider.AddPage<SearchPage, SearchPageViewModel>("Download/Search");

        //Configure Logging
        Log.Logger = new LoggerConfiguration().WriteTo
            .Console(outputTemplate: LOG_OUTPUT_TEMPLATE).WriteTo
            .File(Path.Combine(PathUtil.GetLogsFolderPath(), "logs", $"WonderLog.log"), rollingInterval: RollingInterval.Day, outputTemplate: LOG_OUTPUT_TEMPLATE)
            .CreateLogger();

        builder.Logging.AddSerilog(Log.Logger);
        return host = builder.Build();
    }

    private static void DisableAvaloniaDataAnnotationValidation() {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
        var logger = ServiceProvider.GetRequiredService<ILogger<Application>>();
        if (e.ExceptionObject is Exception ex)
            logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
    }

    private void OnExit(object sender, ControlledApplicationLifetimeExitEventArgs e) {
        var logger = Get<ILogger<Application>>();
        logger.LogInformation("Exiting, exitcode is {exitCode}", e.ApplicationExitCode);

        Get<SettingService>().Save();
    }

    private void OnStartup(object sender, ControlledApplicationLifetimeStartupEventArgs e) {
        Get<SettingService>().Initialize();
        Get<GameService>().RefreshGames();
        HttpUtil.Initialize(new FlurlClient {
            Settings = {
                Timeout = TimeSpan.FromSeconds(15),
            },
            Headers = {
                { "User-Agent", "WonderLab/2.0" },
            },
        });

        ActualThemeVariantChanged += OnActualThemeVariantChanged;
        PlatformSettings.ColorValuesChanged += OnColorValuesChanged;
        Get<ILogger<App>>().LogInformation("当前版本号：{version}", new Uri("resm:WonderLab.version.txt").ToText());
    }

    private void OnColorValuesChanged(object sender, Avalonia.Platform.PlatformColorValues e) {
        var settings = Get<SettingService>().Setting;
        if (settings.IsEnableSystemColor)
            Get<ThemeService>().UpdateColorScheme(settings.ActiveColorVariant,
                settings.ActiveColor = e.AccentColor1.ToUInt32());
    }

    private void OnActualThemeVariantChanged(object sender, EventArgs e) {
        Get<ThemeService>().UpdateThemeVariant(ActualThemeVariant, Get<SettingService>().Setting.ActiveColorVariant);
    }

    #endregion
}