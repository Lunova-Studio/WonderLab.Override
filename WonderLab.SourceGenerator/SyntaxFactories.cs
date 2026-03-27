using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using WonderLab.SourceGenerator.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace WonderLab.SourceGenerator;

internal static class SyntaxFactories {
    internal static class AvaloniaObject {
        private static InvocationExpressionSyntax GetValueDeclaration(string propertyName) {
            var getterId = IdentifierName("GetValue");
            var argumentId = IdentifierName(propertyName);
            return InvocationExpression(getterId)
                .AddArgumentListArguments(Argument(argumentId));
        }

        private static AccessorDeclarationSyntax GetValueGetterDeclaration(string propertyName) {
            var getterSyntax = GetValueDeclaration(propertyName);
            return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithExpressionBody(ArrowExpressionClause(getterSyntax))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static InvocationExpressionSyntax SetValueDeclaration(string propertyName) {
            var setterId = IdentifierName("SetValue");
            var argumentId = IdentifierName(propertyName);
            return InvocationExpression(setterId)
                .AddArgumentListArguments(Argument(argumentId), Argument(IdentifierName("value")));
        }

        private static AccessorDeclarationSyntax SetValueSetterDeclaration(string propertyName) {
            var setterSyntax = SetValueDeclaration(propertyName);
            return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithExpressionBody(ArrowExpressionClause(setterSyntax))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static AccessorDeclarationSyntax[] GetSetValueAccessorsDeclarations(string propertyName) {
            return [GetValueGetterDeclaration(propertyName), SetValueSetterDeclaration(propertyName)];
        }

        internal static PropertyDeclarationSyntax GetSetValuePropertyDeclaration(
            Type generatorType,
            string propertyTypeName,
            string avaloniaPropertyName,
            string accessorsName) {
            return PropertyDeclaration(IdentifierName(propertyTypeName), Identifier(accessorsName))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(
                    AttributeList(SyntaxFactoryUtil.ExcludeFromCodeCoverageAttributeSyntax()),
                    AttributeList(SyntaxFactoryUtil.CompilerGeneratedAttributeSyntax(generatorType)))
                .AddAccessorListAccessors(GetSetValueAccessorsDeclarations(avaloniaPropertyName));
        }

        internal static AccessorDeclarationSyntax SetAndRaiseSetterDeclaration(
            string propertyName,
            string backingFieldName) {
            var arguments = ArgumentList(SeparatedList([
                Argument(IdentifierName(propertyName)),
                Argument(IdentifierName(backingFieldName)).WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword)),
                Argument(IdentifierName("value"))
            ]));
            var setAndRaiseInvocation = InvocationExpression(IdentifierName("SetAndRaise"))
                .WithArgumentList(arguments);
            return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithExpressionBody(ArrowExpressionClause(setAndRaiseInvocation))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }
    }

    internal static class StyledProperty {
        private static InvocationExpressionSyntax RegisterMethodDeclaration(
            string controlTypeName,
            string propertyTypeName,
            string clrAccessorName,
            object defaultValue = null,
            ITypeSymbol propertyTypeSymbol = null) {
            TypeSyntax controlType = IdentifierName(controlTypeName);
            TypeSyntax propertyType = IdentifierName(propertyTypeName);
            var typeArgs = SeparatedList(new[] { controlType, propertyType });
            var method = GenericName(
                Identifier("global::Avalonia.AvaloniaProperty.Register"),
                TypeArgumentList(typeArgs));

            var clrLiteralExpression = LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(clrAccessorName));
            var methodArgs = new List<ArgumentSyntax> { Argument(clrLiteralExpression) };

            if (defaultValue is not null) {
                ExpressionSyntax defaultValueExpr = defaultValue switch {
                    string s => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(s)),
                    int i => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(i)),
                    float f => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(f)),
                    double d => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(d)),
                    bool b => LiteralExpression(b ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
                    _ when propertyTypeSymbol?.TypeKind == TypeKind.Enum => GenerateEnumDefaultExpression(propertyTypeSymbol, defaultValue),
                    _ => DefaultExpression(ParseTypeName(propertyTypeName))
                };

                methodArgs.Add(Argument(defaultValueExpr));
            }

            return InvocationExpression(method)
                .WithArgumentList(ArgumentList(SeparatedList(methodArgs)));
        }

        internal static FieldDeclarationSyntax Declaration(
            string controlTypeName,
            string propertyTypeName,
            string propertyName,
            string clrAccessorName,
            object defaultValue = null,
            ITypeSymbol propertyTypeSymbol = default) {
            TypeSyntax propType = IdentifierName(Identifier(propertyTypeName));

            var styledPropType = GenericName(
                Identifier("global::Avalonia.StyledProperty"),
                TypeArgumentList(SeparatedList(new[] { propType })));

            var invocation = RegisterMethodDeclaration(
                controlTypeName,
                propertyTypeName,
                clrAccessorName,
                defaultValue,
                propertyTypeSymbol);

            var varDeclarator = VariableDeclarator(Identifier(propertyName))
                .WithInitializer(EqualsValueClause(invocation));

            var variable = VariableDeclaration(styledPropType)
                .WithVariables(SingletonSeparatedList(varDeclarator));

            return FieldDeclaration(variable)
                .AddModifiers(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                    Token(SyntaxKind.ReadOnlyKeyword));
        }

        internal static ExpressionSyntax GenerateEnumDefaultExpression(ITypeSymbol typeSymbol, object value) {
            if (typeSymbol is not INamedTypeSymbol namedEnum || value is null)
                return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)value!));

            var field = namedEnum.GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(f => f.HasConstantValue && Equals(f.ConstantValue, value));

            var enumLiteral = field is not null
                ? $"{typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{field.Name}"
                : ((int)value!).ToString();

            return ParseExpression(enumLiteral);
        }
    }
}