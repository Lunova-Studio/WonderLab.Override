using Microsoft.CodeAnalysis;
using System;
using WonderLab.SourceGenerator.Extensions;

namespace WonderLab.SourceGenerator.Models;

internal record AvaloniaPropertyInfo(
    HierarchyInfo HierarchyInfo,
    string TypeName,
    string PropertyName,
    string AccessorName,
    string FieldName,
    ITypeSymbol TypeSymbol,
    object DefaultValue = default) {
    internal static AvaloniaPropertyInfo FromFieldSymbol(IFieldSymbol fieldSymbol) {
        string typeNameWithNullabilityAnnotations = fieldSymbol.Type.GetFullyQualifiedNameWithNullabilityAnnotations();
        string clrAccessorName = GetAccessorsName(fieldSymbol);
        string propertyName = $"{clrAccessorName}Property";

        return new(
            HierarchyInfo.From(fieldSymbol.ContainingType),
            typeNameWithNullabilityAnnotations,
            propertyName,
            clrAccessorName,
            fieldSymbol.Name,
            fieldSymbol.Type);
    }

    internal static AvaloniaPropertyInfo FromPropertySymbol(IPropertySymbol propertySymbol) {
        // 获取类型名（带可空性注解）
        string typeNameWithNullabilityAnnotations = propertySymbol.Type.GetFullyQualifiedNameWithNullabilityAnnotations();
        // 属性名本身
        string clrAccessorName = propertySymbol.Name;
        // AvaloniaProperty 字段名
        string propertyName = $"{clrAccessorName}Property";

        return new(
            HierarchyInfo.From(propertySymbol.ContainingType),
            typeNameWithNullabilityAnnotations,
            propertyName,
            clrAccessorName,
            null,
            propertySymbol.Type);
    }

    private static string GetAccessorsName(IFieldSymbol fieldSymbol) {
        var span = fieldSymbol.Name.AsSpan();
        if (fieldSymbol.Name.StartsWith("_"))
            span = span.Slice(1);

        var chars = span.ToArray();
        chars[0] = char.ToUpperInvariant(chars[0]);
        return new string(chars);
    }

    internal static AvaloniaPropertyInfo FromTypeAndName(
        INamedTypeSymbol classSymbol,
        INamedTypeSymbol typeSymbol,
        string propertyName,
        object defaultValue) {
        string typeNameWithNullabilityAnnotations = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string clrAccessorName = propertyName;
        string styledPropertyName = $"{propertyName}Property";

        return new(
            HierarchyInfo.From(classSymbol),
            typeNameWithNullabilityAnnotations,
            styledPropertyName,
            clrAccessorName,
            null,
            typeSymbol,
            defaultValue);
    }
}
