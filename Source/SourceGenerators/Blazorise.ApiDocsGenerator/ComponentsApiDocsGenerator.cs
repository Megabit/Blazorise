using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.ApiDocsGenerator.Helpers;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

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
            Logger.LogAlways( DateTime.Now.ToLongTimeString() );

            var (compilation, components) = source;
            var sourceText = GenerateComponentsApiSource( compilation, components, ctx );
            ctx.AddSource( "ComponentsApiSource.g.cs", SourceText.From( sourceText, Encoding.UTF8 ) );

            ctx.AddSource( "Log.txt", SourceText.From( Logger.LogMessages, Encoding.UTF8 ) );

        } );
    }


    private static IEnumerable<(INamedTypeSymbol ComponentType, IEnumerable<IPropertySymbol> Properties)> GetComponentProperties( Compilation compilation )
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

            yield return ( type, parameterProperties );
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

    // private static string GenerateComponentsApiSource( ImmutableArray<(INamedTypeSymbol ComponentType, IEnumerable<IPropertySymbol> Properties)> components )
    private static string GenerateComponentsApiSource( Compilation compilation, ImmutableArray<(INamedTypeSymbol ComponentType, IEnumerable<IPropertySymbol> Properties)> components, SourceProductionContext ctx )

    {
        var componentsData = string.Join( "\n", components.Select( component =>
        {
            var componentType = component.ComponentType;

            // Check if the component is an open generic type
            if ( componentType.IsGenericType && componentType.TypeArguments.Any( ta => ta.TypeKind == TypeKind.TypeParameter ) )
            {
                // Skip open generic types, or alternatively, you could emit a simplified type name
                return string.Empty;// or handle as needed, e.g., skipping or special processing
            }

            var componentTypeName = componentType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );
            Logger.IsOn = component.ComponentType.Name == "Button";
            Logger.Log( component.ComponentType.Name );

            var propertiesData = string.Join( ",\n", component.Properties.Select( property =>
            {
                var name = property.Name;
                var typeName = OtherHelpers.GetSimplifiedTypeName( property.Type );
                var xmlComment = OtherHelpers.ExtractSummaryFromXmlComment( property.GetDocumentationCommentXml() );
                var isBlazoriseEnum = property.Type.TypeKind == TypeKind.Enum && property.Type.ToDisplayString().StartsWith( "Blazorise" );

                // Determine default value
                object defaultValue = DefaultValueHelper.GetDefaultValue( compilation, property );

                
                string defaultValueString =
                        defaultValue is null 
                            ? "null" 
                            :property.Type.Name switch
                            {
                                "String" => $""""
                                             $$"""
                                             {defaultValue}
                                             """
                                             """",
                                _ => OtherHelpers.FormatProperly( defaultValue )
                            };
                string defaultValueAsString = property.Type.Name == "String"? defaultValueString: $""""
                                               $$"""
                                               {OtherHelpers.TypeToStringDetails( defaultValueString)}
                                               """
                                               """";//lol
                

                return $"""
                             new ("{name}",typeof({property.Type}), "{typeName}", {defaultValueString},{defaultValueAsString}, "{xmlComment}", {( isBlazoriseEnum ? "true" : "false" )})
                        """;
            } ) );

            return $$"""
                         { typeof({{componentTypeName}}), new ApiDocsForComponent(typeof({{componentTypeName}}), new List<ApiDocsForComponentProperty>
                         {
                             {{propertiesData}}
                         })
                         },
                     """;
        } ) );

        return $$"""
                     using System;
                     using System.Collections.Generic;
                 
                     namespace Blazorise.Docs;
                 
                     public static class ComponentsApiDocsSource
                     {
                         public static readonly Dictionary<Type, ApiDocsForComponent> Components = new Dictionary<Type, ApiDocsForComponent>
                         {
                             {{componentsData}}
                         };
                     }
                     
                     public class ApiDocsForComponent
                         {
                             public ApiDocsForComponent(Type type, List<ApiDocsForComponentProperty> properties)
                             {
                                 Type = type;
                                 Properties = properties;
                             }
                 
                             public Type Type { get; }
                             public List<ApiDocsForComponentProperty> Properties { get; }
                         }  
                 
                         public class ApiDocsForComponentProperty
                         {
                             public ApiDocsForComponentProperty(string name,Type type, string typeName, object defaultValue,string defaultValueString, string description, bool isBlazoriseEnum)
                             {
                                 Name = name;
                                 TypeName = typeName;
                                 Type = type;
                                 DefaultValue = defaultValue;
                                 Description = description;
                                 IsBlazoriseEnum = isBlazoriseEnum;
                                 DefaultValueString = defaultValueString;
                             }
                              public bool IsBlazoriseEnum { get; }
                             public string Name { get; }
                             public string TypeName { get; }
                             public Type Type { get; }
                             public object DefaultValue { get; }
                             public string DefaultValueString { get; }
                             public string Description { get; }
                         }
                 """;
    }
}