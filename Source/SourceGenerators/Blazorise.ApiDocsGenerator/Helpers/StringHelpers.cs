using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Blazorise.ApiDocsGenerator.Dtos;
using Blazorise.ApiDocsGenerator.Extensions;
using Microsoft.CodeAnalysis;

namespace Blazorise.ApiDocsGenerator.Helpers;

/// <summary>
/// Methods that are too small to have their own file
/// </summary>
public class StringHelpers
{
    /// <summary>
    /// Mainly to ensure doubles has invariant culture. 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FormatProperly( object value )
    {
        string defaultValue;

        if ( value is bool boolValue )
        {
            // Explicitly format bool as lowercase true/false
            defaultValue = boolValue.ToString().ToLowerInvariant();
            return defaultValue;
        }
        if ( value is IFormattable formattable )//ensures doubles has invariant culture.
        {
            defaultValue = formattable.ToString( null, CultureInfo.InvariantCulture );
        }
        else
        {
            defaultValue = value.ToString();
        }
        return defaultValue;
    }


    public static string TypeToStringDetails( string value )
    {
        const string toReplace = "global::";
        if ( value.StartsWith( toReplace ) )
        {
            value = value.Substring( toReplace.Length );
        }
        const string toReplace2 = "Blazorise.";
        if ( value.StartsWith( toReplace2 ) )
        {
            value = value.Substring( toReplace2.Length );
        }
        return value;

    }



    internal static string ExtractFromXmlComment( ISymbol iSymbol, ExtractorParts part )
    {
        string xmlComment = iSymbol.GetDocumentationCommentXml();

        if ( string.IsNullOrWhiteSpace( xmlComment ) )
            return "No documentation available - no XML comment";

        // Check if the XML contains <inheritdoc/>
        if ( xmlComment.Contains( "<inheritdoc" ) )
        {
            return GetXmlCommentForInheritdocInterfaces( iSymbol,part );
        }

        var match = Regex.Match( xmlComment, $"<{part.GetXmlTag()}>(.*?)</{part.GetXmlTag()}>", RegexOptions.Singleline );
        if ( !match.Success )
            return  part.GetDefault();
        var text = match.Groups[1].Value.Trim();

        XmlCommentToHtmlConverter converter = new();
        text = converter.Convert( text );             
        return text;
    }
    private static string GetXmlCommentForInheritdocInterfaces( ISymbol iSymbol, ExtractorParts part )
    {
        foreach ( var interfaceSymbol in iSymbol.ContainingType.AllInterfaces.Where( x => x.ToDisplayString().StartsWith( "Blazorise" ) ) )
        {
            // Find if the interface has a member matching this symbol
            var matchingMember = interfaceSymbol.GetMembers()
                .FirstOrDefault( member => iSymbol.Equals( iSymbol.ContainingType.FindImplementationForInterfaceMember( member ), SymbolEqualityComparer.Default ) );

            if ( matchingMember != null )
            {
                return ExtractFromXmlComment( matchingMember, part );
            }
        }
        return "No documentation available - no XML comment from <inheritdoc>";

    }

    //just making sure bool is bool and not Boolean...
    internal static string GetSimplifiedTypeName( ITypeSymbol typeSymbol )
    {
        // Mapping for built-in C# types


        // Handle nullable types
        if ( typeSymbol.NullableAnnotation == NullableAnnotation.Annotated && typeSymbol is INamedTypeSymbol namedType )
        {
            return $"{GetSimplifiedTypeName( namedType.TypeArguments[0] )}?";
        }

        // Handle generic types
        if ( typeSymbol is INamedTypeSymbol genericType && genericType.IsGenericType )
        {
            var baseName = typeMap.ContainsKey( genericType.Name ) ? typeMap[genericType.Name] : genericType.Name;
            var typeArguments = string.Join( ", ", genericType.TypeArguments.Select( GetSimplifiedTypeName ) );
            return $"{baseName}<{typeArguments}>";
        }

        // Handle arrays
        if ( typeSymbol is IArrayTypeSymbol arrayType )
        {
            return $"{GetSimplifiedTypeName( arrayType.ElementType )}[]";
        }

        // Use the mapped name if available, otherwise fallback to the simple name
        return typeMap.TryGetValue( typeSymbol.Name, out var simplifiedName ) ? simplifiedName : typeSymbol.Name;
    }


    static readonly Dictionary<string, string> typeMap = new()
    {
        {
            "Boolean", "bool"
        },
        {
            "Byte", "byte"
        },
        {
            "SByte", "sbyte"
        },
        {
            "Int16", "short"
        },
        {
            "UInt16", "ushort"
        },
        {
            "Int32", "int"
        },
        {
            "UInt32", "uint"
        },
        {
            "Int64", "long"
        },
        {
            "UInt64", "ulong"
        },
        {
            "Single", "float"
        },
        {
            "Double", "double"
        },
        {
            "Decimal", "decimal"
        },
        {
            "String", "string"
        },
        {
            "Char", "char"
        },
        {
            "Object", "object"
        }
    };
}