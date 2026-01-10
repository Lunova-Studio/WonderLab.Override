using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Rendering.Composition;
using WonderLab.Extensions;

namespace WonderLab.Media.Attachments;

public static class AnimatedScrollExtensions {
    public static readonly AttachedProperty<bool> AnimatedScrollProperty =
        AvaloniaProperty.RegisterAttached<Visual, bool>("AnimatedScroll", typeof(Visual), defaultValue: false);

    static AnimatedScrollExtensions() {
        AnimatedScrollProperty.Changed.AddClassHandler<Visual>(HandleAnimatedScrollChanged);
    }

    public static bool GetAnimatedScroll(Visual obj) => obj.GetValue(AnimatedScrollProperty);
    public static void SetAnimatedScroll(Visual obj, bool value) => obj.SetValue(AnimatedScrollProperty, value);

    private static void HandleAnimatedScrollChanged(Visual visual, AvaloniaPropertyChangedEventArgs args) {
        if (args.NewValue is bool enable && enable) {
            visual.AttachedToVisualTree += (_, _) => {
                var compositionVisual = ElementComposition.GetElementVisual(visual);
                if (compositionVisual is not null)
                    ScrollableExtension.ApplyScrollAnimated(compositionVisual);
            };
        }
    }
}