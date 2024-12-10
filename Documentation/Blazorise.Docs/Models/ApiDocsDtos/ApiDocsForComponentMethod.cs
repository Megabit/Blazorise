#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Docs.Models.ApiDocsDtos;

public class ApiDocsForComponentMethod
{
    public ApiDocsForComponentMethod( string name, string returnTypeName, string summary, string remarks, IReadOnlyList<ApiDocsForComponentMethodParameter> parameters )
    {
        Name = name;
        ReturnTypeName = returnTypeName;
        Summary = summary;
        Remarks = remarks;
        Parameters = parameters;
    }

    public string Name { get; set; }
    // public Type ReturnTypeSymbol { get; set; }
    public string ReturnTypeName { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }

    public IReadOnlyList<ApiDocsForComponentMethodParameter> Parameters { get; set; }
}