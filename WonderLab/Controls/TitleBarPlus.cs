using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace WonderLab.Controls;

[TemplatePart("PART_CloseButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_MinimizeButton", typeof(Button), IsRequired = true)]
public sealed class TitleBarPlus : ContentControl {
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<TitleBarPlus, string>(nameof(Title), "WonderLab");

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        var closeButton = e.NameScope.Find<Button>("PART_CloseButton");
        var minimizeButton = e.NameScope.Find<Button>("PART_MinimizeButton");

        var dragLayoutBorder = e.NameScope.Find<Border>("PART_DragLayoutBorder");

        if (VisualRoot is Window window) {
            closeButton.Click += (_, _) => window.Close();
            minimizeButton.Click += (_, _) => window.WindowState = WindowState.Minimized;

            dragLayoutBorder.PointerPressed += (_, args) => {
                window.BeginMoveDrag(args);
            };
        }
    }
}