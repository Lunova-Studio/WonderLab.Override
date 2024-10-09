using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using WonderLab.ViewModels.Windows;
using System.Linq;
using WonderLab.Services.UI;
using WonderLab.ViewModels.Dialogs;
using WonderLab.Views.Controls;

namespace WonderLab.Views.Windows;

public sealed partial class MainWindow : Window {
    private MainWindowViewModel _viewModel;

    public MainWindow() => InitializeComponent();

    private void OnLoaded(object sender, RoutedEventArgs e) {
        _viewModel = DataContext as MainWindowViewModel;

        _viewModel._navigationService.NavigationRequest += x => {
            frame.Navigate(x.Page as Control);
        };

        this.AddHandler(DragDrop.DropEvent, OnDrop);
        App.GetService<ThemeService>().ApplyBackgroundAfterPageLoad(this);
    }

    private void OnDrop(object sender, DragEventArgs args) {
        if (args.Data.GetDataFormats().Contains(DragDropSelector.DEFAULT_DRAG_DATAFORMAT)) {
            return;
        }

        var file = args.Data.GetFiles().First();
        //_dialogService.ShowContentDialog<FileDropDialogViewModel>(file);
    }
}