#region Using directives
using System;
using Blazorise.Extensions;
#endregion

namespace Blazorise.TreeView
{
    public struct TreeViewState<TNode> : IEquatable<TreeViewState<TNode>>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is TreeViewState<TNode> state && Equals( state );

        public bool Equals( TreeViewState<TNode> other )
        {
            return SelectedNode.IsEqual( other.SelectedNode );
        }

        public override int GetHashCode()
        {
            int result = 0;

            if ( SelectedNode != null )
                result ^= SelectedNode.GetHashCode();

            return result;
        }

        public static bool operator ==( TreeViewState<TNode> lhs, TreeViewState<TNode> rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( TreeViewState<TNode> lhs, TreeViewState<TNode> rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public TNode SelectedNode { readonly get; set; }

        #endregion
    }
}
