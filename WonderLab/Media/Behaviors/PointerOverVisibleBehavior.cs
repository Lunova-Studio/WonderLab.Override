using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Rendering.Composition;
using Avalonia.Xaml.Interactivity;
using System;
using WonderLab.Media.Easings;
using WonderLab.SourceGenerator.Attributes;
using WonderLab.Utilities;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(Visual), "Target")]
public sealed partial class PointerOverVisibleBehavior : Behavior {
    protected override void OnLoaded() {
        base.OnLoaded();

        if (Target is null)
            throw new RenderTargetNotReadyException();

        var compositionVisual = ElementComposition.GetElementVisual(Target);
        compositionVisual.Opacity = 0;
        compositionVisual.Visible = false;

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
        var compositionVisual = ElementComposition.GetElementVisual(Target);
        compositionVisual.Visible = false;
    }

    private void OnPointerEntered(object sender, PointerEventArgs e) {
        var compositionVisual = ElementComposition.GetElementVisual(Target);
        var xPoint = compositionVisual.Offset.X;
        var yPoint = compositionVisual.Offset.Y;

        compositionVisual.Visible = true;
        var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(compositionVisual, 0, 1,
            TimeSpan.FromSeconds(0.2), new CubicEaseOut());

        var offsetAni = CompositionAnimationUtil.CreateVector3Animation(compositionVisual,
            new((float)xPoint, 35, 0),
            new((float)xPoint, (float)yPoint, 0), TimeSpan.FromSeconds(0.25), new WonderBackEaseOut() { Amplitude = Classes.Enums.Amplitude.Strong});


        offsetAni.Target = CompositionAnimationUtil.PROPERTY_OFFSET;
        opacityAni.Target = CompositionAnimationUtil.PROPERTY_OPACITY;

        var aniGroup = compositionVisual.Compositor.CreateAnimationGroup();
        aniGroup.Add(offsetAni);
        aniGroup.Add(opacityAni);

        compositionVisual.StartAnimationGroup(aniGroup);
    }
}
