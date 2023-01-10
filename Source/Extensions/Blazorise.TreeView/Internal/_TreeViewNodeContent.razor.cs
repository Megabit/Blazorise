﻿#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView.Internal;

public partial class _TreeViewNodeContent<TNode> : BaseComponent
{
    #region Members

    private TreeViewState<TNode> treeViewState;

    private NodeStyling selectedNodeStyling;

    private NodeStyling nodeStyling;

    private TreeViewSelectionMode selectionMode;

    #endregion

    #region Constructors

    public _TreeViewNodeContent()
    {
        selectedNodeStyling = new()
        {
            Background = Background.Primary,
            TextColor = TextColor.White
        };

        nodeStyling = new()
        {
            Background = Background.Default,
            TextColor = TextColor.Default
        };
    }

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( $"{ClassProvider.Spacing( Spacing.Padding, SpacingSize.Is1, Side.All, Breakpoint.None )} cursor-pointer" );

        if ( Selected )
            builder.Append( $"{ClassProvider.BackgroundColor( selectedNodeStyling.Background )} {ClassProvider.TextColor( selectedNodeStyling.TextColor )} {selectedNodeStyling.Class}" );
        else
            builder.Append( $"{ClassProvider.BackgroundColor( nodeStyling.Background )} {ClassProvider.TextColor( nodeStyling.TextColor )} {nodeStyling.Class}" );

        base.BuildClasses( builder );
    }

    protected Task OnClick()
    {
        if ( SelectionMode != TreeViewSelectionMode.Single )
            return Task.CompletedTask;

        DirtyClasses();
        ParentTreeView?.SelectNode( NodeState.Node );

        return Task.CompletedTask;
    }

    protected Task OnCheckedChanged( bool value )
    {
        if ( ParentTreeView is not null )
            return ParentTreeView.ToggleCheckNode( NodeState.Node );

        return Task.CompletedTask;
    }

    protected override Task OnParametersSetAsync()
    {
        if ( Selected )
            SelectedNodeStyling?.Invoke( NodeState.Node, selectedNodeStyling );
        else
            NodeStyling?.Invoke( NodeState.Node, nodeStyling );

        return base.OnParametersSetAsync();
    }

    #endregion

    #region Properties

    protected bool Selected
        => SelectionMode == TreeViewSelectionMode.Single && ParentTreeViewState.SelectedNode != null && ParentTreeViewState.SelectedNode.Equals( NodeState.Node );

    protected bool Checked
        => SelectionMode == TreeViewSelectionMode.Multiple && ParentTreeViewState.SelectedNodes != null && ParentTreeViewState.SelectedNodes.Contains( NodeState.Node );

    [Parameter] public TreeViewNodeState<TNode> NodeState { get; set; }

    [CascadingParameter]
    protected TreeViewState<TNode> ParentTreeViewState
    {
        get => treeViewState;
        set
        {
            if ( treeViewState == value )
                return;

            treeViewState = value;

            DirtyClasses();
        }
    }

    [CascadingParameter] public TreeView<TNode> ParentTreeView { get; set; }

    [Parameter]
    public TreeViewSelectionMode SelectionMode
    {
        get => selectionMode;
        set
        {
            if ( selectionMode == value )
                return;

            selectionMode = value;

            DirtyClasses();
            DirtyStyles();
        }
    }

    /// <summary>
    /// Gets or sets selected node styling.
    /// </summary>
    [Parameter] public Action<TNode, NodeStyling> SelectedNodeStyling { get; set; }

    /// <summary>
    /// Gets or sets node styling.
    /// </summary>
    [Parameter] public Action<TNode, NodeStyling> NodeStyling { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="_TreeViewNodeContent{TNode}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}