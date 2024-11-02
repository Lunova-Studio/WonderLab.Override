using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using System.Collections.Generic;

namespace WonderLab.Controls.Media.Behaviors;

public sealed class IsBlurMaskVisibleBehavior : Behavior<ExperimentalAcrylicBorder> {
    private Window _mainWindow;

    protected override void OnAttached() {
        base.OnAttached();

        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
            _mainWindow = lifetime.MainWindow;
            _mainWindow.PropertyChanged += OnPropertyChanged;
        }
    }

    private void OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
        if (e.Property == TopLevel.TransparencyLevelHintProperty) {
            AssociatedObject.IsVisible = e.NewValue is IReadOnlyList<WindowTransparencyLevel> newValue 
                && newValue[0] == WindowTransparencyLevel.AcrylicBlur;
        }

        if (e.Property == TopLevel.BackgroundProperty) {
            AssociatedObject.IsVisible = e.NewValue == Brushes.Transparent;
        }
    }
}