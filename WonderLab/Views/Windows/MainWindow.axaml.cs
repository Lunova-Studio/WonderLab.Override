using Avalonia.Controls;
using WonderLab.Interfaces.Navigation;
using WonderLab.ViewModels.Pages;

namespace WonderLab.Views.Windows;

public partial class MainWindow : Window {
    public MainWindow(INavigationService nav) {
        InitializeComponent();

        nav.Attach(RootNav);
        nav.NavigateToAsync<HomePageViewModel>();
    }
}