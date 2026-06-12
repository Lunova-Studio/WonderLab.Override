using Microsoft.CodeAnalysis;
using WonderLab.SourceGenerator.Extensions;

namespace WonderLab.SourceGenerator.Models;

internal record SettingsPropertyInfo(
    HierarchyInfo HierarchyInfo,
    string SettingsPropertyName,
    string PropertyName,
    string TypeName,
    IPropertySymbol PropertySymbol) {
    internal static SettingsPropertyInfo FromPropertySymbol(
        IPropertySymbol settingsPropertySymbol,
        IPropertySymbol modelPropertySymbol) {
        return new(
            HierarchyInfo.From(settingsPropertySymbol.ContainingType),
            settingsPropertySymbol.Name,
            modelPropertySymbol.Name,
            modelPropertySymbol.Type.GetFullyQualifiedNameWithNullabilityAnnotations(),
            modelPropertySymbol);
    }
}