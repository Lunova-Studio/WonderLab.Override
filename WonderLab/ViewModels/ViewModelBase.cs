using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using WonderLab.Interfaces.Navigation;

namespace WonderLab.ViewModels;

public abstract partial class ViewModelBase : ObservableObject {
    public INavigationService NavigationService { get; }

    public ViewModelBase() { }

    public ViewModelBase(INavigationService navigationService) {
        NavigationService = navigationService;
    }

    [RelayCommand]
    private async Task Goback() {
        await NavigationService?.GoBackAsync();
    }
}
