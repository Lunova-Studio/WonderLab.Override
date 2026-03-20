using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Material.Icons;
using Material.Icons.Avalonia;

namespace WonderLab.Controls;

[StyledProperty(typeof(double), "IconSize", 14)]
[StyledProperty(typeof(MaterialIconKind), "Kind")]
[StyledProperty(typeof(MaterialIconKind), "SelectedKind")]
public sealed partial class NavigationItem : RadioButton {
    private MaterialIcon _PART_MaterialIcon;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_MaterialIcon = e.NameScope.Find<MaterialIcon>("PART_MaterialIcon");
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        _PART_MaterialIcon.Kind = IsChecked.Value
            ? SelectedKind
            : Kind;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (!IsLoaded)
            return;

        if (change.Property == IsCheckedProperty)
            _PART_MaterialIcon.Kind = change.GetNewValue<bool?>().Value
                ? SelectedKind
                : Kind;
    }

    //protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
    //    base.OnAttachedToVisualTree(e);
    //    Parent.PropertyChanged += OnParentPropertyChanged;
    //}

    //protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
    //    base.OnDetachedFromVisualTree(e);
    //    Parent.PropertyChanged -= OnParentPropertyChanged;
    //}


    //private void OnParentPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
    //    if (e.Property == BoundsProperty) {
    //        var width = e.GetNewValue<Rect>().Width;

    //        if(width is > 600 and < 839) {

    //        }
    //    }
    //}
}