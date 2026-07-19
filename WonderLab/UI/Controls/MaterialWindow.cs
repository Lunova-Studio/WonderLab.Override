using System;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using DialogHostAvalonia;

namespace WonderLab.UI.Controls;

[TemplatePart(Name = "PART_DialogHost", Type = typeof(DialogHost))]
[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button), IsRequired = true)]
[TemplatePart(Name = "PART_MinimizeButton", Type = typeof(Button), IsRequired = true)]
public class MaterialWindow : Window {
    private Border _PART_TitleBarLayout;
    private Button _PART_CloseButton;
    private Button _PART_MinimizeButton;

    protected override Type StyleKeyOverride => typeof(MaterialWindow);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        _PART_CloseButton = e.NameScope.Find<Button>("PART_CloseButton");
        _PART_MinimizeButton = e.NameScope.Find<Button>("PART_MinimizeButton");
        _PART_TitleBarLayout = e.NameScope.Find<Border>("PART_TitleBarLayout");

        _PART_TitleBarLayout?.PointerPressed += (_, args) => BeginMoveDrag(args);

        _PART_CloseButton?.Click += (_, _) => Close();
        _PART_MinimizeButton?.Click += (_, _) => WindowState = WindowState.Minimized;
    }
}