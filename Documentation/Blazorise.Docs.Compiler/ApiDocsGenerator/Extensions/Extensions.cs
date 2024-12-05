using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Docs.Compiler.ApiDocsGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Extensions;  

public static class EnumerableExtensions
{
    /// <summary>
    /// This looks much cleaner then wrapping all the stuff in string.Join. (check the usage)
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string StringJoin(this IEnumerable<string> source, string separator)
    {
        try
        {
            return source == null ? "" : string.Join(separator, source);
        }
        catch ( Exception e )
        {
            Logger.LogAlways(e.Message);
        }
        return "";

    }
}



public static class TypeSymbolExtensions
{
    public static string ToStringWithGenerics(this ITypeSymbol componentType)
    {
        
        if (componentType == null) throw new ArgumentNullException(nameof(componentType));
        string nonGenericTypeName = componentType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat.WithGenericsOptions( SymbolDisplayGenericsOptions.None ) );
        if ( nonGenericTypeName.Split( "." ).Last() == "Dictionary" )//for dictionaries, otherwise it leaves with Dictionary<> 
            return componentType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat.WithGenericsOptions( SymbolDisplayGenericsOptions.IncludeTypeParameters ) );
        
        return
            nonGenericTypeName +
            $"{(componentType is INamedTypeSymbol
            {
                IsGenericType: true , //EventCallback<TValue> => EventCallback<> . Good for typeof(EventCallback<>)
                NullableAnnotation: not NullableAnnotation.Annotated // such as int? is actually generic Nullable<int>, but we want int?, not int<>?
            } namedType
                ? $"<{new string(',', namedType.TypeParameters.Length - 1)}>" // 
                : "")}";
    }
}

