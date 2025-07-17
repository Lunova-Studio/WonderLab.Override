using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.Transformation;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using MinecraftLaunch.Base.Utilities;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Controls.Experimental.Effect;
using WonderLab.Extensions;
using WonderLab.Media.Easings;
using WonderLab.SourceGenerator.Attributes;
using WonderLab.Utilities;

namespace WonderLab.Controls;

[StyledProperty(typeof(string), "ImagePath")]
[StyledProperty(typeof(double), "ShieldBackgroundOpacity")]
[StyledProperty(typeof(BackgroundType), "BackgroundType", BackgroundType.SolidColor)]
[TemplatePart("PART_CloseButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_MinimizeButton", typeof(Button), IsRequired = true)]
public partial class WonderWindow : Window {
    private Border _PART_BackgroundBorder;
    private SkiaShaderRenderer _PART_SkiaShaderRenderer;
    private ExperimentalAcrylicBorder _PART_AcrylicBlurMask;
    private CancellationTokenSource _cancellationTokenSource = new();

    protected override Type StyleKeyOverride => typeof(WonderWindow);

    protected override async void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        await Task.Delay(100);
        if (BackgroundType is BackgroundType.Bitmap) {
            _PART_BackgroundBorder.Margin = new(-50);
            _PART_BackgroundBorder.Effect = new BlurEffect() {
                Radius = 50
            };

            await Task.Delay(200);
            RunInitAnimation();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        var closeButton = e.NameScope.Find<Button>("PART_CloseButton");
        var minimizeButton = e.NameScope.Find<Button>("PART_MinimizeButton");
        var dragLayoutBorder = e.NameScope.Find<Border>("PART_DragLayoutBorder");

        _PART_BackgroundBorder = e.NameScope.Find<Border>("PART_Background");
        _PART_SkiaShaderRenderer = e.NameScope.Find<SkiaShaderRenderer>("PART_SkiaShaderRenderer");
        _PART_AcrylicBlurMask = e.NameScope.Find<ExperimentalAcrylicBorder>("PART_AcrylicBlurMask");

        closeButton.Click += (_, _) => Close();
        minimizeButton.Click += (_, _) => WindowState = WindowState.Minimized;
        dragLayoutBorder.PointerPressed += (_, arg) => BeginMoveDrag(arg);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == ImagePathProperty && BackgroundType is BackgroundType.Bitmap && IsLoaded)
            UpdateBackground((BackgroundType.None, BackgroundType.Bitmap));

        if (change.Property == BackgroundTypeProperty && IsLoaded)
            UpdateBackground(change.GetOldAndNewValue<BackgroundType>());

        if(change.Property == ShieldBackgroundOpacityProperty && BackgroundType is BackgroundType.Bitmap) {
            Dispatcher.UIThread.Post(() => {
                if (ShieldBackgroundOpacity is 0)
                    RunClearBlurAnimation();
                else
                    RunBlurAnimation();
            });
        }
    }

    private void UpdateBackground((BackgroundType oldValue, BackgroundType newValue) values) {
        if (values.oldValue == values.newValue)
            return;

        _PART_SkiaShaderRenderer.Stop();
        _PART_AcrylicBlurMask.IsVisible = !EnvironmentUtil.IsWindow && values.newValue is BackgroundType.Acrylic;

        _PART_BackgroundBorder.IsVisible =
            values.newValue is BackgroundType.SolidColor or BackgroundType.Bitmap;

        if (EnvironmentUtil.IsWindow &&
                values.newValue is BackgroundType.Acrylic or BackgroundType.Mica) {

            Win32InteropUtil.SetWindowEffect(TryGetPlatformHandle().Handle,
                values.newValue is BackgroundType.Acrylic ? 3 : 4);
            return;
        }

        switch (values.newValue) {
            case BackgroundType.SolidColor:
                Bind(BackgroundProperty,
                    new DynamicResourceExtension("ApplicationBackgroundBrush"));
                break;
            case BackgroundType.Bitmap:
                RenderOptions.SetBitmapInterpolationMode(_PART_BackgroundBorder, BitmapInterpolationMode.LowQuality);
                if (File.Exists(ImagePath))
                    Background = new ImageBrush {
                        Source = new Bitmap(ImagePath),
                        Stretch = Stretch.UniformToFill,
                    };
                break;
            case BackgroundType.Acrylic:
                TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.None];
                break;
            case BackgroundType.Voronoi:
                _PART_SkiaShaderRenderer.SetEffect(SkiaEffect.FromEmbeddedResource("voronoi.sksl"));
                _PART_SkiaShaderRenderer.Start();
                break;
            case BackgroundType.Bubble:
                _PART_SkiaShaderRenderer.SetEffect(SkiaEffect.FromEmbeddedResource("bubble.sksl", BackgroundType.Bubble));
                _PART_SkiaShaderRenderer.Start();
                break;
            case BackgroundType.Mica:
                TransparencyLevelHint = [WindowTransparencyLevel.Mica, WindowTransparencyLevel.None];
                break;
        }
    }

    private async void RunInitAnimation() {
        await _PART_BackgroundBorder.Animate(MarginProperty)
            .WithEasing(new ExponentialEaseOut())
            .WithDuration(TimeSpan.FromSeconds(1))
            .From(_PART_BackgroundBorder.Margin)
            .To(new(0))
            .RunAsync(_cancellationTokenSource.Token);
    }

    private async void RunBlurAnimation() {
        using (_cancellationTokenSource) {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new();
        }

        await _PART_BackgroundBorder.Animate(MarginProperty)
            .WithEasing(new ExponentialEaseOut())
            .WithDuration(TimeSpan.FromSeconds(1))
            .From(_PART_BackgroundBorder.Margin)
            .To(new(-50))
            .RunAsync(_cancellationTokenSource.Token);
    }

    private async void RunClearBlurAnimation() {
        using (_cancellationTokenSource) {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new();
        }

        await _PART_BackgroundBorder.Animate(MarginProperty)
            .WithEasing(new ExponentialEaseOut())
            .WithDuration(TimeSpan.FromSeconds(1))
            .From(_PART_BackgroundBorder.Margin)
            .To(new(0))
            .RunAsync(_cancellationTokenSource.Token);
    }
}