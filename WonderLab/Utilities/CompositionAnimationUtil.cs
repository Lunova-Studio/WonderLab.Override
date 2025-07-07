using Avalonia.Animation.Easings;
using Avalonia.Rendering.Composition;
using System;
using System.Numerics;

namespace WonderLab.Utilities;

public static class CompositionAnimationUtil {
    public const string PROPERTY_SIZE = "Size";
    public const string PROPERTY_SCALE = "Scale";
    public const string PROPERTY_OFFSET = "Offset";
    public const string PROPERTY_OPACITY = "Opacity";

    public static ScalarKeyFrameAnimation CreateScalarAnimation(CompositionVisual compositionVisual, float from, float to, TimeSpan duration, Easing easing = null, TimeSpan? delay = null) {
        easing ??= new ExponentialEaseOut();

        var compositor = compositionVisual.Compositor;
        var scalarAni = compositor.CreateScalarKeyFrameAnimation();

        scalarAni.InsertKeyFrame(0f, from, easing);
        scalarAni.InsertKeyFrame(1f, to, easing);
        scalarAni.Duration = duration;
        scalarAni.DelayTime = delay ?? TimeSpan.Zero;

        return scalarAni;
    }

    public static Vector2KeyFrameAnimation CreateVector2Animation(CompositionVisual compositionVisual, Vector2 from, Vector2 to, TimeSpan duration, Easing easing = null, TimeSpan? delay = null) {
        easing ??= new ExponentialEaseOut();

        var compositor = compositionVisual.Compositor;
        var vector2Ani = compositor.CreateVector2KeyFrameAnimation();

        vector2Ani.InsertKeyFrame(0f, from, easing);
        vector2Ani.InsertKeyFrame(1f, to, easing);
        vector2Ani.Duration = duration;
        vector2Ani.DelayTime = delay ?? TimeSpan.Zero;

        return vector2Ani;
    }

    public static Vector3KeyFrameAnimation CreateVector3Animation(CompositionVisual compositionVisual, string target, TimeSpan duration, Easing easing = null, TimeSpan? delay = null) {
        easing ??= new ExponentialEaseOut();

        var compositor = compositionVisual.Compositor;
        var vector3Ani = compositor.CreateVector3KeyFrameAnimation();

        vector3Ani.Target = target;
        vector3Ani.Duration = duration;
        vector3Ani.DelayTime = delay ?? TimeSpan.Zero;

        return vector3Ani;
    }

    public static Vector3KeyFrameAnimation CreateVector3Animation(CompositionVisual compositionVisual, Vector3 from, Vector3 to, TimeSpan duration, Easing easing = null, TimeSpan? delay = null) {
        easing ??= new ExponentialEaseOut();

        var compositor = compositionVisual.Compositor;
        var vector3Ani = compositor.CreateVector3KeyFrameAnimation();

        vector3Ani.InsertKeyFrame(0f, from, easing);
        vector3Ani.InsertKeyFrame(1f, to, easing);
        vector3Ani.Duration = duration;
        vector3Ani.DelayTime = delay ?? TimeSpan.Zero;

        return vector3Ani;
    }

    public static void CreateSizeImplicitAnimation(CompositionVisual compositionVisual, double milliseconds = 450) {
        if (compositionVisual == null)
            return;

        var compositor = compositionVisual.Compositor;
        var animationGroup = compositor.CreateAnimationGroup();

        var sizeAnimation = compositor.CreateVector2KeyFrameAnimation();
        sizeAnimation.Target = "Size";
        sizeAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", new ExponentialEaseOut());
        sizeAnimation.Duration = TimeSpan.FromMilliseconds(milliseconds);

        animationGroup.Add(sizeAnimation);

        var implicitAnimationCollection = compositor.CreateImplicitAnimationCollection();
        implicitAnimationCollection["Size"] = animationGroup;

        compositionVisual.ImplicitAnimations = implicitAnimationCollection;
    }

    public static void CreateOffsetImplicitAnimation(CompositionVisual compositionVisual, double milliseconds = 450) {
        if (compositionVisual == null)
            return;

        var compositor = compositionVisual.Compositor;
        var animationGroup = compositor.CreateAnimationGroup();

        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Target = "Offset";
        offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", new ExponentialEaseOut());
        offsetAnimation.Duration = TimeSpan.FromMilliseconds(milliseconds);

        animationGroup.Add(offsetAnimation);

        var implicitAnimationCollection = compositor.CreateImplicitAnimationCollection();
        implicitAnimationCollection["Offset"] = animationGroup;

        compositionVisual.ImplicitAnimations = implicitAnimationCollection;
    }

    public static void CreateOpacityImplicitAnimation(CompositionVisual compositionVisual, double milliseconds = 700) {
        if (compositionVisual == null)
            return;

        var compositor = compositionVisual.Compositor;
        var animationGroup = compositor.CreateAnimationGroup();

        ScalarKeyFrameAnimation opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
        opacityAnimation.Target = "Opacity";
        opacityAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
        opacityAnimation.Duration = TimeSpan.FromMilliseconds(milliseconds);

        animationGroup.Add(opacityAnimation);

        var implicitAnimationCollection = compositor.CreateImplicitAnimationCollection();
        implicitAnimationCollection["Opacity"] = animationGroup;

        compositionVisual.ImplicitAnimations = implicitAnimationCollection;
    }
}