#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView;

public partial class _TreeViewNodeContent<TNode> : BaseComponent
{
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

    #region Members

    private TreeViewState<TNode> treeViewState;

    private NodeStyling selectedNodeStyling;

    private NodeStyling nodeStyling;

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
        DirtyClasses();
        ParentTreeView?.SelectNode( NodeState.Node );

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
        => ParentTreeViewState.SelectedNode != null && ParentTreeViewState.SelectedNode.Equals( NodeState.Node );

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