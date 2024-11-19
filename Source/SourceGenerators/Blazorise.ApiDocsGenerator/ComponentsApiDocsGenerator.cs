using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.ApiDocsGenerator.Helpers;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Blazorise.ApiDocsGenerator.Dtos;

namespace Blazorise.ApiDocsGenerator;

[Generator]
public class ComponentsApiDocsGenerator : IIncrementalGenerator
{
    public void Initialize( IncrementalGeneratorInitializationContext context )
    {
        var componentProperties = context.CompilationProvider
            .Select( ( compilation, _ ) => ( compilation, components: GetComponentProperties( compilation ).ToImmutableArray() ) );
        
        

        context.RegisterSourceOutput( componentProperties, ( ctx, source ) =>
        {
            Logger.LogAlways( DateTime.Now.ToLongTimeString()  ); 

            var (compilation, components) = source;
            var sourceText = GenerateComponentsApiSource( compilation, components );
            ctx.AddSource( "ComponentsApiSource.g.cs", SourceText.From( sourceText, Encoding.UTF8 ) );

            ctx.AddSource( "Log.txt", SourceText.From( Logger.LogMessages, Encoding.UTF8 ) );

        } );
    }

    private static IEnumerable<(INamedTypeSymbol ComponentType, IEnumerable<IPropertySymbol> Properties, IEnumerable<IMethodSymbol> Methods)> GetComponentProperties( Compilation compilation )
    {
        var parameterAttributeSymbol = compilation.GetTypeByMetadataName( "Microsoft.AspNetCore.Components.ParameterAttribute" );
        if ( parameterAttributeSymbol == null )
            yield break;

        var baseComponentSymbol = compilation.GetTypeByMetadataName( "Blazorise.BaseComponent" );
        if ( baseComponentSymbol == null )
            yield break;

        var blazoriseNamespace = compilation.GlobalNamespace
            .GetNamespaceMembers()
            .FirstOrDefault( ns => ns.Name == "Blazorise" ); 

        if ( blazoriseNamespace is null )
            yield break;

        foreach ( var type in blazoriseNamespace.GetTypeMembers().OfType<INamedTypeSymbol>() )
        {
            if ( type.TypeKind != TypeKind.Class || !InheritsFrom( type, baseComponentSymbol ) )
                continue;
            var parameterProperties = type
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where( p => p.DeclaredAccessibility == Accessibility.Public &&
                    p.GetAttributes().Any( attr => SymbolEqualityComparer.Default.Equals( attr.AttributeClass, parameterAttributeSymbol ) ) );

            // Retrieve public methods
            var publicMethods = type
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where( m => m.DeclaredAccessibility == Accessibility.Public &&
                    !m.IsImplicitlyDeclared &&
                    m.MethodKind == MethodKind.Ordinary );

            
            yield return ( type, parameterProperties, publicMethods );
        }
    }

    private static bool InheritsFrom( INamedTypeSymbol type, INamedTypeSymbol baseType )
    {
        while ( type != null )
        {
            if ( SymbolEqualityComparer.Default.Equals( type.BaseType, baseType ) )
                return true;

            type = type.BaseType;
        }
        return false;
    }

    private static string GenerateComponentsApiSource( Compilation compilation, ImmutableArray<(INamedTypeSymbol ComponentType, IEnumerable<IPropertySymbol> Properties, IEnumerable<IMethodSymbol> Methods)> components )

    {
        Logger.LogAlways( components.Count().ToString());
        IEnumerable<ApiDocsForComponent> componentsData = components.Select( component =>
        {
            var componentType = component.ComponentType;

            // Check if the component is an open generic type
            if ( componentType.IsGenericType && componentType.TypeArguments.Any( ta => ta.TypeKind == TypeKind.TypeParameter ) )
            {
                // Skip open generic types, or alternatively, you could emit a simplified type name
                return null;// string.Empty;// or handle as needed, e.g., skipping or special processing
            }

            var componentTypeName = componentType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );
            Logger.IsOn = component.ComponentType.Name == "Button";
            Logger.Log( component.ComponentType.Name );

            var propertiesData = component.Properties.Select( property =>
                InfoExtractor.GetPropertyDetails( compilation, property ) );
            if(component.Methods.Any()) 
                Logger.LogAlways($"{component.Methods.Count()} commp . {component.ComponentType.Name}");
            var methodsData = component.Methods.Select(method => 
                InfoExtractor.GetMethodDetails( compilation, method ) );



            ApiDocsForComponent comp = new()
            {
                Type = componentTypeName, Properties = propertiesData,Methods = methodsData
            };
            return comp;
        } );


        
        

        return
            $$"""
                 using System;
                 using System.Collections.Generic;

                 namespace Blazorise.Docs;

                 public static class ComponentsApiDocsSource
                 {
                     public static readonly Dictionary<Type, ApiDocsForComponent> Components = new Dictionary<Type, ApiDocsForComponent>
                     {
                         {{componentsData.Where(comp=>comp is not null ).Select( comp => 
                         {
                             return $$"""
                                            { typeof({{comp.Type}}), new ApiDocsForComponent(typeof({{comp.Type}}), 
                                            new List<ApiDocsForComponentProperty>{
                                                {{
                                                    comp.Properties.Select( prop =>
                                                        $"""
                                                    
                                                         new ("{prop.Name}",typeof({prop.Type}), "{prop.TypeName}", {prop.DefaultValue},{prop.DefaultValueString}, "{prop.Description}", {( prop.IsBlazoriseEnum ? "true" : "false" )}),
                                                         """).StringJoin("\n")
                                                }}},
                                              new List<ApiDocsForComponentMethod>{
                                              {{
                                                  comp.Methods.Select( method =>
                                                      $$"""

                                                       new ("{{method.Name}}","{{method.ReturnTypeName}}", "{{method.Description}}",
                                                            new List<ApiDocsForComponentMethodParameter>{
                                                       {{
                                                        method.Parameters.Select(param => 
                                                            $"""
                                                             new ("{param.Name}","{param.TypeName}" ),
                                                             """
                                                            ).StringJoin("\n")
                                                       }} }),
                                                       """).StringJoin("\n")
                                              }}
                                              
                                              }  
                                        )},

                                    """;
                         }
                         ).StringJoin("\n")}}
                     };
                 }
                 """;
    }
}
