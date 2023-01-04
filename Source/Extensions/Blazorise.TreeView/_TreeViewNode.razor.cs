#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Blazorise.Extensions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView;

public partial class _TreeViewNode<TNode> : BaseComponent
{
    #region Members

    private IEnumerable<TreeViewNodeState<TNode>> NodeStates;

    #endregion

    #region Methods

    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<IEnumerable<TNode>>( nameof( Nodes ), out var paramNodes ) && !paramNodes.AreEqual( Nodes ) )
        {
            NodeStates = paramNodes?.Select( x => new TreeViewNodeState<TNode>( x, Expanded ) )?.ToList();
        }

        return base.SetParametersAsync( parameters );
    }

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "tree-view" );
        builder.Append( "tree-view-collapsed", !Expanded );

        base.BuildClasses( builder );
    }

    protected async Task OnToggleNode( TreeViewNodeState<TNode> nodeState )
    {
        nodeState.Expanded = !nodeState.Expanded;

        if ( nodeState.Expanded )
        {
            ExpandedNodes.Add( nodeState.Node );
            await ExpandedNodesChanged.InvokeAsync( ExpandedNodes );
        }
        else
        {
            ExpandedNodes.Remove( nodeState.Node );
            await ExpandedNodesChanged.InvokeAsync( ExpandedNodes );
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

    [Parameter] public IEnumerable<TNode> Nodes { get; set; }

    [Parameter] public RenderFragment<TNode> NodeContent { get; set; }

    [Parameter] public IList<TNode> ExpandedNodes { get; set; } = new List<TNode>();

    [Parameter] public EventCallback<IList<TNode>> ExpandedNodesChanged { get; set; }

    [Parameter] public Func<TNode, IEnumerable<TNode>> GetChildNodes { get; set; }

    [Parameter] public Func<TNode, bool> HasChildNodes { get; set; } = node => true;

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