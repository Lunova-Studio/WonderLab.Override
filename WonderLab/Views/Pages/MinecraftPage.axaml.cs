using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using WonderLab.Classes.Interfaces;
using WonderLab.Controls;

namespace WonderLab.Views.Pages;

public partial class MinecraftPage : UserControl, INavigationHost {
    Frame INavigationHost.Frame => this.PART_Frame;

    public MinecraftPage() {
        InitializeComponent();
    }
}