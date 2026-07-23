using System.Collections.Generic;

namespace Blazorise;

internal sealed class DockNodeCollector
{
    private readonly List<DockNodeState> nodes = new();

    public void AddNode( DockNodeState node )
    {
        if ( node is not null && !nodes.Contains( node ) )
            nodes.Add( node );
    }

    public IReadOnlyList<DockNodeState> Nodes => nodes;
}