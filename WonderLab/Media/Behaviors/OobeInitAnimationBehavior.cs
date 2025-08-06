using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Labs.Lottie;
using Avalonia.Layout;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using System;
using System.Threading.Tasks;
using WonderLab.Extensions;
using WonderLab.SourceGenerator.Attributes;
using WonderLab.Utilities;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(Control), "BarTarget")]
[StyledProperty(typeof(Control), "FrameTarget")]
[StyledProperty(typeof(Lottie), "LottieTarget")]
public sealed partial class OobeInitAnimationBehavior : Behavior {
    protected override async void OnLoaded() {
#if DEBUG
        await Dispatcher.UIThread.InvokeAsync(async () => {
            var frameCV = ElementComposition.GetElementVisual(FrameTarget);
            var lottieCV = ElementComposition.GetElementVisual(LottieTarget);

            var lottieAni = CompositionAnimationUtil.CreateScalarAnimation(lottieCV, 1f, 0f,
                TimeSpan.FromMilliseconds(200), new ExponentialEaseOut());

            var frameAni = CompositionAnimationUtil.CreateScalarAnimation(frameCV, 0f, 1f,
                TimeSpan.FromMilliseconds(450), new ExponentialEaseOut(), TimeSpan.FromMilliseconds(100));

            lottieCV.StartAnimation("Opacity", lottieAni);
            frameCV.StartAnimation("Opacity", frameAni);
        });
#else
        await Task.Delay(500);
        LottieTarget.Path = "resm:WonderLab.Assets.Lotties.OOBE_Hello.json";
        LottieTarget.AnimationCompleted += OnAnimationCompleted;
#endif
    }

#if !DEBUG
    protected override void OnDetaching() {
        LottieTarget.AnimationCompleted -= OnAnimationCompleted;
    }
#endif

    private async void OnAnimationCompleted(object sender, EventArgs e) {
        await Dispatcher.UIThread.InvokeAsync(() => {
            var frameCV = ElementComposition.GetElementVisual(FrameTarget);
            var lottieCV = ElementComposition.GetElementVisual(LottieTarget);

            var lottieAni = CompositionAnimationUtil.CreateScalarAnimation(lottieCV, 1f, 0f,
                TimeSpan.FromMilliseconds(200), new ExponentialEaseOut());

            var frameAni = CompositionAnimationUtil.CreateScalarAnimation(frameCV, 0f, 1f,
                TimeSpan.FromMilliseconds(450), new ExponentialEaseOut(), TimeSpan.FromMilliseconds(100));

            lottieCV.StartAnimation("Opacity", lottieAni);
            frameCV.StartAnimation("Opacity", frameAni);
        });
    }
}