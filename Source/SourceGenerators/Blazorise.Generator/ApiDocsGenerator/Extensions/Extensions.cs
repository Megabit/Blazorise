using System;
using System.Collections.Generic;
using Blazorise.Generator.ApiDocsGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace Blazorise.Generator.ApiDocsGenerator.Extensions;  

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

        return
            componentType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGenericsOptions(SymbolDisplayGenericsOptions.None)) +
            $"{(componentType is INamedTypeSymbol
            {
                IsGenericType: true , //EventCallback<TValue> => EventCallback<> . Good for typeof(EventCallback<>)
                NullableAnnotation: not NullableAnnotation.Annotated // such as int? is actually generic Nullable<int>, but we want int?, not int<>?
            } ? "<>" : "")}";
    }
}

