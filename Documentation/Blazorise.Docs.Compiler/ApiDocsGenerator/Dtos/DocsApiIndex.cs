using System.Collections.Generic;

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Dtos;

internal sealed class DocsApiIndex
{
    public string GeneratedUtc { get; set; }
    public List<DocsApiComponent> Components { get; set; }
}

internal sealed class DocsApiComponent
{
    public string Type { get; set; }
    public string TypeName { get; set; }
    public string Summary { get; set; }
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public string SearchUrl { get; set; }
    public List<DocsApiProperty> Parameters { get; set; }
    public List<DocsApiProperty> Events { get; set; }
    public List<DocsApiMethod> Methods { get; set; }
}

internal sealed class DocsApiProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string TypeName { get; set; }
    public string DefaultValue { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }
    public bool IsBlazoriseEnum { get; set; }
}

internal sealed class DocsApiMethod
{
    public string Name { get; set; }
    public string ReturnTypeName { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }
    public List<DocsApiMethodParameter> Parameters { get; set; }
}

internal sealed class DocsApiMethodParameter
{
    public string Name { get; set; }
    public string TypeName { get; set; }
}