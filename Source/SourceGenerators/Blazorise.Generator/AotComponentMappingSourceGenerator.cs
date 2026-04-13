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
    public sealed class AotComponentMappingSourceGenerator : IIncrementalGenerator
    {
        private const string MappingAttributeMetadataName = "Blazorise.ProviderComponentMappingAttribute";

        public void Initialize( IncrementalGeneratorInitializationContext context )
        {
            IncrementalValuesProvider<InvocationExpressionSyntax> providerRegistrations = context.SyntaxProvider.CreateSyntaxProvider(
                static ( node, _ ) => node is InvocationExpressionSyntax,
                static ( ctx, _ ) => (InvocationExpressionSyntax)ctx.Node );

            IncrementalValuesProvider<GenericNameSyntax> genericTypeReferences = context.SyntaxProvider.CreateSyntaxProvider(
                static ( node, _ ) => node is GenericNameSyntax,
                static ( ctx, _ ) => (GenericNameSyntax)ctx.Node );

            IncrementalValueProvider<(Compilation compilation, ImmutableArray<InvocationExpressionSyntax> registrations, ImmutableArray<GenericNameSyntax> genericTypes)> source =
                context.CompilationProvider
                    .Combine( providerRegistrations.Collect() )
                    .Combine( genericTypeReferences.Collect() )
                    .Select( static ( tuple, _ ) => (tuple.Left.Left, tuple.Left.Right, tuple.Right) );

            context.RegisterSourceOutput( source, static ( productionContext, sourceContext ) =>
            {
                AddAotComponentMappingsSource( productionContext, sourceContext.compilation, sourceContext.registrations, sourceContext.genericTypes );
            } );
        }

        private static void AddAotComponentMappingsSource(
            SourceProductionContext context,
            Compilation compilation,
            ImmutableArray<InvocationExpressionSyntax> registrations,
            ImmutableArray<GenericNameSyntax> genericTypes )
        {
            ImmutableArray<ProviderMapping> providerMappings = GetProviderMappings( compilation );

            if ( providerMappings.Length == 0 )
            {
                return;
            }

            ImmutableHashSet<string> activeProviderAssemblies = GetActiveProviderAssemblies( compilation, registrations );

            if ( activeProviderAssemblies.Count == 0 )
            {
                ImmutableHashSet<string> discoveredProviderAssemblies = providerMappings
                    .Select( x => x.ProviderAssemblyName )
                    .ToImmutableHashSet( StringComparer.Ordinal );

                if ( discoveredProviderAssemblies.Count != 1 )
                {
                    return;
                }

                activeProviderAssemblies = discoveredProviderAssemblies;
            }

            if ( activeProviderAssemblies.Count != 1 )
            {
                return;
            }

            string providerAssemblyName = activeProviderAssemblies.Single();

            Dictionary<string, ProviderMapping> providerMappingsByComponent = providerMappings
                .Where( x => string.Equals( x.ProviderAssemblyName, providerAssemblyName, StringComparison.Ordinal ) )
                .GroupBy( x => x.ComponentTypeDefinitionDisplay, StringComparer.Ordinal )
                .ToDictionary( x => x.Key, x => x.First(), StringComparer.Ordinal );

            if ( providerMappingsByComponent.Count == 0 )
            {
                return;
            }

            SortedDictionary<string, string> closedMappings = GetClosedMappings( compilation, genericTypes, providerMappingsByComponent );

            if ( closedMappings.Count == 0 )
            {
                return;
            }

            StringBuilder sb = new();
            sb.AppendLine( "using System;" );
            sb.AppendLine( "using System.Runtime.CompilerServices;" );
            sb.AppendLine();
            sb.AppendLine( "namespace Blazorise.Generated;" );
            sb.AppendLine();
            sb.AppendLine( "internal static class BlazoriseAotComponentMappings" );
            sb.AppendLine( "{" );
            sb.AppendLine( "    [ModuleInitializer]" );
            sb.AppendLine( "    internal static void Initialize()" );
            sb.AppendLine( "    {" );

            foreach ( KeyValuePair<string, string> mapping in closedMappings )
            {
                sb.Append( "        global::Blazorise.GeneratedComponentMappingRegistry.Register( typeof( " );
                sb.Append( mapping.Key );
                sb.Append( " ), typeof( " );
                sb.Append( mapping.Value );
                sb.AppendLine( " ) );" );
            }

            sb.AppendLine( "    }" );
            sb.AppendLine( "}" );

            context.AddSource( "BlazoriseAotComponentMappings.g.cs", SourceText.From( sb.ToString(), Encoding.UTF8 ) );
        }

        private static ImmutableArray<ProviderMapping> GetProviderMappings( Compilation compilation )
        {
            List<ProviderMapping> mappings = new();

            foreach ( IAssemblySymbol assemblySymbol in compilation.SourceModule.ReferencedAssemblySymbols )
            {
                foreach ( AttributeData attributeData in assemblySymbol.GetAttributes() )
                {
                    if ( attributeData.AttributeClass?.ToDisplayString() != MappingAttributeMetadataName
                         || attributeData.ConstructorArguments.Length != 2 )
                    {
                        continue;
                    }

                    if ( attributeData.ConstructorArguments[0].Value is not INamedTypeSymbol componentType
                         || attributeData.ConstructorArguments[1].Value is not INamedTypeSymbol implementationType )
                    {
                        continue;
                    }

                    mappings.Add( new ProviderMapping(
                        assemblySymbol.Name,
                        componentType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat ),
                        implementationType ) );
                }
            }

            return mappings.ToImmutableArray();
        }

        private static ImmutableHashSet<string> GetActiveProviderAssemblies( Compilation compilation, ImmutableArray<InvocationExpressionSyntax> registrations )
        {
            HashSet<string> activeProviderAssemblies = new( StringComparer.Ordinal );

            foreach ( InvocationExpressionSyntax registration in registrations )
            {
                SemanticModel semanticModel = compilation.GetSemanticModel( registration.SyntaxTree );
                SymbolInfo symbolInfo = semanticModel.GetSymbolInfo( registration );
                IMethodSymbol methodSymbol = symbolInfo.Symbol as IMethodSymbol;

                if ( methodSymbol is null
                     || !methodSymbol.Name.StartsWith( "Add", StringComparison.Ordinal )
                     || !methodSymbol.Name.EndsWith( "Providers", StringComparison.Ordinal )
                     || !methodSymbol.ContainingNamespace.ToDisplayString().StartsWith( "Blazorise", StringComparison.Ordinal ) )
                {
                    continue;
                }

                activeProviderAssemblies.Add( methodSymbol.ContainingAssembly.Name );
            }

            return activeProviderAssemblies.ToImmutableHashSet( StringComparer.Ordinal );
        }

        private static SortedDictionary<string, string> GetClosedMappings(
            Compilation compilation,
            ImmutableArray<GenericNameSyntax> genericTypes,
            Dictionary<string, ProviderMapping> providerMappingsByComponent )
        {
            SortedDictionary<string, string> mappings = new( StringComparer.Ordinal );

            foreach ( GenericNameSyntax genericType in genericTypes )
            {
                SemanticModel semanticModel = compilation.GetSemanticModel( genericType.SyntaxTree );
                ITypeSymbol resolvedType = semanticModel.GetTypeInfo( genericType ).Type;
                INamedTypeSymbol componentType = resolvedType as INamedTypeSymbol;

                if ( componentType is null
                     || componentType.IsUnboundGenericType )
                {
                    continue;
                }

                string componentDefinitionDisplay = componentType.OriginalDefinition.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );

                if ( !providerMappingsByComponent.TryGetValue( componentDefinitionDisplay, out ProviderMapping providerMapping ) )
                {
                    continue;
                }

                INamedTypeSymbol implementationType = providerMapping.ImplementationTypeDefinition.Construct( componentType.TypeArguments.ToArray() );
                string componentDisplay = componentType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );
                string implementationDisplay = implementationType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );

                mappings[componentDisplay] = implementationDisplay;
            }

            return mappings;
        }

        private sealed class ProviderMapping
        {
            public ProviderMapping( string providerAssemblyName, string componentTypeDefinitionDisplay, INamedTypeSymbol implementationTypeDefinition )
            {
                ProviderAssemblyName = providerAssemblyName;
                ComponentTypeDefinitionDisplay = componentTypeDefinitionDisplay;
                ImplementationTypeDefinition = implementationTypeDefinition;
            }

            public string ProviderAssemblyName { get; }

            public string ComponentTypeDefinitionDisplay { get; }

            public INamedTypeSymbol ImplementationTypeDefinition { get; }
        }
    }
}