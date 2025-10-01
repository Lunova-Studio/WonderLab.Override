using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Launch;
using ObservableCollections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using WonderLab.Services;
using WonderLab.Services.Launch;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MinecraftLogWindowViewModel : ObservableRecipient {
    private readonly ThemeService _themeService;

    private readonly ISynchronizedView<MinecraftLogModel, MinecraftLogModel> _logsView;

    private readonly ObservableList<MinecraftLogModel> _logs = [];
    private readonly ObservableCollection<MinecraftLogLevel> _enabledLogLevels = [
        MinecraftLogLevel.Info,
        MinecraftLogLevel.Warning,
        MinecraftLogLevel.Error,
        MinecraftLogLevel.Fatal,
        MinecraftLogLevel.Debug
    ];

    [ObservableProperty] private bool _isInfoEnabled = true;
    [ObservableProperty] private bool _isWarnEnabled = true;
    [ObservableProperty] private bool _isErrorEnabled = true;
    [ObservableProperty] private bool _isFatalEnabled = true;
    [ObservableProperty] private bool _isDebugEnabled = true;

    [ObservableProperty] private string _title = "日志 - Loading...";
    [ObservableProperty] private MinecraftProcessModel _minecraftProcess;

    public Action ScrollToEnd { get; set; }
    public INotifyCollectionChangedSynchronizedViewList<MinecraftLogModel> MinecraftLogs { get; }

    public MinecraftLogWindowViewModel(ThemeService themeService) {
        _themeService = themeService;

        _logsView = _logs.CreateView(x => x);
        MinecraftLogs = _logsView.ToNotifyCollectionChanged();

        _enabledLogLevels.CollectionChanged += OnEnabledLogLevelsChanged;
        PropertyChanged += OnPropertyChanged;
    }

    private void AttachFilter() {
        _logsView.ResetFilter();
        _logsView.AttachFilter(x => _enabledLogLevels.Contains(x.Level));
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(IsInfoEnabled)) {
            if (IsInfoEnabled) {
                _enabledLogLevels.Add(MinecraftLogLevel.Info);
                _enabledLogLevels.Add(MinecraftLogLevel.Unknown);
            } else {
                _enabledLogLevels.Remove(MinecraftLogLevel.Info);
                _enabledLogLevels.Remove(MinecraftLogLevel.Unknown);
            }
        }

        if (e.PropertyName is nameof(IsWarnEnabled)) {
            if (IsWarnEnabled)
                _enabledLogLevels.Add(MinecraftLogLevel.Warning);
            else
                _enabledLogLevels.Remove(MinecraftLogLevel.Warning);
        }

        if (e.PropertyName is nameof(IsErrorEnabled)) {
            if (IsErrorEnabled)
                _enabledLogLevels.Add(MinecraftLogLevel.Error);
            else
                _enabledLogLevels.Remove(MinecraftLogLevel.Error);
        }

        if (e.PropertyName is nameof(IsFatalEnabled)) {
            if (IsFatalEnabled)
                _enabledLogLevels.Add(MinecraftLogLevel.Fatal);
            else
                _enabledLogLevels.Remove(MinecraftLogLevel.Fatal);
        }

        if (e.PropertyName is nameof(IsDebugEnabled)) {
            if (IsDebugEnabled)
                _enabledLogLevels.Add(MinecraftLogLevel.Debug);
            else
                _enabledLogLevels.Remove(MinecraftLogLevel.Debug);
        }

        if (ScrollToEnd is not null)
            ScrollToEnd();
    }

    private void OnExited(object sender, EventArgs e) {
        MinecraftProcess.MinecraftProcess.Exited -= OnExited;
        MinecraftProcess.MinecraftProcess.OutputLogReceived -= OnOutputLogReceived;
    }

    private void OnOutputLogReceived(object sender, LogReceivedEventArgs e) {
        Dispatcher.UIThread.Post(() => {
            _logs.Add(new MinecraftLogModel(e.Data, _themeService));
            ScrollToEnd();
        });
    }

    private void OnEnabledLogLevelsChanged(object sender, NotifyCollectionChangedEventArgs e) {
        AttachFilter();
    }

    partial void OnMinecraftProcessChanged(MinecraftProcessModel value) {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        Title = $"日志 - {value.MinecraftProcess.Process.Id}";
        MinecraftProcess = value;
        MinecraftProcess.MinecraftProcess.Exited += OnExited;
        MinecraftProcess.MinecraftProcess.OutputLogReceived += OnOutputLogReceived;

        _logs.AddRange(value.GameLogs.Select(x => new MinecraftLogModel(x, _themeService)));
    }
}

public sealed partial class MinecraftLogModel : ObservableObject {
    public MinecraftLogLevel Level { get; }

    [ObservableProperty] private InlineCollection _formatLogs;

    public MinecraftLogModel(MinecraftLogEntry logEntry, ThemeService themeService) {
        var logs = new List<Run>();

        if (!string.IsNullOrEmpty(logEntry.Source))
            logs.Add(new Run {
                Text = $"[{logEntry.Time}] [{logEntry.Source}/{logEntry.LogLevel.ToString().ToUpper()}]",
                Foreground = logEntry.LogLevel switch {
                    MinecraftLogLevel.Info => themeService.GetResource("InfoSourceRunBrush") as IBrush,
                    MinecraftLogLevel.Error => themeService.GetResource("ErrorSourceRunBrush") as IBrush,
                    MinecraftLogLevel.Warning => themeService.GetResource("WarningSourceRunBrush") as IBrush,
                    _ => themeService.GetResource("UnknownSourceRunBrush") as IBrush
                }
            });

        logs.Add(new Run {
            Text = $" {logEntry.Log}",
            Foreground = logEntry.LogLevel switch {
                MinecraftLogLevel.Info => themeService.GetResource("NormalLogRunBrush") as IBrush,
                MinecraftLogLevel.Error => themeService.GetResource("ErrorLogRunBrush") as IBrush,
                MinecraftLogLevel.Warning => themeService.GetResource("WarningLogRunBrush") as IBrush,
                _ => themeService.GetResource("NormalLogRunBrush") as IBrush
            }
        });

        FormatLogs = [.. logs];
        Level = logEntry.LogLevel;
    }
}