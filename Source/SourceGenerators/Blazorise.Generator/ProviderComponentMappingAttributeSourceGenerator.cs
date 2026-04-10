using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Blazorise.Generator
{
    [Generator]
    public sealed class ProviderComponentMappingAttributeSourceGenerator : IIncrementalGenerator
    {
        public void Initialize( IncrementalGeneratorInitializationContext context )
        {
            IncrementalValuesProvider<PropertyDeclarationSyntax> componentMaps = context.SyntaxProvider.CreateSyntaxProvider(
                static ( node, _ ) => node is PropertyDeclarationSyntax property && property.Identifier.ValueText == "ComponentMap",
                static ( ctx, _ ) => (PropertyDeclarationSyntax)ctx.Node );

            IncrementalValueProvider<(Compilation compilation, ImmutableArray<PropertyDeclarationSyntax> componentMaps)> source =
                context.CompilationProvider.Combine( componentMaps.Collect() );

            context.RegisterSourceOutput( source, static ( productionContext, sourceContext ) =>
            {
                AddProviderComponentMappingsSource( productionContext, sourceContext.compilation, sourceContext.componentMaps );
            } );
        }

        private static void AddProviderComponentMappingsSource(
            SourceProductionContext context,
            Compilation compilation,
            ImmutableArray<PropertyDeclarationSyntax> componentMaps )
        {
            SortedDictionary<string, string> mappings = new( StringComparer.Ordinal );

            foreach ( PropertyDeclarationSyntax componentMap in componentMaps )
            {
                SemanticModel semanticModel = compilation.GetSemanticModel( componentMap.SyntaxTree );
                IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol( componentMap ) as IPropertySymbol;

                if ( !IsProviderComponentMap( propertySymbol )
                     || TryGetInitializer( componentMap ) is not InitializerExpressionSyntax initializer )
                {
                    continue;
                }

                foreach ( ExpressionSyntax expression in initializer.Expressions )
                {
                    if ( expression is not InitializerExpressionSyntax elementInitializer
                         || elementInitializer.Expressions.Count != 2
                         || elementInitializer.Expressions[0] is not TypeOfExpressionSyntax componentTypeOf
                         || elementInitializer.Expressions[1] is not TypeOfExpressionSyntax implementationTypeOf )
                    {
                        continue;
                    }

                    INamedTypeSymbol componentType = GetNamedTypeSymbol( semanticModel, componentTypeOf.Type );
                    INamedTypeSymbol implementationType = GetNamedTypeSymbol( semanticModel, implementationTypeOf.Type );

                    if ( !IsOpenGenericTypeDefinition( componentType )
                         || !IsOpenGenericTypeDefinition( implementationType ) )
                    {
                        continue;
                    }

                    mappings[GetTypeDefinitionSyntax( componentType )] = GetTypeDefinitionSyntax( implementationType );
                }
            }

            if ( mappings.Count == 0 )
            {
                return;
            }

            StringBuilder sb = new();
            sb.AppendLine( "#region Using directives" );
            sb.AppendLine( "using System;" );
            sb.AppendLine( "#endregion" );
            sb.AppendLine();

            foreach ( KeyValuePair<string, string> mapping in mappings )
            {
                sb.Append( "[assembly: global::Blazorise.ProviderComponentMapping( typeof( " );
                sb.Append( mapping.Key );
                sb.Append( " ), typeof( " );
                sb.Append( mapping.Value );
                sb.AppendLine( " ) )]" );
            }

            context.AddSource( "ProviderComponentMappings.g.cs", SourceText.From( sb.ToString(), Encoding.UTF8 ) );
        }

        private static bool IsProviderComponentMap( IPropertySymbol propertySymbol )
        {
            if ( propertySymbol is null
                 || !propertySymbol.IsStatic
                 || propertySymbol.Name != "ComponentMap"
                 || propertySymbol.ContainingType?.Name != "Config" )
            {
                return false;
            }

            return propertySymbol.Type.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat )
                == "global::System.Collections.Generic.IDictionary<global::System.Type, global::System.Type>";
        }

        private static InitializerExpressionSyntax TryGetInitializer( PropertyDeclarationSyntax propertyDeclaration )
        {
            if ( propertyDeclaration.ExpressionBody?.Expression is ObjectCreationExpressionSyntax expressionBodyObjectCreation )
            {
                return expressionBodyObjectCreation.Initializer;
            }

            AccessorDeclarationSyntax getter = propertyDeclaration.AccessorList?.Accessors.FirstOrDefault( x => x.IsKind( Microsoft.CodeAnalysis.CSharp.SyntaxKind.GetAccessorDeclaration ) );

            if ( getter?.Body is null )
            {
                return null;
            }

            foreach ( StatementSyntax statement in getter.Body.Statements )
            {
                if ( statement is ReturnStatementSyntax { Expression: ObjectCreationExpressionSyntax objectCreationExpression } )
                {
                    return objectCreationExpression.Initializer;
                }
            }

            return null;
        }

        private static INamedTypeSymbol GetNamedTypeSymbol( SemanticModel semanticModel, TypeSyntax typeSyntax )
        {
            return semanticModel.GetSymbolInfo( typeSyntax ).Symbol as INamedTypeSymbol
                   ?? semanticModel.GetTypeInfo( typeSyntax ).Type as INamedTypeSymbol;
        }

        private static bool IsOpenGenericTypeDefinition( INamedTypeSymbol typeSymbol )
        {
            return typeSymbol?.OriginalDefinition is INamedTypeSymbol originalDefinition
                   && originalDefinition.Arity > 0;
        }

        private static string GetTypeDefinitionSyntax( INamedTypeSymbol typeSymbol )
        {
            StringBuilder sb = new();
            AppendTypeDefinitionSyntax( sb, typeSymbol.OriginalDefinition );
            return sb.ToString();
        }

        private static void AppendTypeDefinitionSyntax( StringBuilder sb, INamedTypeSymbol typeSymbol )
        {
            if ( typeSymbol.ContainingType is not null )
            {
                AppendTypeDefinitionSyntax( sb, typeSymbol.ContainingType );
                sb.Append( '.' );
            }
            else
            {
                sb.Append( "global::" );

                if ( !typeSymbol.ContainingNamespace.IsGlobalNamespace )
                {
                    sb.Append( typeSymbol.ContainingNamespace.ToDisplayString() );
                    sb.Append( '.' );
                }
            }

            sb.Append( typeSymbol.Name );

            if ( typeSymbol.Arity > 0 )
            {
                sb.Append( '<' );

                if ( typeSymbol.Arity > 1 )
                {
                    sb.Append( ',', typeSymbol.Arity - 1 );
                }

                sb.Append( '>' );
            }
        }
    }
}