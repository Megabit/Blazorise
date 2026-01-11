#region Using directives
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

    protected Task OnClick()
    {
        //prevent onclick during multi selection mode or if node is disabled
        if ( NodeState.Disabled || SelectionMode == TreeViewSelectionMode.Multiple )
            return Task.CompletedTask;

        DirtyClasses();
        ParentTreeView?.SelectNode( NodeState.Node );

        return Task.CompletedTask;
    }

    protected Task OnCheckedChanged( bool value )
    {
        if ( ParentTreeView is null || NodeState.Disabled )
            return Task.CompletedTask;

        return ParentTreeView.ToggleCheckNode( NodeState.Node );
    }

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

    private string GetCurrentStyle()
    {
        if ( Selected )
            return selectedNodeStyling.Style;
        else if ( NodeState.Disabled )
            return disabledNodeStyling.Style;
        else
            return nodeStyling.Style;
    }

    protected string NodeTitleClassNames
        => nodeTitleClassBuilder.Class;

    protected string NodeTitleStyleNames
        => nodeTitleStyleBuilder.Styles;

    protected string NodeCheckClassNames
        => nodeCheckClassBuilder.Class;

    protected string NodeCheckStyleNames
        => nodeCheckStyleBuilder.Styles;

    protected string NodeContentStyleNames
        => nodeContentStyleBuilder.Styles;

    private void BuildNodeTitleClasses( ClassBuilder builder )
    {
        builder.Append( "b-tree-view-node-title" );

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

    protected override void DirtyClasses()
    {
        nodeTitleClassBuilder?.Dirty();
        nodeCheckClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    protected override void DirtyStyles()
    {
        nodeTitleStyleBuilder?.Dirty();
        nodeCheckStyleBuilder?.Dirty();
        nodeContentStyleBuilder?.Dirty();

        base.DirtyStyles();
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

    [Parameter] public Action<TNode, NodeStyling> DisabledNodeStyling { get; set; }

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