using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazorise.ApiDocsGenerator.Dtos;

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

    //here it is different
    public string Type { get; set; }
    
    public string TypeName { get; set; }
    public IEnumerable<ApiDocsForComponentProperty> Properties { get; set; } 
    public IEnumerable<ApiDocsForComponentMethod> Methods { get; set; } 
    
    public IEnumerable<string> InheritsFromChain { get; set; } 
}

public class ApiDocsForComponentProperty
{
    public string Name { get; set; }
    
    
    //different 
    public string Type { get; set; }
    public string TypeName { get; set; }
    
    // different
    public string DefaultValue { get; set; }
    public string DefaultValueString { get; set; }
    public string Description { get; set; }
    public bool IsBlazoriseEnum { get; set; }
    public string Remarks { get; set; }

}

public class ApiDocsForComponentMethod
{
    public string Name { get; set; }
    public string ReturnTypeName { get; set; }
    public string Description { get; set; }
    public IEnumerable<ApiDocsForComponentMethodParameter> Parameters { get; set; }
    public string Remarks { get; set; }
}

public class ApiDocsForComponentMethodParameter
{
    public string Name { get; set; }
    public string TypeName { get; set; }
}

