using System.Collections.Generic;

namespace WonderLab.ViewModels.Pages.Settings;

public sealed partial class JavaSettingsPageViewModel : ViewModelBase {
    public List<string> Javas { get; set; } = [
        "111",
        "222",
        "333",
        "444",
        "555",
    ];

    public JavaSettingsPageViewModel() {

    }


}