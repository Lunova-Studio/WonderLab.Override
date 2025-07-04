using Microsoft.CodeAnalysis;

namespace WonderLab.SourceGenerator.Extensions;

internal static class ISymbolExtensions {
    internal static string GetFullyQualifiedName(this ISymbol symbol) {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    internal static string GetFullyQualifiedNameWithNullabilityAnnotations(this ISymbol symbol) {
        var format = SymbolDisplayFormat.FullyQualifiedFormat
            .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        return symbol.ToDisplayString(format);
    }
}