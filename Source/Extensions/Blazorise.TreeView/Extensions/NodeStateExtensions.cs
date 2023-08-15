#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.TreeView.Internal;
#endregion

namespace Blazorise.TreeView.Extensions
{
    internal static class NodeStateExtensions
    {
        public static async IAsyncEnumerable<TreeViewNodeState<TNode>> ToNodeStates<TNode>( this IEnumerable<TNode> nodes,
            Func<TNode, Task<bool>> hasChildNodesAsync,
            Func<TNode, bool> hasChildNodes,
            Func<TNode, bool> isExpanded,
            Func<TNode, bool> isDisabled )
        {
            foreach ( var node in nodes ?? Enumerable.Empty<TNode>() )
            {
                var hasChildren = hasChildNodesAsync is not null
                    ? await hasChildNodesAsync( node )
                    : hasChildNodes( node );

                var expanded = isExpanded( node );
                var disabled = isDisabled( node );

                yield return new TreeViewNodeState<TNode>( node, hasChildren, expanded, disabled );
            }
        }

        public static async IAsyncEnumerable<TreeViewNodeState<TNode>> ToNodeStates<TNode>( this IList nodes,
            Func<TNode, Task<bool>> hasChildNodesAsync,
            Func<TNode, bool> hasChildNodes,
            Func<TNode, bool> isExpanded,
            Func<TNode, bool> isDisabled )
        {
            foreach ( var node in nodes )
            {
                if ( node is TNode tNode )
                {
                    var hasChildren = hasChildNodesAsync is not null
                        ? await hasChildNodesAsync( tNode )
                        : hasChildNodes( tNode );

                    var expanded = isExpanded( tNode );
                    var disabled = isDisabled( tNode );

                    yield return new TreeViewNodeState<TNode>( tNode, hasChildren, expanded, disabled );
                }
            }
        }
    }
}
