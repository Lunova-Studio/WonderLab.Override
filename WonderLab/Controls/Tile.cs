using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Input;
using WonderLab.SourceGenerator.Attributes;
using WonderLab.Utilities;

namespace WonderLab.Controls;

[StyledProperty(typeof(ICommand), "Command")]
[StyledProperty(typeof(object), "CommandParameter")]
[StyledProperty(typeof(bool), "IsAnimationTurn", false)]
[StyledProperty(typeof(bool), "IsEnableAnimation", true)]
public partial class Tile : ListBoxItem {
    private int _index;
    private int _totalCount;
    private TimeSpan _delay;
    private IChildIndexProvider _childIndexProvider;

    public static void RunParentPanelAnimation(Visual visual, bool isAniTurn = false) {
        var compositionVisual = ElementComposition.GetElementVisual(visual);
        Vector3KeyFrameAnimation offsetAni;

        if (isAniTurn)
            offsetAni = CompositionAnimationUtil.CreateVector3Animation(compositionVisual,
                new(0, 100, 0), Vector3.Zero, TimeSpan.FromSeconds(0.75), new ExponentialEaseOut());
        else
            offsetAni = CompositionAnimationUtil.CreateVector3Animation(compositionVisual,
                new(100, 0, (float)compositionVisual.Offset.Z), Vector3.Zero,
                TimeSpan.FromSeconds(0.75), new ExponentialEaseOut());

        compositionVisual.StartAnimation(CompositionAnimationUtil.PROPERTY_OFFSET, offsetAni);
    }

    private void RunAnimation() {
        var compositionVisual = ElementComposition.GetElementVisual(this);
        if (compositionVisual is null)
            return;

        var compositor = compositionVisual.Compositor;
        if (!IsEnableAnimation) {
            var oAni = CompositionAnimationUtil.CreateScalarAnimation(compositionVisual, 0, 1,
                TimeSpan.FromSeconds(0.6), new ExponentialEaseOut(), _delay);

            compositionVisual.StartAnimation(CompositionAnimationUtil.PROPERTY_OPACITY, oAni);
            return;
        }

        var scaleAni = CompositionAnimationUtil.CreateVector3Animation(compositionVisual,
            GetScaleOffset(_index, _totalCount), Vector3.One, TimeSpan.FromSeconds(0.60), new ExponentialEaseOut(), _delay);

        var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(compositionVisual, 0, 1,
            TimeSpan.FromMicroseconds(10 * (_delay.TotalMicroseconds is <= 0 ? 200 : _delay.TotalMicroseconds)),
                new CubicEaseOut(), _delay);

        scaleAni.Target = CompositionAnimationUtil.PROPERTY_SCALE;
        opacityAni.Target = CompositionAnimationUtil.PROPERTY_OPACITY;

        var group = compositor!.CreateAnimationGroup();
        group.Add(scaleAni);
        group.Add(opacityAni);

        var size = compositionVisual!.Size;
        if (IsAnimationTurn)
            compositionVisual!.CenterPoint = new Vector3((float)size.X / 2, (float)size.Y, (float)compositionVisual.CenterPoint.Z);
        else
            compositionVisual!.CenterPoint = new Vector3((float)size.X, (float)size.Y / 2, (float)compositionVisual.CenterPoint.Z);

        compositionVisual?.StartAnimationGroup(group);
    }

    private static Vector3 GetScaleOffset(int index, int total, double minOffset = 0.5) {
        double maxOffset = 0.75;
        double step = (maxOffset - minOffset) / (total - 1);

        return new((float)(maxOffset - index * step));
    }

    protected override async void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (_childIndexProvider is not null) {
            _index = _childIndexProvider.GetChildIndex(this);
            _delay = TimeSpan.FromMilliseconds(_index * 15);

            await Task.Delay(200);
        }

        _ = Dispatcher.UIThread.InvokeAsync(RunAnimation);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        if (this.GetVisualParent() is IChildIndexProvider provider)
            _childIndexProvider = provider;
        else
            throw new InvalidOperationException("Tile must be a child of IChildIndexProvider");

        if (!_childIndexProvider.TryGetTotalCount(out _totalCount))
            throw new InvalidOperationException("IChildIndexProvider must have a data source");

        Opacity = 0;
    }
}