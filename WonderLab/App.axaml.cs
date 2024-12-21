using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using WonderLab.Extensions;
using WonderLab.Extensions.Hosting;
using WonderLab.Services;
using WonderLab.Services.Accounts;
using WonderLab.Services.Launch;
using WonderLab.Services.UI;
using WonderLab.ViewModels.Dialog;
using WonderLab.ViewModels.Dialog.Auth;
using WonderLab.ViewModels.Page;
using WonderLab.ViewModels.Page.Download;
using WonderLab.ViewModels.Page.Setting;
using WonderLab.ViewModels.Window;
using WonderLab.Views.Page;
using WonderLab.Views.Page.Download;
using WonderLab.Views.Page.Setting;

namespace WonderLab;

public sealed class App : Application {
    private const string LOG_OUTPUT_TEMPLATE = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] ({SourceContext}): {Message:lj}{NewLine}{Exception}";

    private static IServiceProvider ServiceProvider { get; set; }

    public static TKey Get<TKey>() {
        return ServiceProvider.GetRequiredService<TKey>();
    }

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void RegisterServices() {
        base.RegisterServices();

        _ = ConfigureIoC(out var host).RunAsync();
        ServiceProvider = host.Services;
    }

    public override void OnFrameworkInitializationCompleted() {
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.Exit += OnExit;
            desktop.Startup += OnStartup;

            desktop.MainWindow = Get<MainWindow>();
            desktop.MainWindow.DataContext = Get<MainWindowViewModel>();
        }

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        base.OnFrameworkInitializationCompleted();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
        var logger = ServiceProvider.GetRequiredService<ILogger<Application>>();
        logger.LogError((e.ExceptionObject as Exception).Message);
    }

    private void OnExit(object sender, ControlledApplicationLifetimeExitEventArgs e) {
        var configService = Get<ConfigService>();
        configService.Save();

        var logger = Get<ILogger<Application>>();
        logger.LogInformation("Exiting, exitcode is {exitCode}", e.ApplicationExitCode);
    }

    private void OnStartup(object sender, ControlledApplicationLifetimeStartupEventArgs e) {
        var configService = Get<ConfigService>();
        configService.Load();

        Get<GameService>().Initialize();
        Get<AccountService>().Initialize();

        //Override AccentColors
        Current.Resources["NormalAccentBrush"] =
            configService.Entries.ActiveAccentColor.ToColor().ToBrush();

        Current.Resources["DarkAccentBrush1"] =
            configService.Entries.ActiveAccentColor.ToColor().GetColorAfterLuminance(-0.15f).ToBrush();

        Current.Resources["DarkAccentBrush2"] =
            configService.Entries.ActiveAccentColor.ToColor().GetColorAfterLuminance(-0.30f).ToBrush();

        Current.Resources["LightAccentBrush1"] =
            configService.Entries.ActiveAccentColor.ToColor().GetColorAfterLuminance(0.15f).ToBrush();

        Current.Resources["LightAccentBrush2"] =
            configService.Entries.ActiveAccentColor.ToColor().GetColorAfterLuminance(0.30f).ToBrush();

        I18NExtension.Culture = new(configService.Entries.ActiveLanguage);

        var themeService = Get<ThemeService>();
        themeService.ApplyTheme(configService.Entries.ThemeType);
        themeService.ApplyWindowEffect(configService.Entries.BackgroundType);
    }

    private static IHost ConfigureIoC(out IHost host) {
        var builder = new AvaloniaHostBuilder();

        //Configure Service
        builder.Services.AddSingleton<TaskService>();
        builder.Services.AddSingleton<GameService>();
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<ConfigService>();
        builder.Services.AddSingleton<LaunchService>();
        builder.Services.AddSingleton<AccountService>();
        builder.Services.AddSingleton<NotificationService>();

        //Configure Window
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddSingleton<MainWindowViewModel>();

        //Configure Dialog
        builder.Services.AddTransient<OfflineAuthDialogViewModel>();
        builder.Services.AddTransient<MicrosoftAuthDialogViewModel>();
        builder.Services.AddTransient<YggdrasilAuthDialogViewModel>();
        builder.Services.AddTransient<ChooseAccountTypeDialogViewModel>();

        //Configure Page
        var page = builder.PageProvider;
        page.AddPage<HomePage, HomePageViewModel>("Home");
        page.AddPage<GamePage, GamePageViewModel>("Game");
        page.AddPage<TaskListPage, TaskListPageViewModel>("TaskList");
        page.AddPage<MultiplayerPage, MultiplayerPageViewModel>("Multiplayer");

        //Setting
        page.AddPage<SettingNavigationPage, SettingNavigationPageViewModel>("Setting/Navigation");
        page.AddPage<LaunchPage, LaunchPageViewModel>("Setting/Launch");
        page.AddPage<AccountPage, AccountPageViewModel>("Setting/Account");
        page.AddPage<NetworkPage, NetworkPageViewModel>("Setting/Network");
        page.AddPage<AppearancePage, AppearancePageViewModel>("Setting/Appearance");
        page.AddPage<AboutPage, AboutPageViewModel>("Setting/About");

        //Download
        page.AddPage<MinecraftNewsPage, MinecraftNewsPageViewModel>("Download/News");
        page.AddPage<DownloadNavigationPage, DownloadNavigationPageViewModel>("Download/Navigation");

        //Configure Logging
        Log.Logger = new LoggerConfiguration().WriteTo
            .Console(outputTemplate: LOG_OUTPUT_TEMPLATE).WriteTo
            .File(Path.Combine("logs", $"WonderLog.log"), rollingInterval: RollingInterval.Day, outputTemplate: LOG_OUTPUT_TEMPLATE)
            .CreateLogger();

        builder.Logging.AddSerilog(Log.Logger);
        return host = builder.Build();
    }
}