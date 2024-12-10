#region Using directives
#endregion

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Dtos;

public class ApiDocsForComponentProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string TypeName { get; set; }
    public string DefaultValueString { get; set; }
    public string Summary { get; set; }
    public bool IsBlazoriseEnum { get; set; }
    public string Remarks { get; set; }
}