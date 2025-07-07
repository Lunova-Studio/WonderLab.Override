using Avalonia.Controls;
using WonderLab.Classes.Models;
using WonderLab.Controls;

namespace WonderLab;

public partial class DashboardPage : Page {
    public DashboardPage() {
        InitializeComponent();
    }

    private void OnSelectTemplateKey(object sender, SelectTemplateEventArgs e) {
        e.TemplateKey = e.DataContext is SinglePlayerSaveModel
            ? "SinglePlayer"
            : "MultiPlayer";
    }
}