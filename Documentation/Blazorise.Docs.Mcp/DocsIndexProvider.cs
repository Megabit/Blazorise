#region Using directives
using System;
#endregion

namespace Blazorise.Docs.Mcp;

internal static class DocsIndexProvider
{
    private static readonly Lazy<DocsIndex> Cached = new( () => DocsIndexLoader.Load<DocsIndex>( "docs-index.json" ) );

    public static DocsIndex GetIndex() => Cached.Value;
}