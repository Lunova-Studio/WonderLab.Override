using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using WonderLab.SourceGenerator.Extensions;
using WonderLab.SourceGenerator.Models;

namespace WonderLab.SourceGenerator.Generators;

[Generator]
internal sealed class SettingsPropertyGenerator : IIncrementalGenerator {
    private const string _attrName = "WonderLab.SettingsAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var propertyInfo = context.SyntaxProvider.ForAttributeWithMetadataName(
            _attrName,
            IsAttrProperty,
            GetPropertyInfos).SelectMany((x, _) => x);

        var groupedPropertyInfo =
            propertyInfo.GroupBy(
                static x => x.HierarchyInfo,
                static x => x);

        context.RegisterSourceOutput(
            groupedPropertyInfo,
            Execute);
    }

    private static bool IsAttrProperty(
        SyntaxNode syntaxNode,
        CancellationToken cancellationToken) {
        return syntaxNode is PropertyDeclarationSyntax {
            AttributeLists.Count: > 0
        };
    }

    private static ImmutableArray<SettingsPropertyInfo> GetPropertyInfos(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token) {
        if (context.TargetSymbol is not IPropertySymbol settingsProperty)
            return ImmutableArray<SettingsPropertyInfo>.Empty;

        if (settingsProperty.Type is not INamedTypeSymbol modelType)
            return ImmutableArray<SettingsPropertyInfo>.Empty;

        var builder = ImmutableArray.CreateBuilder<SettingsPropertyInfo>();
        var properties = modelType
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => !x.IsStatic &&
                       !x.IsIndexer &&
                       x.DeclaredAccessibility == Accessibility.Public &&
                       x.GetMethod is not null);

        foreach (var property in properties)
            builder.Add(SettingsPropertyInfo
                .FromPropertySymbol(settingsProperty, property));

        return builder.ToImmutable();
    }

    private static void Execute(
        SourceProductionContext context,
        (HierarchyInfo, ImmutableArray<SettingsPropertyInfo>) propsPerClass) {
        var (hierarchy, props) = propsPerClass;

        var declarations = PropertyDeclarations(props)
            .ToImmutableArray();

        var compilationUnit = hierarchy.GetCompilationUnit(declarations);
        var srcText = compilationUnit.GetText(Encoding.UTF8).ToString();

        context.AddSource($"{hierarchy.FilenameHint}.g.cs", srcText);
    }

    private static IEnumerable<MemberDeclarationSyntax> PropertyDeclarations(IEnumerable<SettingsPropertyInfo> props) {
        return props.Select(property => SyntaxFactories
            .SettingsProperty
            .Declaration(property.SettingsPropertyName, property.PropertySymbol)).Cast<MemberDeclarationSyntax>();
    }
}