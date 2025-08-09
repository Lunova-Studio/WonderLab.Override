using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Base.Utilities;
using MinecraftLaunch.Utilities;
using ObservableCollections;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class ChooseJavaPageViewModel : PageViewModelBase {
    private readonly SettingService _settingService;
    private readonly ObservableList<JavaEntry> _javas;
    private readonly string[] JavaFilterPatterns = EnvironmentUtil.IsWindow
        ? ["javaw.exe"]
        : ["java"];

    [ObservableProperty] private JavaEntry _activeJava;

    public INotifyCollectionChangedSynchronizedViewList<JavaEntry> Javas { get; }

    public ChooseJavaPageViewModel(SettingService settingService) {
        _settingService = settingService;
        _javas = [.. _settingService.Setting.Javas];

        Javas = _javas.ToNotifyCollectionChangedSlim();
        ActiveJava = _settingService.Setting.ActiveJava ?? _javas?.FirstOrDefault();
        SendEnabledMessage();

        _javas.CollectionChanged += OnCollectionChanged;
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(ActiveJava)) {
            _settingService.Setting.ActiveJava = ActiveJava;
            SendEnabledMessage();
        }
    }

    private void SendEnabledMessage() {
        WeakReferenceMessenger.Default.Send(new EnabledChangedMessage(_settingService.Setting.IsAutoSelectJava ||
            (_javas.Count > 0 && ActiveJava != null)));
    }

    private void OnCollectionChanged(in NotifyCollectionChangedEventArgs<JavaEntry> e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                _settingService.Setting.Javas.Add(_javas.Last());
                break;
            case NotifyCollectionChangedAction.Remove:
                _settingService.Setting.Javas.Remove(e.OldItems[0]);
                break;
        }

        SendEnabledMessage();
    }

    [RelayCommand]
    private Task BrowserJava() => Dispatcher.UIThread.InvokeAsync(async () => {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
            var result = await lifetime.MainWindow.StorageProvider.OpenFilePickerAsync(new() {
                AllowMultiple = false,
                FileTypeFilter = [new("Java") { Patterns = JavaFilterPatterns }]
            });

            if (result is { Count: 0 })
                return;

            var path = result[0].Path.LocalPath;
            if (EnvironmentUtil.IsLinux && !path.EndsWith("java", StringComparison.OrdinalIgnoreCase)) {
                WeakReferenceMessenger.Default.Send(new NotificationMessage("Java 选择错误，请选择正确的java可执行文件",
                    NotificationType.Error));
                return;
            }

            var javaInfo = await JavaUtil.GetJavaInfoAsync(path);

            _javas.Add(javaInfo);
            ActiveJava = _javas.Last();
        }
    }, DispatcherPriority.Background);

    [RelayCommand]
    private Task AutoSearchJava() => Task.Run(async () => {
        var asyncJavas = JavaUtil.EnumerableJavaAsync();

        await foreach (var java in asyncJavas) {
            if (_javas.Any(x => x.JavaPath == java.JavaPath))
                continue;

            _javas.Add(java);
        }

        ActiveJava = _javas.LastOrDefault();
    });
}