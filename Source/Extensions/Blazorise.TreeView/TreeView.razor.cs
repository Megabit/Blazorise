#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView
{
    public partial class TreeViewBase<TNode> : BaseComponent
    {
        #region members

        #endregion

        #region methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            //builder.Append( ClassProvider.TreeView()  );
            //builder.Append( ClassProvider.TreeViewHidden() );
            //builder.Append( ClassProvider.TreeViewIcon() );
            //builder.Append( ClassProvider.TreeViewTitle() );

            base.BuildClasses( builder );
        }

        protected void OnToggleNode( TNode node, bool expand )
        {
            var expanded = ExpandedNodes.Contains( node );

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

        [Parameter] public RenderFragment<TNode> TextContent { get; set; }

        [Parameter] public TNode SelectedNode { get; set; }
        [Parameter] public EventCallback<TNode> SelectedNodeChanged { get; set; }

        [Parameter] public Func<TNode, IEnumerable<TNode>> SetChildNodes { get; set; }

        [Parameter] public IList<TNode> ExpandedNodes { get; set; } = new List<TNode>();
        [Parameter] public EventCallback<IList<TNode>> ExpandedNodesChanged { get; set; }

        [Parameter] public string ExpandNodeIconClass { get; set; } = "far fa-plus-square cursor-pointer";
        [Parameter] public string CollapseNodeIconClass { get; set; } = "far fa-minus-square cursor-pointer";
        [Parameter] public string NodeTitleClass { get; set; } = "p-1 cursor-pointer";
        [Parameter] public string NodeTitleSelectedClass { get; set; } = "bg-primary text-white";

        [Parameter] public bool Visible { get; set; } = true;

        [Parameter] public Func<TNode, bool> HasChildNodes { get; set; } = node => true;

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}