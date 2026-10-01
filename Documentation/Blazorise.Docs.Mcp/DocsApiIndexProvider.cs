#region Using directives
using System;
#endregion

namespace Blazorise.Docs.Mcp;

internal static class DocsApiIndexProvider
{
    private static readonly Lazy<DocsApiIndex> Cached = new( () => DocsIndexLoader.Load<DocsApiIndex>( "docs-api-index.json" ) );

    public static DocsApiIndex GetIndex() => Cached.Value;
}