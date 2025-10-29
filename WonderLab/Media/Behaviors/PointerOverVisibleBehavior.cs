using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(Visual), "Target")]
public sealed partial class PointerOverVisibleBehavior : Behavior {
    protected override void OnLoaded() {
        base.OnLoaded();

        if (Target is null)
            throw new RenderTargetNotReadyException();

        Target.IsVisible = false;

        if (AssociatedObject is Control control) {
            control.PointerExited += OnPointerExited;
            control.PointerEntered += OnPointerEntered;
        }
    }

    protected override void OnDetachedFromVisualTree() {
        base.OnDetachedFromVisualTree();

        if (AssociatedObject is Control control) {
            control.PointerExited -= OnPointerExited;
            control.PointerEntered -= OnPointerEntered;
        }
    }

    private void OnPointerExited(object sender, PointerEventArgs e) {
        Target.IsVisible = false;
    }

    private void OnPointerEntered(object sender, PointerEventArgs e) {
        Target.IsVisible = true;
    }
}
