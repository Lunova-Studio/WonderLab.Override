using Microsoft.CodeAnalysis;
using System.Text;

namespace WonderLab.SourceGenerator.Extensions;

internal static class TypeSymbolExtension {
    internal static string GetFullyQualifiedMetadataName(this ITypeSymbol symbol) {
        var builder = new StringBuilder();

        static void BuildFrom(ISymbol symbol, in StringBuilder builder) {
            switch (symbol) {
                case INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: false }:
                    BuildFrom(symbol.ContainingNamespace, in builder);
                    builder.Append('.');
                    builder.Append(symbol.MetadataName);
                    break;
                case INamespaceSymbol { IsGlobalNamespace: false }:
                    builder.Append(symbol.MetadataName);
                    break;
                case ITypeSymbol { ContainingSymbol: INamespaceSymbol { IsGlobalNamespace: true } }:
                    builder.Append(symbol.MetadataName);
                    break;
                case ITypeSymbol { ContainingSymbol: INamespaceSymbol namespaceSymbol }:
                    BuildFrom(namespaceSymbol, in builder);
                    builder.Append('.');
                    builder.Append(symbol.MetadataName);
                    break;
                case ITypeSymbol { ContainingSymbol: ITypeSymbol typeSymbol }:
                    BuildFrom(typeSymbol, in builder);
                    builder.Append('+');
                    builder.Append(symbol.MetadataName);
                    break;
            }
        }

        BuildFrom(symbol, in builder);

        return builder.ToString();
    }
}