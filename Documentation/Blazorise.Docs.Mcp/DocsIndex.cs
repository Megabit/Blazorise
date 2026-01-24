using System.Collections.Generic;

namespace Blazorise.Docs.Mcp;

internal sealed class DocsIndex
{
    public string GeneratedUtc { get; set; }
    public List<DocsPage> Pages { get; set; }
}

internal sealed class DocsPage
{
    public string Route { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string PagePath { get; set; }
    public List<DocsExample> Examples { get; set; }
}

internal sealed class DocsExample
{
    public string Code { get; set; }
    public string Kind { get; set; }
    public string SourcePath { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}