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
            Func<TNode, bool> hasChildNodesFunc,
            Func<TNode, bool> isExpandedFunc,
            Func<TNode, bool> isDisabledFunc )
        {
            foreach ( var node in nodes ?? Enumerable.Empty<TNode>() )
            {
                var hasChildren = hasChildNodesAsync is not null
                    ? await hasChildNodesAsync( node )
                    : hasChildNodesFunc( node );

                var isExpanded = isExpandedFunc( node );
                var isDisabled = isDisabledFunc( node );

                yield return new TreeViewNodeState<TNode>( node, hasChildren, isExpanded, isDisabled );
            }
        }

        public static async IAsyncEnumerable<TreeViewNodeState<TNode>> ToNodeStates<TNode>( this IList nodes,
            Func<TNode, Task<bool>> hasChildNodesAsync,
            Func<TNode, bool> hasChildNodesFunc,
            Func<TNode, bool> isExpandedFunc,
            Func<TNode, bool> isDisabledFunc )
        {
            foreach ( var node in nodes )
            {
                if ( node is TNode tNode )
                {
                    var hasChildren = hasChildNodesAsync is not null
                        ? await hasChildNodesAsync( tNode )
                        : hasChildNodesFunc( tNode );

                    yield return new TreeViewNodeState<TNode>( tNode, hasChildren, isExpandedFunc( tNode ), isDisabledFunc( tNode ) );
                }
            }
        }
    }
}
