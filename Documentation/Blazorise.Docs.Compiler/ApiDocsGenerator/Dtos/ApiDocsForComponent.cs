#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Dtos;

/// <summary>
/// Easier to gather necessary info.
/// Almost keeps parity with Blazorise/Models/ApiDocsDtos.cs, changes here should be reflected there
/// </summary>
public class ApiDocsForComponent
{
    public ApiDocsForComponent( string type, string typeName,
        IEnumerable<ApiDocsForComponentProperty> properties,
        IEnumerable<ApiDocsForComponentMethod> methods,
        IEnumerable<string> inheritsFromChain,
        string category,
        string subcategory,
        string searchUrl
        )
    {
        Type = type;
        TypeName = typeName;
        Properties = properties;
        Methods = methods;
        InheritsFromChain = inheritsFromChain;
        Category = category;
        Subcategory = subcategory;
        SearchUrl = searchUrl;
    }

    public string SearchUrl { get; }

    public string Type { get; }

    public string TypeName { get; }
    public IEnumerable<ApiDocsForComponentProperty> Properties { get; }
    public IEnumerable<ApiDocsForComponentMethod> Methods { get; }

    public IEnumerable<string> InheritsFromChain { get; }

    public string Category { get; }
    public string Subcategory { get; }
}