﻿using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Utilities;

namespace WonderLab.Media.Transitions;

public sealed class EntrancePageTransition : IPageTransition {
    public Easing Easing { get; set; } = new ExponentialEaseOut();
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.4);

    public EntrancePageTransition() { }

    public EntrancePageTransition(TimeSpan duration = default) {
        Duration = duration;
    }

    public async Task Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken) {
        if (from is not null) {
            var fromEV = ElementComposition.GetElementVisual(from);
            var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(fromEV, 1, 0, Duration, Easing);
            fromEV.StartAnimation(CompositionAnimationUtil.PROPERTY_OPACITY, opacityAni);
        }

        if (to is not null) {
            var toEV = ElementComposition.GetElementVisual(to);
            var group = toEV.Compositor.CreateAnimationGroup();

            var xPoint = toEV.Offset.X;
            var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(toEV, 0, 1, Duration, Easing);
            var offsetAni = CompositionAnimationUtil.CreateVector3Animation(toEV,
                new((float)xPoint, 150, 0),
                    new((float)xPoint, 0, 0), Duration, Easing);

            offsetAni.Target = CompositionAnimationUtil.PROPERTY_OFFSET;
            opacityAni.Target = CompositionAnimationUtil.PROPERTY_OPACITY;

            group.Add(offsetAni);
            group.Add(opacityAni);
            toEV.StartAnimationGroup(group);
        }

        (from as Control).IsHitTestVisible = false;
        (to as Control).IsHitTestVisible = true;
    }
}