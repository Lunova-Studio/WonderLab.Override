using Avalonia.Controls;
using MinecraftLaunch.Base.Interfaces;
using MinecraftLaunch.Base.Models.Network;
using System;
using WonderLab.Controls;

namespace WonderLab;

public partial class SearchPage : Page {
    public SearchPage() {
        InitializeComponent();
    }

    private void OnSelectTemplateKey(object sender, SelectTemplateEventArgs e) {
        e.TemplateKey = e.DataContext switch {
            string => "Minecraft",
            ModrinthResource => "ModrinthResource",
            CurseforgeResource => "CurseforgeResource",
            _ => throw new NotSupportedException()
        };
    }
}