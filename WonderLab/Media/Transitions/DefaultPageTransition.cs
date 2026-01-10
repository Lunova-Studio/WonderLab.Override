using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Utilities;

namespace WonderLab.Media.Transitions;

public sealed class DefaultPageTransition : IPageTransition {
    public TimeSpan Duration { get; set; }
    public Easing Easing { get; set; } = new ExponentialEaseOut();

    public DefaultPageTransition() { }

    public DefaultPageTransition(TimeSpan duration) {
        Duration = duration;
    }

    public async Task Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken) {
        if (from is not null) {
            var fromEV = ElementComposition.GetElementVisual(from);
            var size = fromEV!.Size;

            var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(fromEV, 1, 0, TimeSpan.FromMilliseconds(400), Easing);
            var scaleAni = CompositionAnimationUtil.CreateVector3Animation(fromEV, new(1), new(0.9f), Duration, Easing);

            scaleAni.Target = CompositionAnimationUtil.PROPERTY_SCALE;
            opacityAni.Target = CompositionAnimationUtil.PROPERTY_OPACITY;

            var group = fromEV.Compositor.CreateAnimationGroup();
            group.Add(scaleAni);
            group.Add(opacityAni);

            fromEV!.CenterPoint = new Vector3((float)size.X / 2, (float)size.Y / 2, (float)fromEV.CenterPoint.Z);
            await Dispatcher.UIThread.InvokeAsync(() => fromEV.StartAnimationGroup(group));
        }

        if (to is not null) {
            var toEV = ElementComposition.GetElementVisual(to);
            var size = toEV!.Size;

            var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(toEV, 0, 1, Duration, Easing);
            var scaleAni = CompositionAnimationUtil.CreateVector3Animation(toEV, new(1.2f), new(1), Duration, Easing);

            scaleAni.Target = CompositionAnimationUtil.PROPERTY_SCALE;
            opacityAni.Target = CompositionAnimationUtil.PROPERTY_OPACITY;

            var group = toEV.Compositor.CreateAnimationGroup();
            group.Add(scaleAni);
            group.Add(opacityAni);

            toEV!.CenterPoint = new Vector3((float)size.X / 2, (float)size.Y / 2, (float)toEV.CenterPoint.Z);
            await Dispatcher.UIThread.InvokeAsync(() => toEV.StartAnimationGroup(group));
        }

        (from as Control).IsHitTestVisible = false;
        (to as Control).IsHitTestVisible = true;
    }
}