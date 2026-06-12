using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WonderLab.SourceGenerator.Extensions;
using WonderLab.SourceGenerator.Models;

namespace WonderLab.SourceGenerator.Generators;

[Generator]
internal class StyledPropertyGenerator : AvaloniaPropertyGenerator, IIncrementalGenerator {
    private const string _attrName = "WonderLab.StyledPropertyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var propertyInfo = GetPropertiesFromGeneratorContext(context, _attrName);
        var groupedPropertyInfo = propertyInfo.GroupBy(static p => p.HierarchyInfo, static p => p);

        context.RegisterSourceOutput(groupedPropertyInfo, Execute);
    }

    private static void Execute(
        SourceProductionContext context,
        (HierarchyInfo, ImmutableArray<AvaloniaPropertyInfo>) propsPerClass) {
        var (hierarchy, props) = propsPerClass;

        var declarations = StyledPropsAndClrAccessorsDeclarations(props)
            .ToImmutableArray();

        var compilationUnit = hierarchy.GetCompilationUnit(declarations);
        var srcText = compilationUnit.GetText(Encoding.UTF8).ToString();

        context.AddSource($"{hierarchy.FilenameHint}.g.cs", srcText);
    }

    private static IEnumerable<MemberDeclarationSyntax> StyledPropsAndClrAccessorsDeclarations(IEnumerable<AvaloniaPropertyInfo> props) {
        foreach (var (hierarchyInfo, typeName, propertyName, accessorName, _, typeSymbol, defaultValue) in props) {
            yield return SyntaxFactories.StyledProperty.Declaration(
                controlTypeName: hierarchyInfo.Hierarchy[0].QualifiedName,
                propertyTypeName: typeName,
                propertyTypeSymbol: typeSymbol,
                propertyName: propertyName,
                clrAccessorName: accessorName,
                defaultValue: defaultValue);

            yield return SyntaxFactories.AvaloniaObject.GetSetValuePropertyDeclaration(
                typeof(StyledPropertyGenerator),
                typeName,
                propertyName,
                accessorName);
        }
    }
}