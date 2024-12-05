using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Helpers;

public static class DefaultValueHelper
{
    public static object GetDefaultValue( Compilation compilation, IPropertySymbol property )
    {
        object defaultValue = null;

        // Getting default value of property
        var syntaxReference = property.DeclaringSyntaxReferences.FirstOrDefault();

        if ( syntaxReference?.GetSyntax() is PropertyDeclarationSyntax propertySyntax )
        {
            // Try to get constant value from property initializer
            Optional<object> constantValue = TryToGetConstantValueFromPropertyInitializer( compilation, propertySyntax );
            if ( constantValue.HasValue && constantValue.Value != null )
            {
                Logger.Log( $"Got constant value from property initializer: {constantValue.Value}" );
            }
            else
            {
                // Try to get default value from getter
                constantValue = TryToGetDefaultValueFromGetter( compilation, propertySyntax );
            }

            if ( constantValue is { HasValue: true, Value: not null } )
            {
                defaultValue = constantValue.Value;
            }


            if ( property.Type.TypeKind == TypeKind.Enum && defaultValue is not string )//
            {
                defaultValue = HandleEnums( property, constantValue.Value );
            }
        }

        defaultValue ??= GetDefaultValueOfType( property.Type );

        // For other types, assign default values
        // defaultValue ??= GetDefaultValueOfType( property.Type );
        return defaultValue;
    }

    private static Optional<object> TryToGetDefaultValueFromGetter( Compilation compilation,
        PropertyDeclarationSyntax propertySyntax )
    {
        // Access the getter accessor
        var getter = propertySyntax.AccessorList?.Accessors.FirstOrDefault( a => a.Kind() == SyntaxKind.GetAccessorDeclaration );
        if ( getter == null )
        {
            return new Optional<object>();
        }

        ExpressionSyntax returnExpression = null;

        // Handle expression-bodied getter (e.g., get => color;)
        if ( getter.ExpressionBody != null )
        {
            returnExpression = getter.ExpressionBody.Expression;
        }
        else
        {
            // Handle block-bodied getter (e.g., get { return color; })
            var statements = getter.Body?.Statements;
            if ( statements is { Count: 1 } )
            {
                if ( statements.Value[0] is ReturnStatementSyntax returnStatement )
                {
                    returnExpression = returnStatement.Expression;
                }
            }
        }

        // If the getter returns an identifier (likely the backing field)
        if ( returnExpression is not IdentifierNameSyntax identifierName )
        {
            return new Optional<object>();
        }

        var fieldName = identifierName.Identifier.Text;

        // Find the field in the class with the matching name
        if ( propertySyntax.Parent is not ClassDeclarationSyntax classDeclaration )
        {
            return new Optional<object>();
        }

        var fieldDeclaration = classDeclaration.Members
            .OfType<FieldDeclarationSyntax>()
            .FirstOrDefault( f => f.Declaration.Variables.Any( v => v.Identifier.Text == fieldName ) );

        if ( fieldDeclaration == null )
        {
            return new Optional<object>();
        }

        VariableDeclaratorSyntax variableDeclarator = fieldDeclaration.Declaration.Variables.First( v => v.Identifier.Text == fieldName );
        if ( variableDeclarator.Initializer == null )
        {
            return new Optional<object>();
        }

        ExpressionSyntax fieldInitializerValue = variableDeclarator.Initializer.Value;

        object expressionString = QualifyExpressionSyntax( compilation, fieldInitializerValue );
        return new Optional<object>( expressionString );
    }

    private static Optional<object> TryToGetConstantValueFromPropertyInitializer( Compilation compilation, PropertyDeclarationSyntax propertySyntax )
    {
        // Check if the property has an initializer (i.e., a default value assigned directly)
        if ( propertySyntax.Initializer is null )
        {
            // No initializer present
            // public int MyProperty { get; set; }
            return new Optional<object>();
        }
        // Get the expression representing the initializer value
        ExpressionSyntax initializerValue = propertySyntax.Initializer.Value;

        object expressionString = QualifyExpressionSyntax( compilation, initializerValue );
        return new Optional<object>( expressionString );
    }

    private static object QualifyExpressionSyntax( Compilation compilation, ExpressionSyntax expression )
    {
        var semanticModel = compilation.GetSemanticModel( expression.SyntaxTree );

        var rewriter = new FullyQualifiedNameRewriter( semanticModel );
        var qualifiedExpression = rewriter.Visit( expression );

        if ( qualifiedExpression.ToString().Contains( "Blazorise" ) )
        {//this will process constants, but only Blazorise constants. Blazorise.Snackbar.Constants.DefaultIntervalBeforeClose will return value (5000), otherwise the constant name (int.MaxValue)
            var symbolInfo = semanticModel.GetSymbolInfo( expression );
            if ( symbolInfo.Symbol is IFieldSymbol { IsConst: true } fieldSymbol )
            {
                // If the symbol is a constant field, return its value
                return fieldSymbol.ConstantValue;
            }
        }

        return qualifiedExpression.ToString();
    }

    private static string HandleEnums( IPropertySymbol property, object enumValue = null )
    {
        // If we have an enum value provided, use it; otherwise, default to zero
        object valueToFind = enumValue ?? 0;

        Logger.Log( $"HandleEnums: property '{property.Name}', enumValue: {enumValue}, valueToFind: {valueToFind}" );

        // Find the enum member corresponding to the value
        IFieldSymbol enumMember = property.Type.GetMembers().OfType<IFieldSymbol>()
            .FirstOrDefault( f => f.HasConstantValue && Equals( f.ConstantValue, valueToFind ) );

        if ( enumMember != null )
        {
            // Get the fully qualified name of the enum member
            var enumMemberName = $"{property.Type}.{enumMember.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat )}";

            Logger.Log( $"Found enum member: {enumMemberName}" );
            return enumMemberName;
        }
        else
        {
            // If no member with the value is found, use default expression
            var defaultValue = $"default({property.Type.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat )})";
            Logger.Log( $"No matching enum member found. Returning default value: {defaultValue}" );
            return defaultValue;
        }
    }

    private static string GetDefaultValueOfType( ITypeSymbol typeSymbol )
    {
        return typeSymbol.SpecialType switch
        {
            SpecialType.System_Boolean => "false",
            SpecialType.System_Char => "'\\0'",
            SpecialType.System_SByte or SpecialType.System_Byte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_Decimal or SpecialType.System_Single or SpecialType.System_Double
                => "0",
            SpecialType.System_String => null,
            _ => typeSymbol.IsReferenceType ? null : null

            //here is the way for handling the int?, Size?, etc : $"default({typeSymbol.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat )})"
        };
    }
}