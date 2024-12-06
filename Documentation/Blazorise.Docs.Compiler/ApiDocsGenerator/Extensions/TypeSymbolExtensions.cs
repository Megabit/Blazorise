#region Using directives
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
#endregion

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Extensions;

public static class TypeSymbolExtensions
{
    public static string ToStringWithGenerics( this ITypeSymbol componentType )
    {

        if ( componentType == null )
            throw new ArgumentNullException( nameof( componentType ) );
        string nonGenericTypeName = componentType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat.WithGenericsOptions( SymbolDisplayGenericsOptions.None ) );
        if ( nonGenericTypeName.Split( "." ).Last() == "Dictionary" )//for dictionaries, otherwise it leaves with Dictionary<> 
            return componentType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat.WithGenericsOptions( SymbolDisplayGenericsOptions.IncludeTypeParameters ) );

        return
            nonGenericTypeName +
            $"{( componentType is INamedTypeSymbol
            {
                IsGenericType: true, //EventCallback<TValue> => EventCallback<> . Good for typeof(EventCallback<>)
                NullableAnnotation: not NullableAnnotation.Annotated // such as int? is actually generic Nullable<int>, but we want int?, not int<>?
            } namedType
                ? $"<{new string( ',', namedType.TypeParameters.Length - 1 )}>" // 
                : "" )}";
    }
}