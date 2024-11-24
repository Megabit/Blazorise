using System.Collections.Generic;
using Blazorise.ApiDocsGenerator.Dtos;
using Microsoft.CodeAnalysis;
using System.Linq;
using Blazorise.ApiDocsGenerator.Extensions;


namespace Blazorise.ApiDocsGenerator.Helpers;

public class InfoExtractor
{
    public static ApiDocsForComponentProperty GetPropertyDetails( Compilation compilation, IPropertySymbol property )
    {
        ApiDocsForComponentProperty propertyDetails = new();
        propertyDetails.Name = property.Name;
        propertyDetails.Type = property.Type.TypeKind == TypeKind.TypeParameter//with generic arguments:  
            ? "object"//typeof(TValue) is invalid => typeof(object)
            : property.Type.ToStringWithGenerics();//e.g.: typeof(EventCallback<TValue>) => typeof(EventCallback<>) 
        propertyDetails.TypeName = OtherHelpers.GetSimplifiedTypeName( property.Type );
        propertyDetails.Description = OtherHelpers.ExtractSummaryFromXmlComment( property );
        propertyDetails.IsBlazoriseEnum = property.Type.TypeKind == TypeKind.Enum && property.Type.ToDisplayString().StartsWith( "Blazorise" );

        // Determine default value
        object defaultValue = DefaultValueHelper.GetDefaultValue( compilation, property );


        string defaultValueString = defaultValue is null
            ? "null"
            : property.Type.Name switch
            {
                "String" => $""""
                             $$"""
                             {defaultValue}
                             """
                             """",
                _ => OtherHelpers.FormatProperly( defaultValue )
            };
        string defaultValueAsString = property.Type.Name == "String" ? defaultValueString : $""""
                                                                                             $$"""
                                                                                             {OtherHelpers.TypeToStringDetails( defaultValueString )}
                                                                                             """
                                                                                             """";
        propertyDetails.DefaultValueString = defaultValueAsString;
        propertyDetails.DefaultValue = defaultValueString;
        return propertyDetails;
    }





    public static ApiDocsForComponentMethod GetMethodDetails( IMethodSymbol method )
    {
        var methodName = method.Name;
        var returnTypeName = method.ReturnType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat );
        var description = OtherHelpers.ExtractSummaryFromXmlComment( method );

        var parameters = method.Parameters.Select( param => new ApiDocsForComponentMethodParameter
        {
            Name = param.Name, TypeName = param.Type.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
        } );


        var apiMethod = new ApiDocsForComponentMethod()
        {
            Name = methodName, ReturnTypeName = returnTypeName, Description = description, Parameters = parameters,
        };
        return apiMethod;

    }
}