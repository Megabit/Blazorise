using Blazorise.ApiDocsGenerator.Dtos;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Blazorise.ApiDocsGenerator.Helpers;

public class InfoExtractor
{

    public static ApiDocsForComponentProperty GetPropertyDetails( Compilation compilation, IPropertySymbol property )
    {
        ApiDocsForComponentProperty propertyDetails = new();
        propertyDetails.Name = property.Name;
        propertyDetails.Type = property.Type.ToString();
        propertyDetails.TypeName = OtherHelpers.GetSimplifiedTypeName( property.Type );
        propertyDetails.Description= OtherHelpers.ExtractSummaryFromXmlComment( property.GetDocumentationCommentXml() );
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





    public static ApiDocsForComponentMethod GetMethodDetails( Compilation compilation, IMethodSymbol method )
    {
        var methodName = method.Name;
        // ITypeSymbol returnTypeSymbol = method.ReturnType;
        var returnTypeName = method.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        // var isStatic = method.IsStatic;
        // var isAsync = method.IsAsync;
        var description = OtherHelpers.ExtractSummaryFromXmlComment(method.GetDocumentationCommentXml());

        var parameters = method.Parameters.Select(param => new ApiDocsForComponentMethodParameter
        {
            Name = param.Name,
            TypeName = param.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
        });


        var apiMethod = new ApiDocsForComponentMethod(){
        Name = methodName,
        ReturnTypeName= returnTypeName,
        Description= description,
        Parameters= parameters,
        };
        return apiMethod;
        
    }
}

