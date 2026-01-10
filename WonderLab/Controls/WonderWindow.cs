using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;

namespace WonderLab.Controls;

[StyledProperty(typeof(object), "TitleBarContent")]
[StyledProperty(typeof(Avalonia.Controls.Controls), "OverlayControls")]
public partial class WonderWindow : Window {
    protected override Type StyleKeyOverride => typeof(WonderWindow);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        var closeButton = e.NameScope.Find<Button>("PART_CloseButton");
        var minimizeButton = e.NameScope.Find<Button>("PART_MinimizeButton");
        var dragLayoutBorder = e.NameScope.Find<Border>("PART_DragLayoutBorder");

        //_PART_BackgroundBorder = e.NameScope.Find<Border>("PART_Background");
        //_PART_SkiaShaderRenderer = e.NameScope.Find<SkiaShaderRenderer>("PART_SkiaShaderRenderer");
        //_PART_AcrylicBlurMask = e.NameScope.Find<ExperimentalAcrylicBorder>("PART_AcrylicBlurMask");

        closeButton.Click += (_, _) => Close();
        minimizeButton.Click += (_, _) => WindowState = WindowState.Minimized;
        dragLayoutBorder.PointerPressed += (_, arg) => BeginMoveDrag(arg);
    }
}