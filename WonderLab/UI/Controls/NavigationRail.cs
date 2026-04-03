using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WonderLab.UI.Controls;

[StyledProperty(typeof(ICommand), "FABCommand")]
public sealed partial class NavigationRail : ItemsControl {
    private bool _isHidden = false;
    private Border _PART_RootBorder;

    private CancellationTokenSource _cancellationTokenSource = new();

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_RootBorder = e.NameScope.Find<Border>("PART_RootBorder");
    }
}