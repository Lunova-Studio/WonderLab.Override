using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Extensions;
using WonderLab.Media.Transitions;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;


[PseudoClasses(":press")]
[StyledProperty(typeof(BarState), "BarState", BarState.Collapsed)]
public partial class DynamicBar : ContentControl {
    // 记录拖拽起点、初始偏移和实时偏移
    private double _startX;
    private double _initialOffsetX;
    private double _offsetX;

    private bool _isDragging;
    private bool _canOpenPanel;
    
    private Panel _PART_Layout;
    private Border _PART_LayoutBorder;
    private Border _PART_ContentLayoutBorder;

    private CancellationTokenSource _cts = new();
    private readonly DynamicBarTransition _barTransition = new();

    public DynamicBar() {
        Loaded += OnLoaded;
    }

    private void SetPseudoclasses(bool isPress)
        => PseudoClasses.Set(":press", isPress);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_Layout = e.NameScope.Find<Panel>("PART_Layout")!;
        _PART_LayoutBorder = e.NameScope.Find<Border>("PART_LayoutBorder")!;
        _PART_ContentLayoutBorder = e.NameScope.Find<Border>("PART_ContentLayoutBorder")!;

        _PART_LayoutBorder.PointerMoved += OnPointerMoved;
        _PART_LayoutBorder.PointerPressed += OnPointerPressed;
        _PART_LayoutBorder.PointerReleased += OnPointerReleased;

        _PART_LayoutBorder.RenderTransform = new TransformGroup {
            Children = { new TranslateTransform() }
        };
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) {
        // 初始淡入和滑入动画
        await Task.Delay(250);

        var fadeIn = _PART_LayoutBorder
            .Animate(OpacityProperty)
            .WithDuration(TimeSpan.FromSeconds(0.8))
            .From(0).To(1)
            .RunAsync(_cts.Token);

        var slideIn = _PART_LayoutBorder
            .Animate(TranslateTransform.XProperty)
            .WithDuration(TimeSpan.FromSeconds(0.5))
            .From(25).To(-16)
            .RunAsync(_cts.Token);

        var contentSlide = _PART_ContentLayoutBorder
            .Animate(TranslateTransform.XProperty)
            .WithEasing(new ExponentialEaseIn())
            .WithDuration(TimeSpan.FromSeconds(0.1))
            .From(-16).To(460)
            .RunAsync(_cts.Token);

        await Task.WhenAll(fadeIn, slideIn, contentSlide);
        _PART_ContentLayoutBorder.Opacity = 1;
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e) {
        if (BarState != BarState.Collapsed)
            return;

        var pt = e.GetCurrentPoint(_PART_LayoutBorder);
        if (!pt.Properties.IsLeftButtonPressed)
            return;

        SetPseudoclasses(true);
        _isDragging = true;

        var tt = (TranslateTransform)((TransformGroup)_PART_LayoutBorder.RenderTransform).Children[0];
        _initialOffsetX = tt.X;
        _startX = pt.Position.X;
    }

    private void OnPointerMoved(object sender, PointerEventArgs e) {
        if (!_isDragging || BarState != BarState.Collapsed)
            return;

        var curX = e.GetCurrentPoint(_PART_LayoutBorder).Position.X;
        var newX = _initialOffsetX + (curX - _startX);

        _offsetX = Math.Min(newX, _offsetX);

        if (_offsetX > -15 || _offsetX < -50)
            return;

        _canOpenPanel = _offsetX <= -40;

        var tt = (TranslateTransform)((TransformGroup)_PART_LayoutBorder.RenderTransform).Children[0];
        tt.X = _offsetX;
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e) {
        if (!_isDragging || BarState != BarState.Collapsed)
            return;

        SetPseudoclasses(false);
        _isDragging = false;

        if (e.InitialPressMouseButton == MouseButton.Left) {
            if (_canOpenPanel) {
                BarState = BarState.Expanded;
            } else {
                AnimateBackTo(-16);
            }
        }

        _offsetX = 0;
        _canOpenPanel = false;
    }

    private async void OnPointerCaptureLost(object sender, PointerCaptureLostEventArgs e) {
        var tt = (TranslateTransform)((TransformGroup)_PART_LayoutBorder.RenderTransform).Children[0];
        await _PART_LayoutBorder
            .Animate(TranslateTransform.XProperty)
            .WithDuration(TimeSpan.FromMilliseconds(300))
            .From(tt.X).To(0)
            .RunAsync(_cts.Token);
    }

    private void AnimateBackTo(double to) {
        _PART_LayoutBorder.Animate(TranslateTransform.XProperty)
            .WithDuration(TimeSpan.FromMilliseconds(300))
            .From(_offsetX)
            .To(to)
            .RunAsync(_cts.Token);
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == BarStateProperty) {
            // 取消所有进行中动画
            _cts.Cancel();
            _cts = new CancellationTokenSource();

            var (oldState, newState) = change.GetOldAndNewValue<BarState>();
            _barTransition.OldState = oldState;
            _barTransition.NewState = newState;

            await _barTransition.Start(
                _PART_LayoutBorder,
                _PART_ContentLayoutBorder,
                false,
                _cts.Token);
        }
    }
}

public enum BarState {
    Collapsed,
    Expanded,
    Hidden
}