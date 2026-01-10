using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Globalization;
using WonderLab;
using WonderLab.Classes.Generators;

namespace WonderLab.Controls;

[StyledProperty(typeof(IBrush), "Background")]
[StyledProperty(typeof(IBrush), "BorderBrush")]
[StyledProperty(typeof(bool), "ClipContent", true)]
[StyledProperty(typeof(double), "CornerSmoothing", 0.6)]
[StyledProperty(typeof(Thickness), "BorderThickness")]
[StyledProperty(typeof(CornerRadius), "CornerRadius")]
public sealed partial class SmoothBorder : Decorator {
    public override void Render(DrawingContext context) {
        var rect = new Rect(Bounds.Size);
        var geometry = CreateGeometry(rect, CornerRadius.TopLeft, CornerSmoothing);

        if (Background != null) {
            context.DrawGeometry(Background, null, geometry);
        }

        if (BorderBrush != null && BorderThickness.Left > 0) {
            var pen = new Pen(BorderBrush, BorderThickness.Left, lineJoin: PenLineJoin.Round);
            context.DrawGeometry(null, pen, geometry);
        }
    }

    protected override Size ArrangeOverride(Size finalSize) {
        var child = Child;
        if (child != null) {
            var childBounds = new Rect(finalSize)
                .Deflate(Padding);

            child.Arrange(childBounds);

            child.Clip = ClipContent
                ? CreateGeometry(childBounds, CornerRadius.TopLeft, CornerSmoothing)
                : null;
        }

        return finalSize;
    }

    protected override Size MeasureOverride(Size availableSize) {
        var child = Child;
        var padding = Padding;

        if (child != null) {
            var childConstraint = availableSize.Deflate(padding);
            child.Measure(childConstraint);
            return child.DesiredSize.Inflate(padding);
        }

        return new Size(padding.Left + padding.Right, padding.Top + padding.Bottom);
    }

    private static Geometry CreateGeometry(Rect rect, double cornerRadius, double cornerSmoothing) {
        if (cornerSmoothing <= 0) {
            RectangleGeometry rectangleGeometry = new(rect, cornerRadius, cornerRadius);
            return rectangleGeometry;
        }

        return SquirclePathGenerator.CreateGeometry(rect.Width, rect.Height, cornerRadius, cornerSmoothing);
    }
}