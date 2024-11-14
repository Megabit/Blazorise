using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Blazorise.ApiDocsGenerator;


[Generator]
public class ComponentsApiDocsGenerator : IIncrementalGenerator
{
    public void Initialize( IncrementalGeneratorInitializationContext context )
    {
        var componentProperties = context.CompilationProvider  
            .Select( ( compilation, _ ) => GetComponentProperties( compilation ).ToImmutableArray() );
        context.RegisterSourceOutput( componentProperties, ( ctx, components ) =>  
        {
            var sourceText = GenerateComponentsApiSource( components ); 
            ctx.AddSource( "ComponentsApiSource.g.cs", SourceText.From( sourceText, Encoding.UTF8 ) );
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
            if ( type.TypeKind == TypeKind.Class && InheritsFrom( type, baseComponentSymbol ) )
            {
                var parameterProperties = type
                    .GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where( p => p.DeclaredAccessibility == Accessibility.Public &&
                        p.GetAttributes().Any( attr => SymbolEqualityComparer.Default.Equals( attr.AttributeClass, parameterAttributeSymbol ) ) );

                yield return ( type, parameterProperties );
            }
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

    private static string GenerateComponentsApiSource( ImmutableArray<(INamedTypeSymbol ComponentType, IEnumerable<IPropertySymbol> Properties)> components )
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
            var propertiesData = string.Join( ",\n", component.Properties.Select( property =>
            {
                var name = property.Name;
                var type = GetSimplifiedTypeName(property.Type);
                var xmlComment = ExtractSummary( property.GetDocumentationCommentXml() );

                string defaultValue = type switch
                {
                    "bool" => "false",
                    "string" => "string.Empty",
                    "int" => "0",
                    "double" => "0.0",
                    _ => null
                };

                
                var formattedDefaultValue = defaultValue == null ? "null" : $"\"{defaultValue}\"";

                return $"""
                             new ("{name}", "{type}", {formattedDefaultValue}, "{xmlComment}")
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
                             public ApiDocsForComponentProperty(string name, string type, string defaultValue, string description)
                             {
                                 Name = name;
                                 Type = type;
                                 DefaultValue = defaultValue;
                                 Description = description;
                             }
                 
                             public string Name { get; }
                             public string Type { get; }
                             public string DefaultValue { get; }
                             public string Description { get; }
                         }
                 """;
    }

    private static string ExtractSummary(string xmlComment)
    {
        if (string.IsNullOrWhiteSpace(xmlComment))
            return "No documentation available - no XML comment";

        var match = Regex.Match(xmlComment, @"<summary>(.*?)</summary>", RegexOptions.Singleline);
        if ( !match.Success )
            return "No summary found";
        // Sanitize the entire content first to prevent script injection
        var sanitizedText = SanitizeForHtml(match.Groups[1].Value.Trim());

        // Replace <see cref> tags with bolded sanitized type names
        sanitizedText = Regex.Replace(sanitizedText, 
        @"&lt;see\s+cref=&quot;[TPFEMN]:(Blazorise\.)?(.*?)&quot;\s*/&gt;",//also removes the "Blazorise." prefix 
        m =>
        {
            var typeName = m.Groups[2].Value; // Extract the type name
            return $"<strong>{typeName}</strong>"; // Wrap the  type name in <strong>
        });

        // Remove line breaks within the summary
        sanitizedText = sanitizedText.Replace("\n", " ").Replace("\r", "");

        return sanitizedText;

    }

    private static string SanitizeForHtml(string input)
    {
        // Escape HTML special characters to prevent injection
        return input
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    
    //just making sure bool is bool and not Boolean...
    private static string GetSimplifiedTypeName(ITypeSymbol typeSymbol)
    {
        // Mapping for built-in C# types
        var typeMap = new Dictionary<string, string>
        {
            { "Boolean", "bool" },
            { "Byte", "byte" },
            { "SByte", "sbyte" },
            { "Int16", "short" },
            { "UInt16", "ushort" },
            { "Int32", "int" },
            { "UInt32", "uint" },
            { "Int64", "long" },
            { "UInt64", "ulong" },
            { "Single", "float" },
            { "Double", "double" },
            { "Decimal", "decimal" },
            { "String", "string" },
            { "Char", "char" },
            { "Object", "object" }
        };

        // Handle nullable types
        if (typeSymbol.NullableAnnotation == NullableAnnotation.Annotated && typeSymbol is INamedTypeSymbol namedType)
        {
            return $"{GetSimplifiedTypeName(namedType.TypeArguments[0])}?";
        }

        // Handle generic types
        if (typeSymbol is INamedTypeSymbol genericType && genericType.IsGenericType)
        {
            var baseName = typeMap.ContainsKey(genericType.Name) ? typeMap[genericType.Name] : genericType.Name;
            var typeArguments = string.Join(", ", genericType.TypeArguments.Select(GetSimplifiedTypeName));
            return $"{baseName}<{typeArguments}>";
        }

        // Handle arrays
        if (typeSymbol is IArrayTypeSymbol arrayType)
        {
            return $"{GetSimplifiedTypeName(arrayType.ElementType)}[]";
        }

        // Use the mapped name if available, otherwise fallback to the simple name
        return typeMap.TryGetValue(typeSymbol.Name, out var simplifiedName) ? simplifiedName : typeSymbol.Name;
    }
    
    
    

}