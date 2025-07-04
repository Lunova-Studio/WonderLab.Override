using Avalonia.Controls;
using WonderLab.Controls;

namespace WonderLab;

public partial class SearchPage : Page {
    public SearchPage() {
        InitializeComponent();
    }

    private void OnSelectTemplateKey(object sender, SelectTemplateEventArgs e) {
        e.TemplateKey = e.DataContext switch {
            string => "Minecraft",
            _ => "Unkonwn"
        };
    }
}