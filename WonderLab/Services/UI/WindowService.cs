using System;
using Avalonia;
using System.Linq;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using WonderLab.Views.Windows;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WonderLab.Views;
using System.Xml.Serialization;
using WonderLab.Services.Wrap;

namespace WonderLab.Services.UI;

/// <summary>
/// 主窗体 <see cref="MainWindow"/> 的扩展服务类
/// </summary>
public sealed class WindowService {
    private DialogService _dialogService;
    private Action<PointerEventArgs> _pointerMovedAction;
    private Action<PointerEventArgs> _pointerExitedAction;

    public Window MainWindow { get; }

    private readonly WrapService _wrapService;
    private readonly SettingService _settingService;
    private readonly ILogger<WindowService> _logger;

    public bool IsLoaded => MainWindow.IsLoaded;
    public double ActualWidth => MainWindow.Bounds.Width;
    public double ActualHeight => MainWindow.Bounds.Height;

    public WindowService(SettingService settingService, WrapService wrapService, ILogger<WindowService> logger) {
        _logger = logger;
        _wrapService = wrapService;
        _settingService = settingService;

        MainWindow = SettingService.IsInitialize
            ? App.ServiceProvider.GetService<OobeWindow>()
            : App.ServiceProvider.GetService<MainWindow>();

        MainWindow.ActualThemeVariantChanged += (_, args) => {
            if (MainWindow.TransparencyLevelHint.Any(x => x == WindowTransparencyLevel.AcrylicBlur)) {
                //SetBackground(1);
            }
        };
    }

    public void Close() {
        if (_wrapService.Client is { IsConnected: true }) {
            _wrapService.Close();
        }

        MainWindow.Close();
    }

    public async void CopyText(string text) {
        await MainWindow.Clipboard.SetTextAsync(text);
    }

    public void SetWindowState(WindowState state) {
        MainWindow.WindowState = state;
    }

    public void BeginMoveDrag(PointerPressedEventArgs args) {
        MainWindow.BeginMoveDrag(args);
    }

    public void HandlePropertyChanged(AvaloniaProperty property, Action action) {
        MainWindow.PropertyChanged += (_, args) => {
            if (args.Property == property) {
                action?.Invoke();
            }
        };
    }

    public void RegisterPointerMoved(Action<PointerEventArgs> action) {
        _pointerMovedAction = action;
        MainWindow.PointerMoved += OnPointerMoved;
    }

    public void RegisterPointerExited(Action<PointerEventArgs> action) {
        _pointerExitedAction = action;
        MainWindow.PointerExited += OnPointerExited;
    }

    public void UnregisterPointerMoved() {
        MainWindow.PointerMoved -= OnPointerMoved;
    }

    public void UnregisterPointerExited() {
        MainWindow.PointerExited -= OnPointerExited;
    }

    public IStorageProvider GetStorageProvider() {
        return MainWindow.StorageProvider;
    }

    private void OnPointerMoved(object sender, PointerEventArgs e) {
        _pointerMovedAction?.Invoke(e);
    }

    private void OnPointerExited(object sender, PointerEventArgs e) {
        _pointerExitedAction?.Invoke(e);
    }
}