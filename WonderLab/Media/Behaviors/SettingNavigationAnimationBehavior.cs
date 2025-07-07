using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using System;
using System.Numerics;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(string), "PageKey")]
[StyledProperty(typeof(bool), "IsForward")]
[StyledProperty(typeof(AvaloniaPageProvider), "PageProvider")]
public sealed partial class SettingNavigationFromBehavior : Behavior<Control> {
    protected override void OnLoaded() {
        base.OnLoaded();
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
        if (e.Property == PageKeyProperty)
            RunAnimation();
    }

    private void RunAnimation() {
        Dispatcher.UIThread.Post(() => {
            var compositionVisual = ElementComposition.GetElementVisual(AssociatedObject);
            var compositor = compositionVisual.Compositor;

            var group = compositor!.CreateAnimationGroup();
            if (IsForward) {
                var scaleAni = compositor!.CreateVector3KeyFrameAnimation();
                scaleAni.Target = "Scale";
                scaleAni!.Duration = TimeSpan.FromSeconds(0.70);

                scaleAni.InsertExpressionKeyFrame(0f, "this.FinalValue", new ExponentialEaseOut());
                scaleAni.InsertKeyFrame(1f, new(0.75f), new ExponentialEaseOut());

                var opacityAni = compositor?.CreateScalarKeyFrameAnimation();
                opacityAni!.Target = "Opacity";
                opacityAni!.Duration = TimeSpan.FromSeconds(0.50);

                opacityAni.InsertExpressionKeyFrame(0f, "this.FinalValue", new CubicEaseOut());
                opacityAni?.InsertKeyFrame(1f, 0f, new CubicEaseOut());

                group.Add(scaleAni);
                group.Add(opacityAni);
            } else {
                var scaleAni = compositor!.CreateVector3KeyFrameAnimation();
                scaleAni.Target = "Scale";
                scaleAni!.Duration = TimeSpan.FromSeconds(0.70);
                scaleAni.InsertKeyFrame(0f, Vector3.One, new ExponentialEaseOut());
                scaleAni.InsertKeyFrame(1f, Vector3.One, new ExponentialEaseOut());

                var opacityAni = compositor?.CreateScalarKeyFrameAnimation();
                opacityAni!.Target = "Opacity";
                opacityAni!.Duration = TimeSpan.FromSeconds(0.75);
                opacityAni?.InsertKeyFrame(0f, 0f, new ExponentialEaseOut());
                opacityAni?.InsertKeyFrame(1f, 1f, new ExponentialEaseOut());

                var parent = AssociatedObject.GetVisualParent();
                var distance = (float)parent.Bounds.Width;

                var offsetAni = compositor?.CreateVector3KeyFrameAnimation();
                offsetAni.Target = "Offset";
                offsetAni!.Duration = TimeSpan.FromSeconds(0.35);

                offsetAni.InsertKeyFrame(0f, new(-distance, 0, 0), new ExponentialEaseIn());
                offsetAni.InsertKeyFrame(1f, Vector3.One, new ExponentialEaseOut());

                group.Add(scaleAni);
                group.Add(offsetAni);
                group.Add(opacityAni);
            }

            //set center point
            var size = compositionVisual!.Size;
            compositionVisual.CenterPoint = new(size.X / 2, size.Y / 2, compositionVisual.CenterPoint.Z);
            compositionVisual?.StartAnimationGroup(group);
        });
    }
}

[StyledProperty(typeof(bool), "IsForward")]
[StyledProperty(typeof(string), "PageKey")]
[StyledProperty(typeof(AvaloniaPageProvider), "PageProvider")]
public sealed partial class SettingNavigationToBehavior : Behavior<Control> {
    private object _pageCache;

    protected override void OnLoaded() {
        base.OnLoaded();
        PropertyChanged += OnPropertyChanged;
    }

    private async void OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
        if (e.Property == PageKeyProperty) {
            if (!string.IsNullOrEmpty(e.GetNewValue<string>())) {
                _pageCache = await Dispatcher.UIThread.InvokeAsync(() =>
                    PageProvider.GetPage(e.GetNewValue<string>()), DispatcherPriority.Background);

                Dispatcher.UIThread.Post(() => {
                    if (AssociatedObject is ContentControl control)
                        control.Content = _pageCache;
                });
            }

            RunAnimation();
        }
    }

    private void RunAnimation() {
        Dispatcher.UIThread.Post(() => {
            var compositionVisual = ElementComposition.GetElementVisual(AssociatedObject);
            var compositor = compositionVisual.Compositor;

            var group = compositor!.CreateAnimationGroup();
            if (IsForward) {
                var scaleAni = compositor!.CreateVector3KeyFrameAnimation();
                scaleAni.Target = "Scale";
                scaleAni!.Duration = TimeSpan.FromSeconds(0.70);

                scaleAni.InsertExpressionKeyFrame(0f, "this.FinalValue", new ExponentialEaseOut());
                scaleAni.InsertKeyFrame(1f, new(0.75f), new ExponentialEaseOut());

                var opacityAni = compositor?.CreateScalarKeyFrameAnimation();
                opacityAni!.Target = "Opacity";
                opacityAni!.Duration = TimeSpan.FromSeconds(0.50);

                opacityAni.InsertExpressionKeyFrame(0f, "this.FinalValue", new CubicEaseOut());
                opacityAni?.InsertKeyFrame(1f, 0f, new CubicEaseOut());

                group.Add(scaleAni);
                group.Add(opacityAni);
            } else {
                var scaleAni = compositor!.CreateVector3KeyFrameAnimation();
                scaleAni.Target = "Scale";
                scaleAni!.Duration = TimeSpan.FromSeconds(0.70);
                scaleAni.InsertKeyFrame(0f, Vector3.One, new ExponentialEaseOut());
                scaleAni.InsertKeyFrame(1f, Vector3.One, new ExponentialEaseOut());

                var opacityAni = compositor?.CreateScalarKeyFrameAnimation();
                opacityAni!.Target = "Opacity";
                opacityAni!.Duration = TimeSpan.FromSeconds(0.75);
                opacityAni?.InsertKeyFrame(0f, 0f, new ExponentialEaseOut());
                opacityAni?.InsertKeyFrame(1f, 1f, new ExponentialEaseOut());

                var parent = AssociatedObject.GetVisualParent();
                var distance = (float)parent.Bounds.Width;

                var offsetAni = compositor?.CreateVector3KeyFrameAnimation();
                offsetAni.Target = "Offset";
                offsetAni!.Duration = TimeSpan.FromSeconds(0.35);

                offsetAni.InsertKeyFrame(0f, new(distance, 0, 0), new ExponentialEaseIn());
                offsetAni.InsertKeyFrame(1f, Vector3.One, new ExponentialEaseOut());

                group.Add(scaleAni);
                group.Add(offsetAni);
                group.Add(opacityAni);
            }

            //set center point
            var size = compositionVisual!.Size;
            compositionVisual.CenterPoint = new(size.X / 2, size.Y / 2, compositionVisual.CenterPoint.Z);
            compositionVisual?.StartAnimationGroup(group);
        });
    }
}