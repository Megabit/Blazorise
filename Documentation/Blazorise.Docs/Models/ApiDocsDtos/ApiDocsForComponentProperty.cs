#region Using directives
using System;
#endregion

namespace Blazorise.Docs.Models.ApiDocsDtos;

public class ApiDocsForComponentProperty : IApiDocsRecord
{
    public ApiDocsForComponentProperty( string name, Type type, string typeName, string defaultValueString, string summary, string remarks, bool isBlazoriseEnum )
    {
        Name = name;
        Type = type;
        TypeName = typeName;
        DefaultValueString = defaultValueString;
        Summary = summary;
        IsBlazoriseEnum = isBlazoriseEnum;
        Remarks = remarks;
    }
    public string Name { get; set; }
    public Type Type { get; set; }
    public string TypeName { get; set; }
    public string DefaultValueString { get; set; }
    public string DefaultValue { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }

    public bool IsBlazoriseEnum { get; set; }
}