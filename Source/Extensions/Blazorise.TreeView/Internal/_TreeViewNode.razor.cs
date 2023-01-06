#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Blazorise.Extensions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Blazorise.TreeView.Extensions;
#endregion

namespace Blazorise.TreeView.Internal;

public partial class _TreeViewNode<TNode> : BaseComponent
{
    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "tree-view" );
        builder.Append( "tree-view-collapsed", !Expanded );

        base.BuildClasses( builder );
    }

    private async Task ToggleNode( TreeViewNodeState<TNode> nodeState, bool refresh = true )
    {
        nodeState.Expanded = !nodeState.Expanded;

        if ( nodeState.Expanded )
        {
            if ( !ExpandedNodes.Contains( nodeState.Node ) )
            {
                ExpandedNodes.Add( nodeState.Node );
            }

            await ExpandedNodesChanged.InvokeAsync( ExpandedNodes );

            if ( nodeState.HasChildren )
            {
                var childNodes = GetChildNodesAsync is not null
                    ? await GetChildNodesAsync( nodeState.Node )
                    : GetChildNodes( nodeState.Node );

                if ( !nodeState.Children.Select( x => x.Node ).AreEqual( childNodes ) )
                {
                    nodeState.Children = new();

                    await foreach ( var childNodeState in childNodes.ToNodeStates( HasChildNodesAsync, HasChildNodes, ExpandedNodes.Intersect( childNodes ?? Enumerable.Empty<TNode>() ).Any() ) )
                    {
                        nodeState.Children.Add( childNodeState );
                    }
                }
            }
        }
        else
        {
            ExpandedNodes.Remove( nodeState.Node );
            await ExpandedNodesChanged.InvokeAsync( ExpandedNodes );
        }

        if ( refresh )
        {
            DirtyClasses();

            await InvokeAsync( StateHasChanged );
        }
    }

    public async Task ExpandAll()
    {
        foreach ( var nodeState in NodeStates ?? Enumerable.Empty<TreeViewNodeState<TNode>>() )
        {
            if ( HasChildNodes( nodeState.Node ) )
            {
                if ( !nodeState.Expanded )
                    await ToggleNode( nodeState, false );

                ExecuteAfterRender( async () =>
                {
                    if ( nodeState.ViewRef is not null )
                        await nodeState.ViewRef.ExpandAll();
                } );
            }
        }

        DirtyClasses();

        await InvokeAsync( StateHasChanged );
    }

    public async Task CollapseAll()
    {
        foreach ( var nodeState in NodeStates ?? Enumerable.Empty<TreeViewNodeState<TNode>>() )
        {
            if ( HasChildNodes( nodeState.Node ) )
            {
                if ( nodeState.Expanded )
                    await ToggleNode( nodeState, false );

                ExecuteAfterRender( async () =>
                {
                    if ( nodeState.ViewRef is not null )
                        await nodeState.ViewRef.CollapseAll();
                } );
            }
        }

        DirtyClasses();

        await InvokeAsync( StateHasChanged );
    }

    private Action<TNode, NodeStyling> ResolveNodeStylingAction( Action<TNode, NodeStyling> action )
    {
        return action ?? new Action<TNode, NodeStyling>( ( item, style ) => { return; } );
    }

    #endregion

    #region Properties

    [Parameter] public IEnumerable<TreeViewNodeState<TNode>> NodeStates { get; set; }

    [Parameter] public RenderFragment<TNode> NodeContent { get; set; }

    [Parameter] public IList<TNode> ExpandedNodes { get; set; } = new List<TNode>();

    [Parameter] public EventCallback<IList<TNode>> ExpandedNodesChanged { get; set; }

    [Parameter] public Func<TNode, IEnumerable<TNode>> GetChildNodes { get; set; }

    [Parameter] public Func<TNode, bool> HasChildNodes { get; set; } = node => true;

    [Parameter] public Func<TNode, Task<IEnumerable<TNode>>> GetChildNodesAsync { get; set; }

    [Parameter] public Func<TNode, Task<bool>> HasChildNodesAsync { get; set; }

    /// <summary>
    /// Defines if the treenode should be expanded.
    /// </summary>
    [Parameter] public bool Expanded { get; set; }

    /// <summary>
    /// Defines the name of the treenode expand icon.
    /// </summary>
    [Parameter] public IconName ExpandIconName { get; set; }

    /// <summary>
    /// Defines the style of the treenode expand icon.
    /// </summary>
    [Parameter] public IconStyle? ExpandIconStyle { get; set; }

    /// <summary>
    /// Defines the size of the treenode expand icon.
    /// </summary>
    [Parameter] public IconSize? ExpandIconSize { get; set; }

    /// <summary>
    /// Defines the name of the treenode collapse icon.
    /// </summary>
    [Parameter] public IconName CollapseIconName { get; set; }

    /// <summary>
    /// Defines the style of the treenode collapse icon.
    /// </summary>
    [Parameter] public IconStyle? CollapseIconStyle { get; set; }

    /// <summary>
    /// Defines the size of the treenode collapse icon.
    /// </summary>
    [Parameter] public IconSize? CollapseIconSize { get; set; }

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