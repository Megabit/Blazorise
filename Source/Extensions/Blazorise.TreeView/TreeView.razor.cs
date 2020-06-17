#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView
{
    public partial class TreeView<TNode> : BaseComponent
    {
        #region Members

        private TreeViewStore<TNode> store = new TreeViewStore<TNode>
        {
        };

        #endregion

        #region Methods

        public void SelectNode( TNode node )
        {
            SelectedNode = node;

            StateHasChanged();
        }

        #endregion

        #region Properties

        [Parameter] public IEnumerable<TNode> Nodes { get; set; }

        [Parameter] public RenderFragment<TNode> NodeContent { get; set; }

        [Parameter]
        public TNode SelectedNode
        {
            get => store.SelectedNode;
            set
            {
                if ( EqualityComparer<TNode>.Default.Equals( store.SelectedNode, value ) )
                    return;

                store.SelectedNode = value;

                SelectedNodeChanged.InvokeAsync( store.SelectedNode );

                DirtyClasses();
            }
        }

        [Parameter] public EventCallback<TNode> SelectedNodeChanged { get; set; }

        [Parameter] public Func<TNode, IEnumerable<TNode>> SetChildNodes { get; set; }

        [Parameter] public IList<TNode> ExpandedNodes { get; set; } = new List<TNode>();

        [Parameter] public EventCallback<IList<TNode>> ExpandedNodesChanged { get; set; }

        [Parameter] public bool Visible { get; set; }

        [Parameter] public Func<TNode, bool> HasChildNodes { get; set; } = node => true;

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}