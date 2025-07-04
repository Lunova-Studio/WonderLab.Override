using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using WonderLab.SourceGenerator.Extensions;
using WonderLab.SourceGenerator.Models;

namespace WonderLab.SourceGenerator.Generators;

[Generator]
internal class StyledPropertyGenerator : AvaloniaPropertyGenerator, IIncrementalGenerator {
    private const string _attrName = "WonderLab.SourceGenerator.Attributes.StyledPropertyAttribute";

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
        foreach (var p in props) {
            yield return SyntaxFactories.StyledProperty.Declaration(
                p.HierarchyInfo.Hierarchy[0].QualifiedName,
                p.TypeName,
                p.PropertyName,
                p.AccessorName,
                p.DefaultValue);

            yield return SyntaxFactories.AvaloniaObject.GetSetValuePropertyDeclaration(
                typeof(StyledPropertyGenerator),
                p.TypeName,
                p.PropertyName,
                p.AccessorName);
        }
    }
}