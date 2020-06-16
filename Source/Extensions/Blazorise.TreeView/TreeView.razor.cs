#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView
{
    public partial class TreeViewBase<TNode> : BaseComponent
    {
        #region Members

        private bool visible = true;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "tree-view" );
            builder.Append( "tree-view-hidden", !Visible );

            base.BuildClasses( builder );
        }

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

            StateHasChanged();
        }

        protected void OnSelectNode( TNode node )
        {
            SelectedNode = node;
            SelectedNodeChanged.InvokeAsync( node );
        }

        #endregion

        #region Properties

        [Parameter] public IEnumerable<TNode> Nodes { get; set; }

        [Parameter] public RenderFragment<TNode> NodeContent { get; set; }

        [Parameter] public TNode SelectedNode { get; set; }

        [Parameter] public EventCallback<TNode> SelectedNodeChanged { get; set; }

        [Parameter] public Func<TNode, IEnumerable<TNode>> SetChildNodes { get; set; }

        [Parameter] public IList<TNode> ExpandedNodes { get; set; } = new List<TNode>();

        [Parameter] public EventCallback<IList<TNode>> ExpandedNodesChanged { get; set; }

        protected string NodeTitleClass
            => $"{ClassProvider.Spacing( Spacing.Padding, SpacingSize.Is1, Side.All, Breakpoint.None )} cursor-pointer";

        protected string NodeTitleSelectedClass
            => $"{ClassProvider.BackgroundColor( Background.Primary )} {ClassProvider.TextColor( TextColor.White )}";

        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                if ( value == visible )
                    return;

                visible = value;

                DirtyClasses();
            }
        }

        [Parameter] public Func<TNode, bool> HasChildNodes { get; set; } = node => true;

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}