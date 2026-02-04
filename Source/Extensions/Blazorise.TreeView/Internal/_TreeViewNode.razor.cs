#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.TreeView;
using Blazorise.TreeView.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.TreeView.Internal;

public partial class _TreeViewNode<TNode> : BaseComponent, IDisposable
{
    #region Members

    private bool checkChildrenLoaded;

    internal NotifyCollectionChangedEventHandler PreviousNotifyCollectionChangedEventHandler;

    private int? expandedNodesHash;

    private ClassBuilder nodeClassBuilder;
    private StyleBuilder nodeStyleBuilder;
    private ClassBuilder nodeIconClassBuilder;
    private StyleBuilder nodeIconStyleBuilder;
    private TreeViewNodeContext<TNode> nodeContext;

    #endregion

    #region Constructors

    public _TreeViewNode()
    {
        nodeClassBuilder = new( BuildNodeClasses );
        nodeStyleBuilder = new( BuildNodeStyles );
        nodeIconClassBuilder = new( BuildNodeIconClasses );
        nodeIconStyleBuilder = new( BuildNodeIconStyles );
    }

    #endregion

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        if ( AutoExpandAll )
        {
            await AutoExpandNodes();
        }
        else if ( ExpandedNodes?.Count > 0 )
        {
            foreach ( var nodeState in NodeStates ?? Enumerable.Empty<TreeViewNodeState<TNode>>() )
            {
                if ( nodeState.HasChildren && ExpandedNodes.Contains( nodeState.Node ) == true )
                {
                    await LoadChildNodes( nodeState );
                }
            }
        }

        await base.OnInitializedAsync();
    }

    public override async Task SetParametersAsync( ParameterView parameters )
    {
        checkChildrenLoaded = true;

        await base.SetParametersAsync( parameters );
    }

    protected override async Task OnParametersSetAsync()
    {
        var expandedNodesHashChanged = SyncExpandedNodesHash();

        if ( expandedNodesHashChanged )
        {
            await SynchronizeExpandedNodes();
        }

        // Check if expanded is true but children is empty to load child nodes if needed, happens after Reload,
        // we use the bool flag to ensure we don't do this multiple times during render.
        if ( checkChildrenLoaded )
        {
            var unloadedNodeStates = NodeStates != null
                ? NodeStates.Where( o => o.Expanded && o.Children.Count == 0 )
                : Enumerable.Empty<TreeViewNodeState<TNode>>();

            foreach ( TreeViewNodeState<TNode> unloadedNodeState in unloadedNodeStates )
            {
                await LoadChildNodes( unloadedNodeState );
            }

            checkChildrenLoaded = false;
        }

        DirtyClasses();
        DirtyStyles();
    }

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-tree-view-node" );
        builder.Append( "b-tree-view-node-collapsed", !Expanded );

        base.BuildClasses( builder );
    }

    protected override void BuildCustomClasses( ClassBuilder builder )
    {
        string nodesClass = ParentTreeView?.Classes?.Nodes;
        if ( !string.IsNullOrWhiteSpace( nodesClass ) )
        {
            builder.Append( nodesClass );
        }
    }

    protected override void BuildCustomStyles( StyleBuilder builder )
    {
        string nodesStyle = ParentTreeView?.Styles?.Nodes;
        if ( !string.IsNullOrWhiteSpace( nodesStyle ) )
        {
            builder.Append( nodesStyle.Trim().TrimEnd( ';' ) );
        }
    }

    protected override void DirtyClasses()
    {
        nodeClassBuilder?.Dirty();
        nodeIconClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    protected override void DirtyStyles()
    {
        nodeStyleBuilder?.Dirty();
        nodeIconStyleBuilder?.Dirty();

        base.DirtyStyles();
    }

    private async Task AutoExpandNodes()
    {
        foreach ( var nodeState in NodeStates ?? Enumerable.Empty<TreeViewNodeState<TNode>>() )
        {
            if ( nodeState.HasChildren && !nodeState.Expanded && !nodeState.AutoExpanded )
            {
                nodeState.AutoExpanded = true;

                await ToggleNode( nodeState );
            }
        }
    }

    private async Task ToggleNode( TreeViewNodeState<TNode> nodeState, bool refresh = true )
    {
        nodeState.Expanded = !nodeState.Expanded;

        if ( nodeState.Expanded )
        {
            if ( !ExpandedNodes.Contains( nodeState.Node ) )
            {
                ExpandedNodes.Add( nodeState.Node );
                await ExpandedNodesChanged.InvokeAsync( ExpandedNodes );
            }

            if ( nodeState.HasChildren )
            {
                await LoadChildNodes( nodeState );
            }
        }
        else
        {
            if ( ExpandedNodes.Remove( nodeState.Node ) )
            {
                await ExpandedNodesChanged.InvokeAsync( ExpandedNodes );
            }
        }

        if ( refresh )
        {
            DirtyClasses();

            await InvokeAsync( StateHasChanged );
        }
    }

    private async Task LoadChildNodes( TreeViewNodeState<TNode> nodeState )
    {
        var childNodes = GetChildNodesAsync is not null
            ? await GetChildNodesAsync( nodeState.Node )
            : GetChildNodes is not null
                ? GetChildNodes( nodeState.Node )
                : null;

        NotifyCollectionChangedEventHandler childrenChangedHandler = ( sender, e ) =>
        {
            OnChildrenChanged( sender, e, nodeState, childNodes );
        };

        if ( childNodes is INotifyCollectionChanged observableCollection )
        {
            if ( PreviousNotifyCollectionChangedEventHandler is not null )
            {
                observableCollection.CollectionChanged -= PreviousNotifyCollectionChangedEventHandler;
            }

            observableCollection.CollectionChanged += childrenChangedHandler;
            PreviousNotifyCollectionChangedEventHandler = childrenChangedHandler;
        }

        if ( !nodeState.Children.Select( x => x.Node ).AreEqual( childNodes ) )
        {
            await ReloadChildren( nodeState, childNodes );
        }

    }

    private async Task ReloadChildren( TreeViewNodeState<TNode> nodeState, IEnumerable<TNode> childNodes )
    {
        nodeState.Children.Clear();

        await foreach ( var childNodeState in childNodes.ToNodeStates( HasChildNodesAsync, DetermineHasChildNodes, ( node ) => ExpandedNodes?.Contains( node ) == true, DetermineIsDisabled ) )
        {
            nodeState.Children.Add( childNodeState );
        }
    }

    private async void OnChildrenChanged( object sender, NotifyCollectionChangedEventArgs e, TreeViewNodeState<TNode> nodeState, IEnumerable<TNode> childNodes )
    {
        if ( e.Action == NotifyCollectionChangedAction.Add )
        {
            await foreach ( var childNodeState in e.NewItems.ToNodeStates( HasChildNodesAsync, DetermineHasChildNodes, ( node ) => ExpandedNodes?.Contains( node ) == true, DetermineIsDisabled ) )
            {
                if ( !nodeState.Children.Exists( x => x.Node.IsEqual( childNodeState.Node ) ) )
                    nodeState.Children.Add( childNodeState );
            }
        }
        else if ( e.Action == NotifyCollectionChangedAction.Remove )
        {
            nodeState.Children.RemoveAll( x => e.OldItems.Contains( x.Node ) );
        }
        else
        {
            if ( !nodeState.Children.Select( x => x.Node ).AreEqual( childNodes ) )
            {
                await ReloadChildren( nodeState, childNodes );
            }
        }

        StateHasChanged();
    }

    private bool SyncExpandedNodesHash()
    {
        var newExpandedNodesHash = ExpandedNodes?.GetListHash();

        if ( expandedNodesHash == newExpandedNodesHash )
            return false;

        expandedNodesHash = newExpandedNodesHash;

        return true;
    }

    private async Task SynchronizeExpandedNodes()
    {
        if ( NodeStates is null )
            return;

        await SynchronizeExpandedNodes( NodeStates );
    }

    private async Task SynchronizeExpandedNodes( IEnumerable<TreeViewNodeState<TNode>> nodeStates )
    {
        foreach ( var nodeState in nodeStates )
        {
            var shouldBeExpanded = ExpandedNodes?.Contains( nodeState.Node ) == true;

            if ( nodeState.Expanded != shouldBeExpanded )
            {
                nodeState.Expanded = shouldBeExpanded;

                if ( shouldBeExpanded && nodeState.HasChildren )
                {
                    await LoadChildNodes( nodeState );
                }
            }

            if ( nodeState.Children?.Count > 0 )
            {
                await SynchronizeExpandedNodes( nodeState.Children );
            }
        }
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( ParentNode?.PreviousNotifyCollectionChangedEventHandler is not null )
            {
                if ( NodeStates is INotifyCollectionChanged observableCollection )
                {
                    observableCollection.CollectionChanged -= ParentNode.PreviousNotifyCollectionChangedEventHandler;
                }
            }
        }

        base.Dispose( disposing );
    }

    public async Task ExpandAll()
    {
        foreach ( var nodeState in NodeStates ?? Enumerable.Empty<TreeViewNodeState<TNode>>() )
        {
            if ( DetermineHasChildNodes( nodeState.Node ) )
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
            if ( DetermineHasChildNodes( nodeState.Node ) )
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

    /// <summary>
    /// Event handler for <see cref="ContextMenu"/> event callback.
    /// </summary>
    /// <param name="nodeState">The node state that is being clicked.</param>
    /// <param name="eventArgs">Supplies information about an contextmenu event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task OnContextMenuHandler( TreeViewNodeState<TNode> nodeState, MouseEventArgs eventArgs )
    {
        return ContextMenu.InvokeAsync( new TreeViewNodeMouseEventArgs<TNode>( nodeState.Node, eventArgs ) );
    }

    protected string NodeClassNames( TreeViewNodeState<TNode> nodeState )
    {
        NodeContext = BuildNodeContext( nodeState );
        return nodeClassBuilder.Class;
    }

    protected string NodeStyleNames( TreeViewNodeState<TNode> nodeState )
    {
        NodeContext = BuildNodeContext( nodeState );
        return nodeStyleBuilder.Styles;
    }

    protected string NodeIconClassNames
        => nodeIconClassBuilder.Class;

    protected string NodeIconStyleNames
        => nodeIconStyleBuilder.Styles;

    private void BuildNodeClasses( ClassBuilder builder )
    {
        string nodeClass = ParentTreeView?.Classes?.Node?.Invoke( nodeContext );
        builder.Append( nodeClass );
    }

    private void BuildNodeStyles( StyleBuilder builder )
    {
        string nodeStyle = ParentTreeView?.Styles?.Node?.Invoke( nodeContext );
        if ( !string.IsNullOrWhiteSpace( nodeStyle ) )
            builder.Append( nodeStyle.Trim().TrimEnd( ';' ) );
    }

    private TreeViewNodeContext<TNode> BuildNodeContext( TreeViewNodeState<TNode> nodeState )
    {
        if ( nodeState is null )
            return null;

        bool isSelected = SelectionMode == TreeViewSelectionMode.Single
            && ParentTreeViewState.SelectedNode is not null
            && ParentTreeViewState.SelectedNode.Equals( nodeState.Node );

        bool isChecked = SelectionMode == TreeViewSelectionMode.Multiple
            && ParentTreeViewState.SelectedNodes is not null
            && ParentTreeViewState.SelectedNodes.Contains( nodeState.Node );

        return new TreeViewNodeContext<TNode>(
            nodeState.Node,
            nodeState.HasChildren,
            nodeState.Expanded,
            nodeState.Disabled,
            nodeState.AutoExpanded,
            isSelected,
            isChecked,
            SelectionMode );
    }

    private TreeViewNodeContext<TNode> NodeContext
    {
        get => nodeContext;
        set
        {
            if ( nodeContext.IsEqual( value ) )
                return;

            nodeContext = value;
            nodeClassBuilder.Dirty();
            nodeStyleBuilder.Dirty();
        }
    }

    private void BuildNodeIconClasses( ClassBuilder builder )
    {
        builder.Append( "b-tree-view-node-icon" );

        string nodeIconClass = ParentTreeView?.Classes?.NodeIcon;
        if ( !string.IsNullOrWhiteSpace( nodeIconClass ) )
        {
            builder.Append( nodeIconClass );
        }
    }

    private void BuildNodeIconStyles( StyleBuilder builder )
    {
        string nodeIconStyle = ParentTreeView?.Styles?.NodeIcon;
        if ( !string.IsNullOrWhiteSpace( nodeIconStyle ) )
        {
            builder.Append( nodeIconStyle.Trim().TrimEnd( ';' ) );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the node has child elements.
    /// </summary>
    protected Func<TNode, bool> DetermineHasChildNodes => HasChildNodes ?? ( node => false );

    /// <summary>
    /// Indicates the node's disabled state. Used for preventing selection.
    /// </summary>
    protected Func<TNode, bool> DetermineIsDisabled => IsDisabled ?? ( node => false );

    [Parameter] public IEnumerable<TreeViewNodeState<TNode>> NodeStates { get; set; }

    [Parameter] public RenderFragment<TNode> NodeContent { get; set; }

    [Parameter] public IList<TNode> ExpandedNodes { get; set; } = new List<TNode>();

    [Parameter] public EventCallback<IList<TNode>> ExpandedNodesChanged { get; set; }

    [Parameter] public Func<TNode, IEnumerable<TNode>> GetChildNodes { get; set; }

    [Parameter] public Func<TNode, bool> HasChildNodes { get; set; }

    [Parameter] public Func<TNode, bool> IsDisabled { get; set; }

    [Parameter] public Func<TNode, Task<IEnumerable<TNode>>> GetChildNodesAsync { get; set; }

    [Parameter] public Func<TNode, Task<bool>> HasChildNodesAsync { get; set; }

    [Parameter] public TreeViewSelectionMode SelectionMode { get; set; }

    /// <summary>
    /// Defines if the treenode should be expanded.
    /// </summary>
    [Parameter] public bool Expanded { get; set; }

    /// <summary>
    /// Defines if the treenode should be automatically expanded. Note that it can happen only once when the tree is first loaded.
    /// </summary>
    [Parameter] public bool AutoExpandAll { get; set; }

    /// <summary>
    /// Controls if the child nodes, which are currently not expanded, are visible.<para></para>
    /// This is useful for optimizing large TreeViews. See <see href="https://learn.microsoft.com/en-us/aspnet/core/blazor/components/virtualization">Docs for virtualization</see> for more info.
    /// </summary>
    [Parameter] public bool Virtualize { get; set; }

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

    /// <summary>
    /// Gets or sets selected node styling.
    /// </summary>
    [Parameter] public Action<TNode, NodeStyling> SelectedNodeStyling { get; set; }

    /// <summary>
    /// Gets or sets disabled node styling.
    /// </summary>
    [Parameter] public Action<TNode, NodeStyling> DisabledNodeStyling { get; set; }

    /// <summary>
    /// Gets or sets node styling.
    /// </summary>
    [Parameter] public Action<TNode, NodeStyling> NodeStyling { get; set; }

    /// <summary>
    /// The event is fired when an element or text selection is right clicked to show the context menu.
    /// </summary>
    [Parameter] public EventCallback<TreeViewNodeMouseEventArgs<TNode>> ContextMenu { get; set; }

    /// <summary>
    /// Used to prevent the default action for an <see cref="ContextMenu"/> event.
    /// </summary>
    [Parameter] public bool ContextMenuPreventDefault { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="_TreeViewNode{TNode}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// A reference to the parent node component.
    /// </summary>
    [CascadingParameter] public _TreeViewNode<TNode> ParentNode { get; set; }

    [CascadingParameter] public TreeView<TNode> ParentTreeView { get; set; }

    [CascadingParameter] protected TreeViewState<TNode> ParentTreeViewState { get; set; }

    #endregion
}