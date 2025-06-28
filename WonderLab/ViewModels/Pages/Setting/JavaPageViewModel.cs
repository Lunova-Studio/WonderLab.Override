using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Base.Utilities;
using MinecraftLaunch.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class JavaPageViewModel : ObservableObject {
    private readonly SettingService _settingService;
    private readonly ObservableCollection<JavaEntry> _javas;
    private readonly string[] JavaFilterPatterns = EnvironmentUtil.IsWindow 
        ? ["javaw.exe"] 
        : ["java"];

    [ObservableProperty] private int _maxMemorySize = 512;
    [ObservableProperty] private JavaEntry _activeJava;
    [ObservableProperty] private bool _isAutoSelectJava;

    public ReadOnlyObservableCollection<JavaEntry> Javas { get; }

    public JavaPageViewModel(SettingService settingService) {
        _settingService = settingService;

        ActiveJava = _settingService.Setting.ActiveJava;
        MaxMemorySize = _settingService.Setting.MaxMemorySize;
        IsAutoSelectJava = _settingService.Setting.IsAutoSelectJava;

        Javas = new(_javas = [.. _settingService.Setting.Javas]);
        _javas.CollectionChanged += OnCollectionChanged;
    }

    [RelayCommand]
    private void RemoveJava() {
        if (_javas.Remove(ActiveJava))
            ActiveJava = _javas.FirstOrDefault();
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
            var javaInfo = await JavaUtil.GetJavaInfoAsync(path)
                ?? throw new NullReferenceException();

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

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                _settingService.Setting.Javas.Add(_javas.Last());
                break;
            case NotifyCollectionChangedAction.Remove:
                _settingService.Setting.Javas.Remove(e.OldItems[0] as JavaEntry);
                break;
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        switch (e.PropertyName) {
            case nameof(ActiveJava):
                _settingService.Setting.ActiveJava = ActiveJava;
                break;
            case nameof(IsAutoSelectJava):
                _settingService.Setting.IsAutoSelectJava = IsAutoSelectJava;
                break;
            case nameof(MaxMemorySize):
                _settingService.Setting.MaxMemorySize = MaxMemorySize;
                break;
        }
    }
}
