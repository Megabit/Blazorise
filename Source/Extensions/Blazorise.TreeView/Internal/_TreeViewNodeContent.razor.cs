#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView.Internal;

/// <summary>
/// Renders and handles interaction for the visible content of one tree node.
/// </summary>
/// <typeparam name="TNode">Application model represented by the node.</typeparam>
public partial class _TreeViewNodeContent<TNode> : BaseComponent
{
    #region Members

    private TreeViewState<TNode> treeViewState;

    private NodeStyling selectedNodeStyling;

    private NodeStyling disabledNodeStyling;

    private NodeStyling nodeStyling;

    private TreeViewSelectionMode selectionMode;

    private ClassBuilder nodeTitleClassBuilder;
    private StyleBuilder nodeTitleStyleBuilder;
    private ClassBuilder nodeCheckClassBuilder;
    private StyleBuilder nodeCheckStyleBuilder;
    private StyleBuilder nodeContentStyleBuilder;

    #endregion

    #region Constructors

    /// <summary>
    /// Prepares styling builders for the title and selection control.
    /// </summary>
    public _TreeViewNodeContent()
    {
        selectedNodeStyling = new()
        {
            Background = Background.Primary,
            TextColor = TextColor.White
        };

        disabledNodeStyling = new()
        {
            Background = Background.Light,
            TextColor = TextColor.Muted
        };

        nodeStyling = new()
        {
            Background = Background.Default,
            TextColor = TextColor.Default
        };

        nodeTitleClassBuilder = new( BuildNodeTitleClasses );
        nodeTitleStyleBuilder = new( BuildNodeTitleStyles );
        nodeCheckClassBuilder = new( BuildNodeCheckClasses );
        nodeCheckStyleBuilder = new( BuildNodeCheckStyles );
        nodeContentStyleBuilder = new( BuildNodeContentStyles );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( $"{ClassProvider.Spacing( Spacing.Padding, SpacingSize.Is1, Side.All, Breakpoint.None )} cursor-pointer" );

        if ( Selected )
            builder.Append( $"{ClassProvider.BackgroundColor( selectedNodeStyling.Background )} {ClassProvider.TextColor( selectedNodeStyling.TextColor )} {selectedNodeStyling.Class}" );
        else if ( NodeState?.Disabled ?? false )
            builder.Append( $"{ClassProvider.BackgroundColor( disabledNodeStyling.Background )} {ClassProvider.TextColor( disabledNodeStyling.TextColor )} {disabledNodeStyling.Class}" );
        else
            builder.Append( $"{ClassProvider.BackgroundColor( nodeStyling.Background )} {ClassProvider.TextColor( nodeStyling.TextColor )} {nodeStyling.Class}" );

        string nodeContentClass = ParentTreeView?.Classes?.NodeContent;
        if ( !string.IsNullOrWhiteSpace( nodeContentClass ) )
        {
            builder.Append( nodeContentClass );
        }

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Applies the configured selection behavior when the node is activated.
    /// </summary>
    protected Task OnClick()
    {
        //prevent onclick during multi selection mode or if node is disabled
        if ( NodeState.Disabled || SelectionMode == TreeViewSelectionMode.Multiple )
            return Task.CompletedTask;

        DirtyClasses();
        ParentTreeView?.SelectNode( NodeState.Node );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Synchronizes checkbox state with multi-node selection.
    /// </summary>
    protected Task OnCheckedChanged( bool value )
    {
        if ( ParentTreeView is null || NodeState.Disabled )
            return Task.CompletedTask;

        return ParentTreeView.ToggleCheckNode( NodeState.Node );
    }

    /// <inheritdoc />
    protected override Task OnParametersSetAsync()
    {
        if ( Selected )
            SelectedNodeStyling?.Invoke( NodeState.Node, selectedNodeStyling );
        else if ( NodeState.Disabled )
            DisabledNodeStyling?.Invoke( NodeState.Node, disabledNodeStyling );
        else
            NodeStyling?.Invoke( NodeState.Node, nodeStyling );

        DirtyClasses();
        DirtyStyles();

        return base.OnParametersSetAsync();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( ParentTreeView.Draggable && !dragDropInitialized )
        {
            await ParentTreeView.DragDrop.Attach( ElementRef, ElementId );
            dragDropInitialized = true;
        }
        else if ( !ParentTreeView.Draggable && dragDropInitialized )
        {
            await ParentTreeView.DragDrop.Detach( ElementRef, ElementId );
            dragDropInitialized = false;
        }
    }

    private string GetCurrentStyle()
    {
        if ( Selected )
            return selectedNodeStyling.Style;
        else if ( NodeState.Disabled )
            return disabledNodeStyling.Style;
        else
            return nodeStyling.Style;
    }

    /// <summary>
    /// CSS classes decorating the node title region.
    /// </summary>
    protected string NodeTitleClassNames
        => nodeTitleClassBuilder.Class;

    /// <summary>
    /// Inline styles decorating the node title region.
    /// </summary>
    protected string NodeTitleStyleNames
    => nodeTitleStyleBuilder.Styles;

    /// <summary>
    /// CSS classes assigned to the node selection checkbox.
    /// </summary>
    protected string NodeCheckClassNames
        => nodeCheckClassBuilder.Class;

    /// <summary>
    /// Inline styles assigned to the node selection checkbox.
    /// </summary>
    protected string NodeCheckStyleNames
        => nodeCheckStyleBuilder.Styles;

    /// <summary>
    /// Custom styles supplied for the node's content wrapper.
    /// </summary>
    protected string NodeContentStyleNames
        => nodeContentStyleBuilder.Styles;

    private void BuildNodeTitleClasses( ClassBuilder builder )
    {
        builder.Append( "b-tree-view-node-title" );

        switch ( ParentTreeView.DragDrop.GetDropState( NodeState ) )
        {
            case TreeViewDropIndicator.InsertBefore:
                builder.Append( ["b-tree-view-node-drop-target", "b-tree-view-node-title-drop-before"] );
                break;
            case TreeViewDropIndicator.InsertAfter:
                builder.Append( ["b-tree-view-node-drop-target", "b-tree-view-node-title-drop-after"] );
                break;
            case TreeViewDropIndicator.DropAsChild:
                builder.Append( ["b-tree-view-node-drop-target", ClassProvider.BackgroundColor( Background.Primary.Subtle )] );
                break;
            default:
                break;
        }

        string nodeTitleClass = ParentTreeView?.Classes?.NodeTitle;
        if ( !string.IsNullOrWhiteSpace( nodeTitleClass ) )
        {
            builder.Append( nodeTitleClass );
        }
    }

    private void BuildNodeTitleStyles( StyleBuilder builder )
    {
        string nodeTitleStyle = ParentTreeView?.Styles?.NodeTitle;
        if ( !string.IsNullOrWhiteSpace( nodeTitleStyle ) )
        {
            builder.Append( nodeTitleStyle.Trim().TrimEnd( ';' ) );
        }
    }

    private void BuildNodeCheckClasses( ClassBuilder builder )
    {
        builder.Append( "b-tree-view-node-check" );

        string nodeCheckClass = ParentTreeView?.Classes?.NodeCheck;
        if ( !string.IsNullOrWhiteSpace( nodeCheckClass ) )
        {
            builder.Append( nodeCheckClass );
        }
    }

    private void BuildNodeCheckStyles( StyleBuilder builder )
    {
        string nodeCheckStyle = ParentTreeView?.Styles?.NodeCheck;
        if ( !string.IsNullOrWhiteSpace( nodeCheckStyle ) )
        {
            builder.Append( nodeCheckStyle.Trim().TrimEnd( ';' ) );
        }
    }

    private void BuildNodeContentStyles( StyleBuilder builder )
    {
        string nodeContentStyle = ParentTreeView?.Styles?.NodeContent;
        if ( !string.IsNullOrWhiteSpace( nodeContentStyle ) )
        {
            builder.Append( nodeContentStyle.Trim().TrimEnd( ';' ) );
        }

        string stateStyle = GetCurrentStyle();
        if ( !string.IsNullOrWhiteSpace( stateStyle ) )
        {
            builder.Append( stateStyle.Trim().TrimEnd( ';' ) );
        }
    }

    /// <inheritdoc />
    protected override void DirtyClasses()
    {
        nodeTitleClassBuilder?.Dirty();
        nodeCheckClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc />
    protected override void DirtyStyles()
    {
        nodeTitleStyleBuilder?.Dirty();
        nodeCheckStyleBuilder?.Dirty();
        nodeContentStyleBuilder?.Dirty();

        base.DirtyStyles();
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && dragDropInitialized )
        {
            await ParentTreeView.DragDrop.Detach( ElementRef, ElementId );
            dragDropInitialized = false;
        }

        await base.DisposeAsync( disposing );
    }

    #endregion

    #region Properties

    private bool dragDropInitialized;

    /// <summary>
    /// Whether this node is the current single-selection item.
    /// </summary>
    protected bool Selected
        => SelectionMode == TreeViewSelectionMode.Single && ParentTreeViewState.SelectedNode != null && ParentTreeViewState.SelectedNode.Equals( NodeState.Node );

    /// <summary>
    /// Whether this node belongs to the multi-selection set.
    /// </summary>
    protected bool Checked
        => SelectionMode == TreeViewSelectionMode.Multiple && ParentTreeViewState.SelectedNodes != null && ParentTreeViewState.SelectedNodes.Contains( NodeState.Node );

    /// <summary>
    /// Runtime expansion, loading, and model state for this node.
    /// </summary>
    [Parameter] public TreeViewNodeState<TNode> NodeState { get; set; }

    /// <summary>
    /// Shared selection and drag state maintained by the root tree.
    /// </summary>
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

    /// <summary>
    /// Root tree that owns this node content.
    /// </summary>
    [CascadingParameter] public TreeView<TNode> ParentTreeView { get; set; }

    /// <summary>
    /// Effective selection mode inherited from the root tree.
    /// </summary>
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
    /// Defines styling applied to selected nodes.
    /// </summary>
    [Parameter] public Action<TNode, NodeStyling> SelectedNodeStyling { get; set; }

    /// <summary>
    /// Styling callback applied when the node is disabled.
    /// </summary>
    [Parameter] public Action<TNode, NodeStyling> DisabledNodeStyling { get; set; }

    /// <summary>
    /// Defines styling applied to nodes.
    /// </summary>
    [Parameter] public Action<TNode, NodeStyling> NodeStyling { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="_TreeViewNodeContent{TNode}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}