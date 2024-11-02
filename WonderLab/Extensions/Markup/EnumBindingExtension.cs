using Avalonia.Markup.Xaml;
using System;

namespace WonderLab.Extensions.Markup;

public sealed class EnumBindingExtension : MarkupExtension {
    private readonly Type _enumType;

    public EnumBindingExtension(Type enumType) {
        _enumType = enumType;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return Enum.GetValues(_enumType);
    }
}