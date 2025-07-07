using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

[StyledProperty(typeof(string), "Icon")]
[StyledProperty(typeof(string), "Header", "Header")]
public sealed partial class SettingCard : ContentControl {
    private Grid _PART_Layout;
    private ContentPresenter _PART_ContentPresenter;

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (_PART_ContentPresenter is not null && _PART_ContentPresenter.Bounds.Width > Bounds.Width / 2) {
            Grid.SetRow(_PART_ContentPresenter, 1);
            Grid.SetColumn(_PART_ContentPresenter, 1);
            _PART_ContentPresenter.Margin = new(12, 0, 0, 0);
            _PART_Layout.RowSpacing = 8;
        }
    }

    protected override Size MeasureOverride(Size availableSize) {
        if (_PART_ContentPresenter.Bounds.Width > availableSize.Width / 2) {
            Grid.SetRow(_PART_ContentPresenter, 1);
            Grid.SetColumn(_PART_ContentPresenter, 1);
            _PART_ContentPresenter.Margin = new(12, 0, 0, 0);
            _PART_Layout.RowSpacing = 8;
        } else {
            Grid.SetRow(_PART_ContentPresenter, 0);
            Grid.SetColumn(_PART_ContentPresenter, 3);
            _PART_ContentPresenter.Margin = new(0);
            _PART_Layout.RowSpacing = 0;
        }

        return base.MeasureOverride(availableSize);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_Layout = e.NameScope.Find<Grid>("PART_LayoutGrid");
        _PART_ContentPresenter = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
    }
}

[StyledProperty(typeof(string), "Icon")]
[StyledProperty(typeof(object), "Footer")]
[StyledProperty(typeof(object), "Header", "Header")]
[StyledProperty(typeof(bool), "IsExpanded")]
[StyledProperty(typeof(bool), "CanExpanded")]
public sealed partial class SettingExpandCard : ItemsControl;