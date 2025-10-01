using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using System;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Extensions;
using WonderLab.Media.Easings;
using WonderLab.SourceGenerator.Attributes;
using WonderLab.Utilities;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(bool), "IsExited")]
public sealed partial class MinecraftProcessCardAnimationBehavior : Behavior<Button> {
    protected override async void OnLoaded() {
        base.OnLoaded();
        await Task.Delay(100);
        var compositionVisual = ElementComposition.GetElementVisual(AssociatedObject);

        var height = AssociatedObject.Bounds.Height + 20;
        var xPoint = compositionVisual.Offset.X;

        var offsetAni = CompositionAnimationUtil.CreateVector3Animation(compositionVisual,
            new((float)xPoint, (float)height, 0),
            new((float)xPoint, 0, 0), TimeSpan.FromSeconds(0.4), new WonderBackEaseOut() { Amplitude = Amplitude.Strong });

        var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(compositionVisual, 0, 1,
            TimeSpan.FromSeconds(0.6), new CubicEaseOut());

        offsetAni.Target = CompositionAnimationUtil.PROPERTY_OFFSET;
        opacityAni.Target = CompositionAnimationUtil.PROPERTY_OPACITY;

        var aniGroup = compositionVisual.Compositor.CreateAnimationGroup();

        aniGroup.Add(offsetAni);
        aniGroup.Add(opacityAni);
        compositionVisual.StartAnimationGroup(aniGroup);
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == IsExitedProperty && change.GetNewValue<bool>())
            await RunExitAnimation();

        async Task RunExitAnimation() {
            var compositionVisual = ElementComposition.GetElementVisual(AssociatedObject);

            var height = AssociatedObject.Bounds.Height + 20;
            var xPoint = compositionVisual.Offset.X;

            var offsetAni = CompositionAnimationUtil.CreateVector3Animation(compositionVisual,
                new((float)xPoint, 0, 0),
                new((float)xPoint, (float)height, 0), TimeSpan.FromSeconds(0.4), new WonderBackEaseIn() { Amplitude = Amplitude.Strong });

            compositionVisual.StartAnimation(CompositionAnimationUtil.PROPERTY_OFFSET, offsetAni);
            await Task.Delay(TimeSpan.FromSeconds(0.2));
            AssociatedObject.Width = 0;
        }
    }
}