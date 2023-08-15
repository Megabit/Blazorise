using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Blazorise.TreeView;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Pages.Tests;

public partial class TreeViewPage : ComponentBase
{
    TreeView<NodeInfo> treeViewRef;
    private IList<NodeInfo> expandedNodes = new List<NodeInfo>();
    private IList<NodeInfo> selectedNodes = new List<NodeInfo>();
    private NodeInfo selectedNode;
    private TreeViewSelectionMode selectionMode;

    public class NodeInfo
    {
        public string Text { get; set; }
        public IEnumerable<NodeInfo> Children { get; set; }
        public bool Disabled { get; set; }
    }

    private IEnumerable<NodeInfo> Nodes = new[]
    {
        new NodeInfo { Text = "NodeInfo 1" },
        new NodeInfo
        {
            Text = "NodeInfo 2",
            Children = new []
            {
                new NodeInfo { Text = "NodeInfo 2.1" },
                new NodeInfo
                {
                    Text = "NodeInfo 2.2", Children = new []
                    {
                        new NodeInfo { Text = "NodeInfo 2.2.1" },
                        new NodeInfo { Text = "NodeInfo 2.2.2" },
                        new NodeInfo { Text = "NodeInfo 2.2.3" },
                        new NodeInfo { Text = "NodeInfo 2.2.4" }
                    }
                },
                new NodeInfo { Text = "NodeInfo 2.3" },
                new NodeInfo { Text = "NodeInfo 2.4" }
            }
        },
        new NodeInfo { Text = "NodeInfo 3" },
        new NodeInfo
        {
            Text = "NodeInfo 4",
            Children = new []
            {
                new NodeInfo { Text = "NodeInfo 4.1" },
                new NodeInfo
                {
                    Text = "NodeInfo 4.2", Children = new []
                    {
                        new NodeInfo { Text = "NodeInfo 4.2.1" },
                        new NodeInfo { Text = "NodeInfo 4.2.2" },
                        new NodeInfo { Text = "NodeInfo 4.2.3" },
                        new NodeInfo { Text = "NodeInfo 4.2.4" }
                    }
                },
                new NodeInfo { Text = "NodeInfo 4.3" },
                new NodeInfo { Text = "NodeInfo 4.4" }
            }
        },
        new NodeInfo { Text = "NodeInfo 5" },
        new NodeInfo { Text = "NodeInfo 6" }
    };

    private async Task ForceReload()
    {
        await treeViewRef.Reload();
    }

    private async Task DisableRandomNode()
    {
        var allNodes = Flatten( Nodes, node => node.Children ).ToList();

        if ( allNodes.Any() )
        {
            Random rand = new Random();
            int randomIndex = rand.Next( allNodes.Count );
            var node = allNodes[randomIndex];
            node.Disabled = true;
        }

        await treeViewRef.Reload();
    }

    private async Task DisableAll()
    {
        var allNodes = Flatten( Nodes, node => node.Children ).ToList();

        foreach ( var node in allNodes )
        {
            node.Disabled = true;
        }

        await treeViewRef.Reload();
    }

    private async Task EnableAllNodes()
    {
        var allNodes = Flatten( Nodes, node => node.Children ).ToList();

        foreach ( var node in allNodes )
        {
            node.Disabled = false;
        }

        await treeViewRef.Reload();
    }

    protected IEnumerable<T> Flatten<T>( IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector )
    {
        var queue = new Queue<T>( source );
        while ( queue.Count > 0 )
        {
            var current = queue.Dequeue();
            yield return current;
            var children = childrenSelector( current );
            if ( children == null )
                continue;
            foreach ( var child in children )
                queue.Enqueue( child );
        }
    }
}