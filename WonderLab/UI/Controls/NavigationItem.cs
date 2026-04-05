using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Material.Icons;
using Material.Icons.Avalonia;
using System;

namespace WonderLab.UI.Controls;

[StyledProperty(typeof(double), "IconSize", 14)]
[StyledProperty(typeof(MaterialIconKind), "Kind")]
[StyledProperty(typeof(MaterialIconKind), "SelectedKind")]
public sealed partial class NavigationItem : RadioButton {
    private MaterialIcon _PART_MaterialIcon;

    protected override Type StyleKeyOverride => typeof(NavigationItem);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_MaterialIcon = e.NameScope.Find<MaterialIcon>("PART_MaterialIcon");
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        _PART_MaterialIcon?.Kind = IsChecked.Value
            ? SelectedKind
            : Kind;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (!IsLoaded)
            return;

        if (change.Property == IsCheckedProperty)
            _PART_MaterialIcon?.Kind = change.GetNewValue<bool?>().Value
                ? SelectedKind
                : Kind;
    }
}