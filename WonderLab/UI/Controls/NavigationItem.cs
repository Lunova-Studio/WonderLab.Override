using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Material.Icons;
using Material.Icons.Avalonia;
using System.Diagnostics;

namespace WonderLab.UI.Controls;

[PseudoClasses(":pressed", ":selected")]
[StyledProperty(typeof(double), "IconSize", 14)]
[StyledProperty(typeof(MaterialIconKind), "Kind")]
[StyledProperty(typeof(MaterialIconKind), "SelectedKind")]
public sealed partial class NavigationItem : ListBoxItem {
    private MaterialIcon _PART_MaterialIcon;

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        _PART_MaterialIcon?.Kind = IsSelected
            ? SelectedKind
            : Kind;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_MaterialIcon = e.NameScope.Find<MaterialIcon>("PART_MaterialIcon");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (!IsLoaded)
            return;

        if (change.Property == IsSelectedProperty)
            _PART_MaterialIcon?.Kind = change.GetNewValue<bool>()
                ? SelectedKind
                : Kind;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e) {
        if (e.Properties.IsLeftButtonPressed)// 只有左键按下才会更新 IsSelected
            base.OnPointerPressed(e);
    }
}