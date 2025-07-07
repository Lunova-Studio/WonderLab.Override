using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Threading.Tasks;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

/// <summary>
/// 涟漪特效控件
/// </summary>
[StyledProperty(typeof(IBrush), "RippleFill")]
[StyledProperty(typeof(double), "RippleOpacity", 0.6)]
[StyledProperty(typeof(bool), "RaiseRippleCenter", false)]
public sealed partial class RippleControl : ContentControl {
    private Ripple _last;
    private byte _pointers;
    private Canvas PART_RippleCanvasRoot;

    public RippleControl() {
        AddHandler(PointerReleasedEvent, PointerReleasedHandler);
        AddHandler(PointerPressedEvent, PointerPressedHandler);
        AddHandler(PointerCaptureLostEvent, PointerCaptureLostHandler);
    }

    private void PointerPressedHandler(object sender, PointerPressedEventArgs e) {
        if (_pointers != 0 || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        _pointers++;
        Ripple ripple = _last = CreateRipple(e, RaiseRippleCenter);
        PART_RippleCanvasRoot.Children.Add(ripple);
        ripple.RunFirstStep();
    }

    private void PointerReleasedHandler(object sender, PointerReleasedEventArgs e) {
        RemoveLastRipple();
    }

    private void PointerCaptureLostHandler(object sender, PointerCaptureLostEventArgs e) {
        RemoveLastRipple();
    }

    private void RemoveLastRipple() {
        if (_last == null) return;

        _pointers--;
        OnReleaseHandler(_last);
        _last = null;
    }

    private void OnReleaseHandler(Ripple r) {
        r.RunSecondStep();
        _ = Task.Delay(Ripple.Duration).ContinueWith(RemoveRippleTask, TaskScheduler
            .FromCurrentSynchronizationContext());

        return;

        void RemoveRippleTask(Task arg1) {
            PART_RippleCanvasRoot.Children.Remove(r);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        PART_RippleCanvasRoot = e.NameScope.Find<Canvas>("PART_RippleCanvasRoot")!;
    }

    private Ripple CreateRipple(PointerPressedEventArgs e, bool center) {
        double width = Bounds.Width;
        double height = Bounds.Height;
        Ripple ripple = new Ripple(width, height) {
            Fill = RippleFill
        };

        if (center) {
            ripple.Margin = new Thickness(width / 2.0, height / 2.0, 0.0, 0.0);
        } else {
            ripple.SetupInitialValues(e, this);
        }
        return ripple;
    }
}

/// <summary>
/// 涟漪特效
/// </summary>
public sealed class Ripple : Ellipse {
    private readonly double _endX;
    private readonly double _endY;
    private readonly double _maxDiam;
    private static Easing Easing { get; set; } = new CubicEaseOut();

    public static readonly TimeSpan Duration = new(0, 0, 0, 0, 500);

    public Ripple(double outerWidth, double outerHeight) {
        InitializeTransitions();
        Width = 0.0;
        Height = 0.0;

        // Calculate the maximum diameter using Pythagorean theorem
        _maxDiam = Math.Sqrt(Math.Pow(outerWidth, 2.0) + Math.Pow(outerHeight, 2.0));
        _endY = _maxDiam - outerHeight;
        _endX = _maxDiam - outerWidth;
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Top;
        Opacity = 1.0;
    }

    public void SetupInitialValues(PointerPressedEventArgs e, Control parent) {
        Point position = e.GetPosition(parent);
        Margin = new Thickness(position.X, position.Y, 0.0, 0.0);
    }

    public void RunFirstStep() {
        Width = _maxDiam;
        Height = _maxDiam;
        Margin = new Thickness((0.0 - _endX) / 2.0, (0.0 - _endY) / 2.0, 0.0, 0.0);
    }

    public void RunSecondStep() {
        Opacity = 0.0;
    }

    private void InitializeTransitions() {
        Transitions = [
            new DoubleTransition {
                Duration = Duration,
                Easing = Easing,
                Property = WidthProperty
            },
            new DoubleTransition {
                Duration = Duration,
                Easing = Easing,
                Property = HeightProperty
            },
            new DoubleTransition {
                Duration = Duration,
                Easing = Easing,
                Property = OpacityProperty
            },
            new ThicknessTransition {
                Duration = Duration,
                Easing = Easing,
                Property = MarginProperty
            }
        ];
    }
}