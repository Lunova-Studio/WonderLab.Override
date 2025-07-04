using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using WonderLab.SourceGenerator.Models;

namespace WonderLab.SourceGenerator.Generators;

internal abstract class AvaloniaPropertyGenerator {
    private const string _attrName = "WonderLab.SourceGenerator.Attributes.StyledPropertyAttribute";

    protected static IncrementalValuesProvider<AvaloniaPropertyInfo> GetPropertiesFromGeneratorContext
        (IncrementalGeneratorInitializationContext context, string attributeName) {
        var classProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            attributeName,
            IsAttrClass,
            GetPropertyInfoFromClass).SelectMany((x, _) => x);

        return classProvider;
    }

    private static bool IsAttrClass(SyntaxNode syntaxNode, CancellationToken cancellationToken) {
        return syntaxNode is ClassDeclarationSyntax classDecl
            && classDecl.AttributeLists.Count > 0;
    }

    private static ImmutableArray<AvaloniaPropertyInfo> GetPropertyInfoFromClass(GeneratorAttributeSyntaxContext context, CancellationToken token) {
        var classSymbol = (INamedTypeSymbol)context.TargetSymbol;
        var result = ImmutableArray.CreateBuilder<AvaloniaPropertyInfo>();

        foreach (var attr in classSymbol.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() is _attrName)) {
            if (attr.ConstructorArguments.Length >= 2) {
                var typeArg = attr.ConstructorArguments[0];
                var nameArg = attr.ConstructorArguments[1];
                var defaultValueArg = attr.ConstructorArguments.Length > 2 ? attr.ConstructorArguments[2] : default;

                if (typeArg.Value is INamedTypeSymbol typeSymbol && nameArg.Value is string propertyName) {
                    var defaultValue = defaultValueArg.Value;
                    result.Add(AvaloniaPropertyInfo.FromTypeAndName(classSymbol, typeSymbol, propertyName, defaultValue));
                }
            }
        }

        return result.ToImmutable();
    }
}
