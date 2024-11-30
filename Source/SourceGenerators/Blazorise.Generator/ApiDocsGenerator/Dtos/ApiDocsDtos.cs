using System.Collections.Generic;

namespace Blazorise.Generator.ApiDocsGenerator.Dtos;

/// <summary>
/// Easier to gather necessary info.
/// Almost keeps parity with Blazorise/Models/ApiDocsDtos.cs, changes here should be reflected there
/// </summary>
public class ApiDocsForComponent
{
    public ApiDocsForComponent( string type, string typeName, 
        IEnumerable<ApiDocsForComponentProperty> properties,
        IEnumerable<ApiDocsForComponentMethod> methods,
        IEnumerable<string> inheritsFromChain )
    {
        Type = type;
        TypeName = typeName;
        Properties = properties;
        Methods = methods;
        InheritsFromChain = inheritsFromChain;
    }

    public string Type { get; }
    
    public string TypeName { get; }
    public IEnumerable<ApiDocsForComponentProperty> Properties { get; } 
    public IEnumerable<ApiDocsForComponentMethod> Methods { get; } 
    
    public IEnumerable<string> InheritsFromChain { get; } 
}

public class ApiDocsForComponentProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string TypeName { get; set; }
    public string DefaultValue { get; set; }
    public string DefaultValueString { get; set; }
    public string Summary { get; set; }
    public bool IsBlazoriseEnum { get; set; }
    public string Remarks { get; set; }

}

public class ApiDocsForComponentMethod
{
    public string Name { get; set; }
    public string ReturnTypeName { get; set; }
    public string Summary { get; set; }
    public IEnumerable<ApiDocsForComponentMethodParameter> Parameters { get; set; }
    public string Remarks { get; set; }
}

public class ApiDocsForComponentMethodParameter
{
    public string Name { get; set; }
    public string TypeName { get; set; }
}

