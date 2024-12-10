#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Dtos;

public class ApiDocsForComponentMethod
{
    public string Name { get; set; }
    public string ReturnTypeName { get; set; }
    public string Summary { get; set; }
    public IEnumerable<ApiDocsForComponentMethodParameter> Parameters { get; set; }
    public string Remarks { get; set; }
}