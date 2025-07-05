using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(Carousel), "Target")]
[StyledProperty(typeof(CarouselButtonType), "ButtonType")]
public sealed partial class CarouselButtonBehavior : Behavior<Button> {
    protected override void OnAttached() {
        base.OnAttached();
        if (AssociatedObject is null)
            return;

        AssociatedObject.Click += OnClick;
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        AssociatedObject.Click -= OnClick;
    }

    private void OnClick(object sender, RoutedEventArgs e) {
        if (Target is null)
            return;

        switch (ButtonType) {
            case CarouselButtonType.Next:
                Target.Next();
                break;
            case CarouselButtonType.Previous:
                Target.Previous();
                break;
        }
    }
}

public enum CarouselButtonType {
    Next,
    Previous
}