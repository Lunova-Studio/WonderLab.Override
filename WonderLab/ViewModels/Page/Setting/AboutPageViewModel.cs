using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WonderLab.ViewModels.Page.Setting;
public sealed partial class AboutPageViewModel : ObservableObject {
    [RelayCommand]
    private Task JumpToLink(string url) => Task.Run(() => {
        using var _ = Process.Start(new ProcessStartInfo(url) {
            UseShellExecute = true,
            Verb = "open"
        });
    });
}