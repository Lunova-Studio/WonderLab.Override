using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using System;
using System.Numerics;

namespace WonderLab.Controls.Media.Behaviors;

public sealed class AnimationInPageBehavior : Behavior<UserControl> {
    protected override void OnAttached() {
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching() {
        base.OnDetaching();
        AssociatedObject.Loaded -= OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) {
        Dispatcher.UIThread.Post(() => {
            var compositor = ElementComposition.GetElementVisual(AssociatedObject).Compositor;
            var vector3KeyFrameAnimation = compositor.CreateVector3KeyFrameAnimation();
            var animationGroup = compositor.CreateAnimationGroup();

            vector3KeyFrameAnimation.Target = "Offset";
            vector3KeyFrameAnimation.Duration = TimeSpan.FromMilliseconds(450);

            vector3KeyFrameAnimation.InsertKeyFrame(0f, new Vector3(0, 100, 0));
            vector3KeyFrameAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue", new SplineEasing(0.1, 0.9, 0.2, 1.0));

            var opacAni = compositor.CreateScalarKeyFrameAnimation();
            opacAni.Target = "Opacity";
            opacAni.Duration = TimeSpan.FromMilliseconds(450);

            opacAni.InsertKeyFrame(0f, 0);
            opacAni.InsertKeyFrame(1f, 1f, new SplineEasing(0.1, 0.0, 0.2, 1.0));

            animationGroup.Add(vector3KeyFrameAnimation);
            animationGroup.Add(opacAni);

            ElementComposition.GetElementVisual(AssociatedObject).StartAnimationGroup(animationGroup);
        }, DispatcherPriority.ApplicationIdle);
    }
}