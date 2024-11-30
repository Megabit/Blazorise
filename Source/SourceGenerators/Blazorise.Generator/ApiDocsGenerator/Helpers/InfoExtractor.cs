using System.Linq;
using Blazorise.Generator.ApiDocsGenerator.Dtos;
using Blazorise.Generator.ApiDocsGenerator.Extensions;
using Microsoft.CodeAnalysis;

namespace Blazorise.Generator.ApiDocsGenerator.Helpers;

public class InfoExtractor
{
    public static ApiDocsForComponentProperty GetPropertyDetails( Compilation compilation, IPropertySymbol property )
    {
        ApiDocsForComponentProperty propertyDetails = new();
        propertyDetails.Name = property.Name;
        propertyDetails.Type = property.Type.TypeKind == TypeKind.TypeParameter//with generic arguments:  
            ? "object"//typeof(TValue) is invalid => typeof(object)
            : property.Type.ToStringWithGenerics();//e.g.: typeof(EventCallback<TValue>) => typeof(EventCallback<>) 
        propertyDetails.TypeName = StringHelpers.GetSimplifiedTypeName( property.Type );
        propertyDetails.Summary = StringHelpers.ExtractFromXmlComment( property, ExtractorParts.Summary );
        propertyDetails.Remarks = StringHelpers.ExtractFromXmlComment( property, ExtractorParts.Remarks );
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
                _ => StringHelpers.FormatProperly( defaultValue )
            };
        string defaultValueAsString = property.Type.Name == "String" ? defaultValueString : $""""
                                                                                             $$"""
                                                                                             {StringHelpers.TypeToStringDetails( defaultValueString, propertyDetails.Type )}
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
        var summary = StringHelpers.ExtractFromXmlComment( method,ExtractorParts.Summary );
        var remarks = StringHelpers.ExtractFromXmlComment( method, ExtractorParts.Remarks );

        var parameters = method.Parameters.Select( param => new ApiDocsForComponentMethodParameter
        {
            Name = param.Name, TypeName = param.Type.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
        } );

        var apiMethod = new ApiDocsForComponentMethod()
        {
            Name = methodName, ReturnTypeName = returnTypeName, Summary = summary,Remarks = remarks , Parameters = parameters,
        };
        return apiMethod;

    }
}