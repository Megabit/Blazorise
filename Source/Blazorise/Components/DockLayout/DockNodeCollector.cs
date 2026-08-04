using System;
using System.Collections.Generic;

namespace Blazorise;

internal sealed class DockNodeCollector
{
    private readonly List<DockNodeState> nodes = new();

    private readonly Action nodesChanged;

    public DockNodeCollector( Action onNodesChanged = null )
    {
        nodesChanged = onNodesChanged;
    }

    public void AddNode( DockNodeState node )
    {
        if ( node is not null && !nodes.Contains( node ) )
        {
            nodes.Add( node );
            nodesChanged?.Invoke();
        }
    }

    public void RemoveNode( DockNodeState node )
    {
        if ( node is not null && nodes.Remove( node ) )
            nodesChanged?.Invoke();
    }

    public IReadOnlyList<DockNodeState> Nodes => nodes;
}