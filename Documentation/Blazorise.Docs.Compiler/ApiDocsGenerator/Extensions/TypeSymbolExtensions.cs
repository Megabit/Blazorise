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
        {
            if (componentType is INamedTypeSymbol nt && nt.IsGenericType)
            {
                var typeArguments = nt.TypeArguments.Select(
                x =>
                {
                    if ( x is INamedTypeSymbol t && t.IsGenericType )
                    {//this is for the case where generic param (in dictionary) has generic param of TItem (which is not valid, thus replacing with <object>)
                     //spotted in DataGrid._BaseDataGridRowEdit<>.CellValues (of type Dictionary<string, CellEditContext<TItem>> => Dictionary<string, CellEditContext<object>>)
                     //maybe this can be also used for other generics of generic where TItem lives, but currently there is no such case, thus the limit for Dictionary.
                        return $"{t.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat.WithGenericsOptions( SymbolDisplayGenericsOptions.None ) )}<object>";
                    }
                    //just a normal type like string (inside a Dictionary<string)
                    return x.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat.WithGenericsOptions( SymbolDisplayGenericsOptions.IncludeTypeParameters ) );
                }
                ); 
                return $"{nonGenericTypeName}<{string.Join(", ", typeArguments)}>";
            }
        }
        

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