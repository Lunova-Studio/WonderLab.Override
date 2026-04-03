using Avalonia.Controls;
using System.Threading.Tasks;

namespace WonderLab.Interfaces.Navigation;

public interface INavigationService {
    void Attach(NavigationPage navigationPage);

    Task GoBackAsync();
    Task NavigateToAsync<TViewModel>() where TViewModel : class;
    Task NavigateToPageAsync<TPage>() where TPage : UserControl;
    
}