#region Using directives
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView
{
    public partial class _TreeViewNode<TNode> : BaseComponent
    {
        #region Members

        private bool expanded = true;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "tree-view" );
            builder.Append( "tree-view-collapsed", !Expanded );

            base.BuildClasses( builder );
        }

        //protected Task OnToggleNode()
        //{
        //    Expanded = !Expanded;

        //    return Task.CompletedTask;
        //}

        protected void OnToggleNode( TNode node, bool expand )
        {
            bool expanded = ExpandedNodes.Contains( node );

            if ( expanded && !expand )
            {
                ExpandedNodes.Remove( node );
                ExpandedNodesChanged.InvokeAsync( ExpandedNodes );
            }
            else if ( !expanded && expand )
            {
                ExpandedNodes.Add( node );
                ExpandedNodesChanged.InvokeAsync( ExpandedNodes );
            }

            InvokeAsync( StateHasChanged );
        }

        private Action<TNode, NodeStyling> ResolveNodeStylingAction( Action<TNode, NodeStyling> action )
        {
            return action ?? new Action<TNode, NodeStyling>( ( item, style ) => { return; } );
        }

        #endregion

        #region Properties

        [Parameter] public IEnumerable<TNode> Nodes { get; set; }

        [Parameter] public RenderFragment<TNode> NodeContent { get; set; }

        [Parameter] public IList<TNode> ExpandedNodes { get; set; } = new List<TNode>();

        [Parameter] public EventCallback<IList<TNode>> ExpandedNodesChanged { get; set; }

        [Parameter] public Func<TNode, IEnumerable<TNode>> GetChildNodes { get; set; }

        [Parameter] public Func<TNode, bool> HasChildNodes { get; set; } = node => true;

        [Parameter]
        public bool Expanded
        {
            get => expanded;
            set
            {
                if ( value == expanded )
                    return;

                expanded = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] public _TreeViewNode<TNode> ParentNode { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets selected node styling.
        /// </summary>
        [Parameter] public Action<TNode, NodeStyling> SelectedNodeStyling { get; set; }

        /// <summary>
        /// Gets or sets node styling.
        /// </summary>
        [Parameter] public Action<TNode, NodeStyling> NodeStyling { get; set; }

        #endregion
    }
}