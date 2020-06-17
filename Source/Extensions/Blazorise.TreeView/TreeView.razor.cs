﻿#region Using directives
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

        /// <summary>
        /// Collection of child TreeView items (child nodes)
        /// </summary>
        [Parameter] public IEnumerable<TNode> Nodes { get; set; }

        /// <summary>
        /// Template to display content for the node
        /// </summary>
        [Parameter] public RenderFragment<TNode> NodeContent { get; set; }

        /// <summary>
        /// Currently selected TreeView item/node.
        /// </summary>
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

        /// <summary>
        /// Occurs when the selected TreeView node has changed.
        /// </summary>
        [Parameter] public EventCallback<TNode> SelectedNodeChanged { get; set; }

        /// <summary>
        /// List of currently expanded TreeView items (child nodes).
        /// </summary>
        [Parameter] public IList<TNode> ExpandedNodes { get; set; } = new List<TNode>();

        /// <summary>
        /// Occurs when the collection of expanded nodes has changed.
        /// </summary>
        [Parameter] public EventCallback<IList<TNode>> ExpandedNodesChanged { get; set; }

        /// <summary>
        /// Gets the list of child nodes for each node.
        /// </summary>
        [Parameter] public Func<TNode, IEnumerable<TNode>> GetChildNodes { get; set; }

        /// <summary>
        /// Indicates if the node has child elements.
        /// </summary>
        [Parameter] public Func<TNode, bool> HasChildNodes { get; set; } = node => true;

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}