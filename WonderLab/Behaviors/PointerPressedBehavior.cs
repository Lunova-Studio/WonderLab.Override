using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System.Windows.Input;

namespace WonderLab.Behaviors;

public sealed class PointerPressedBehavior : Behavior<Control> {
    public ICommand Command {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<PointerPressedBehavior, ICommand>(nameof(Command));

    public static readonly StyledProperty<object> CommandParameterProperty =
        AvaloniaProperty.Register<PointerPressedBehavior, object>(nameof(CommandParameter));

    protected override void OnAttachedToVisualTree() {
        base.OnAttachedToVisualTree();
        AssociatedObject.PointerPressed += OnPointerPressed;
    }

    protected override void OnDetachedFromVisualTree() {
        base.OnDetachedFromVisualTree();
        AssociatedObject.PointerPressed -= OnPointerPressed;
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e) {
        if (Command.CanExecute(CommandParameter)) {
            Command.Execute(CommandParameter);
        }
    }
}