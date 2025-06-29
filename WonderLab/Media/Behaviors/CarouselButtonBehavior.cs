using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace WonderLab.Media.Behaviors;

public sealed class CarouselButtonBehavior : Behavior<Button> {
    public static readonly StyledProperty<Carousel> TargetProperty =
        AvaloniaProperty.Register<CarouselButtonBehavior, Carousel>(nameof(Target));

    public static readonly StyledProperty<CarouselButtonType> ButtonTypeProperty =
        AvaloniaProperty.Register<CarouselButtonBehavior, CarouselButtonType>(nameof(ButtonType));

    public Carousel Target {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public CarouselButtonType ButtonType {
        get => GetValue(ButtonTypeProperty);
        set => SetValue(ButtonTypeProperty, value);
    }

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