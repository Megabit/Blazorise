#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Licensing;
using Blazorise.Modules;
using Blazorise.TreeView.EventArguments;
using Blazorise.TreeView.Extensions;
using Blazorise.TreeView.Internal;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView;

public partial class TreeView<TNode> : BaseComponent<TreeViewClasses<TNode>, TreeViewStyles<TNode>>, IDisposable
{
    #region Members

    private _TreeViewNode<TNode> treeViewNodeRef;

    private TreeViewState<TNode> treeViewState = new()
    {
        SelectedNodes = new List<TNode>(),
        SelectionMode = TreeViewSelectionMode.Single,
    };

    private List<TreeViewNodeState<TNode>> treeViewNodeStates;

    #endregion

    #region Methods

    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool nodesChanged = parameters.TryGetValue<IEnumerable<TNode>>( nameof( Nodes ), out var paramNodes ) && !paramNodes.AreEqual( Nodes );
        bool selectedNodeChanged = parameters.TryGetValue<TNode>( nameof( SelectedNode ), out var paramSelectedNode ) && !paramSelectedNode.IsEqual( treeViewState.SelectedNode );
        bool selectedNodesChanged = parameters.TryGetValue<IList<TNode>>( nameof( SelectedNodes ), out var paramSelectedNodes ) && !paramSelectedNodes.AreEqual( treeViewState.SelectedNodes );
        bool virtualizeDefined = parameters.TryGetValue<bool>( nameof( Virtualize ), out var paramVirtualize );
        bool virtualizeChanged = virtualizeDefined && !paramVirtualize.IsEqual( Virtualize );

        bool heightDefined = parameters.TryGetValue<IFluentSizing>( nameof( Height ), out var paramHeight );
        bool overflowDefined = parameters.TryGetValue<IFluentOverflow>( nameof( Overflow ), out var paramOverflow );

        if ( selectedNodeChanged )
        {
            treeViewState = treeViewState with { SelectedNode = paramSelectedNode };
        }

        if ( selectedNodesChanged )
        {
            treeViewState = treeViewState with { SelectedNodes = paramSelectedNodes };
        }

        // We don't want to have memory leak so we need to unsubscribe any previous event if it exist.
        // The unsubscribe must happen before SetParametersAsync.
        if ( Nodes is INotifyCollectionChanged observableCollectionBeforeChange )
        {
            observableCollectionBeforeChange.CollectionChanged -= OnCollectionChanged;
        }

        await base.SetParametersAsync( parameters );

        if ( Rendered && virtualizeChanged )
        {
            if ( !heightDefined && paramVirtualize )
                Height = Blazorise.Height.Px( 300 );
            else
                Height = paramHeight;

            if ( !overflowDefined && paramVirtualize )
                Overflow = Blazorise.Overflow.Auto;
            else
                Overflow = paramOverflow;

            DirtyClasses();
            DirtyStyles();
        }

        if ( nodesChanged )
        {
            await Reload();
        }

        // Now we can safely subscribe to the changes.
        if ( Nodes is INotifyCollectionChanged observableCollectionAfterChange )
        {
            observableCollectionAfterChange.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    /// Handles root node collection changes.
    /// </summary>
    /// <param name="sender">The observable collection that raised the event.</param>
    /// <param name="e">Supplies information about the collection change.</param>
    private void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
    {
        if ( e.Action == NotifyCollectionChangedAction.Add )
        {
            InvokeAsync( async () =>
            {
                int insertIndex = e.NewStartingIndex >= 0
                    ? e.NewStartingIndex
                    : treeViewNodeStates.Count;

                await foreach ( var nodeState in e.NewItems.ToNodeStates( null, HasChildNodesAsync, DetermineHasChildNodes, ( node ) => ExpandedNodes?.Contains( node ) == true, DetermineIsDisabled ) )
                {
                    InsertTreeViewNodeState( nodeState, insertIndex++ );
                    treeViewNodeRef?.RegisterNodeState( nodeState );
                }

                await InvokeAsync( StateHasChanged );
            } );
        }

        if ( e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Move )
        {
            InvokeAsync( Reload );
        }
    }

    /// <summary>
    /// Attempts to find and remove an existing node from the Treeview.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public Task RemoveNode( TNode node )
    {
        SearchTryRemoveNode( treeViewNodeStates, node );
        return Task.CompletedTask;
    }

    /// <summary>
    /// Recursively searches for a given node to remove it from the Treeview.
    /// </summary>
    /// <param name="nodeStates"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    private void SearchTryRemoveNode( List<TreeViewNodeState<TNode>> nodeStates, TNode node )
    {
        if ( nodeStates.IsNullOrEmpty() )
            return;

        var nodeToRemove = nodeStates.FirstOrDefault( x => x.Node.Equals( node ) );
        if ( nodeToRemove is not null )
        {
            nodeStates.Remove( nodeToRemove );
        }
        else
        {
            foreach ( var nodeState in nodeStates )
            {
                SearchTryRemoveNode( nodeState.Children, node );
            }
        }
    }

    /// <summary>
    /// Triggers the reload of the <see cref="TreeView{TNode}.Nodes"/>.
    /// </summary>
    /// <returns>Returns the awaitable task.</returns>
    public async Task Reload()
    {
        treeViewNodeStates = new();

        foreach ( TreeViewNodeState<TNode> nodeState in await CreateNodeStatesAsync( Nodes ) )
        {
            AddTreeViewNodeState( nodeState );
        }

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Triggers the reload of a specific node and its subtree.
    /// </summary>
    /// <param name="node">Node to reload.</param>
    /// <returns>Returns the awaitable task.</returns>
    public async Task ReloadNode( TNode node )
    {
        if ( treeViewNodeStates.IsNullOrEmpty() )
            return;

        if ( !TryFindNodeState( treeViewNodeStates, treeViewNodeRef, node, out TreeViewNodeState<TNode> nodeState, out _TreeViewNode<TNode> ownerView ) )
            return;

        TreeViewNodeState<TNode> updatedNodeState = await CreateNodeStateAsync( node );

        ownerView?.UnregisterNodeState( nodeState );

        nodeState.Node = updatedNodeState.Node;
        nodeState.HasChildren = updatedNodeState.HasChildren;
        nodeState.Disabled = updatedNodeState.Disabled;
        nodeState.Expanded = updatedNodeState.HasChildren && updatedNodeState.Expanded;

        if ( !nodeState.HasChildren && ExpandedNodes.Remove( nodeState.Node ) )
        {
            await ExpandedNodesChanged.InvokeAsync( ExpandedNodes );
        }

        nodeState.Children.Clear();
        nodeState.ViewRef?.ReloadNodeStatesSubscriptions( nodeState.Children );

        ownerView?.RegisterNodeState( nodeState );

        DirtyClasses();
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Adds a root node state to the end of the root node state collection.
    /// </summary>
    /// <param name="treeViewNodeState">The node state to add.</param>
    private void AddTreeViewNodeState( TreeViewNodeState<TNode> treeViewNodeState )
        => InsertTreeViewNodeState( treeViewNodeState, treeViewNodeStates.Count );

    /// <summary>
    /// Inserts a root node state at the requested index while respecting license row limits.
    /// </summary>
    /// <param name="treeViewNodeState">The node state to insert.</param>
    /// <param name="index">The requested insertion index.</param>
    private void InsertTreeViewNodeState( TreeViewNodeState<TNode> treeViewNodeState, int index )
    {
        var maxRowsLimit = BlazoriseLicenseLimitsHelper.GetTreeViewRowsLimit( LicenseChecker );

        if ( maxRowsLimit.HasValue )
        {
            if ( treeViewNodeStates?.Count >= maxRowsLimit.Value )
            {
                return;
            }
        }

        treeViewNodeStates.Insert( Math.Clamp( index, 0, treeViewNodeStates.Count ), treeViewNodeState );
    }

    internal async Task<TreeViewNodeState<TNode>> CreateNodeStateAsync( TNode node, TreeViewNodeState<TNode> parent = null )
    {
        bool hasChildren = HasChildNodesAsync is not null
            ? await HasChildNodesAsync( node )
            : DetermineHasChildNodes( node );

        bool expanded = ExpandedNodes?.Contains( node ) == true;
        bool disabled = DetermineIsDisabled( node );

        return new TreeViewNodeState<TNode>( node, hasChildren, expanded, disabled, parent );
    }

    internal async Task<List<TreeViewNodeState<TNode>>> CreateNodeStatesAsync( IEnumerable<TNode> nodes, TreeViewNodeState<TNode> parent = null )
    {
        List<TreeViewNodeState<TNode>> nodeStates = new();

        foreach ( TNode node in nodes ?? Enumerable.Empty<TNode>() )
        {
            nodeStates.Add( await CreateNodeStateAsync( node, parent ) );
        }

        return nodeStates;
    }

    private bool TryFindNodeState( IList<TreeViewNodeState<TNode>> nodeStates, _TreeViewNode<TNode> ownerView, TNode node, out TreeViewNodeState<TNode> foundNodeState, out _TreeViewNode<TNode> foundOwnerView )
    {
        if ( nodeStates is not null )
        {
            foreach ( TreeViewNodeState<TNode> nodeState in nodeStates )
            {
                if ( nodeState.Node.IsEqual( node ) )
                {
                    foundNodeState = nodeState;
                    foundOwnerView = ownerView;
                    return true;
                }

                if ( TryFindNodeState( nodeState.Children, nodeState.ViewRef, node, out foundNodeState, out foundOwnerView ) )
                    return true;
            }
        }

        foundNodeState = null;
        foundOwnerView = null;
        return false;
    }

    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( Nodes is INotifyCollectionChanged observableCollection )
            {
                observableCollection.CollectionChanged -= OnCollectionChanged;
            }
        }

        base.Dispose( disposing );
    }

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-tree-view" );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Selects the node when in single selection mode.
    /// </summary>
    /// <param name="node">Node to select.</param>
    public void SelectNode( TNode node )
    {
        if ( treeViewState.SelectionMode == TreeViewSelectionMode.Multiple )
            return;

        if ( treeViewState.SelectedNode.IsEqual( node ) )
            return;

        treeViewState = treeViewState with { SelectedNode = node };

        SelectedNodeChanged.InvokeAsync( treeViewState.SelectedNode );

        DirtyClasses();
        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Toggles the checked state of the node when in multiple selection mode.
    /// </summary>
    /// <param name="node">Node to toggle.</param>
    public async Task ToggleCheckNode( TNode node )
    {
        if ( treeViewState.SelectionMode == TreeViewSelectionMode.Single )
            return;

        if ( treeViewState.SelectedNodes.Contains( node ) )
            treeViewState.SelectedNodes.Remove( node );
        else
            treeViewState.SelectedNodes.Add( node );

        await SelectedNodesChanged.InvokeAsync( treeViewState.SelectedNodes );

        DirtyClasses();
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Expands all the collapsed TreeView nodes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ExpandAll()
    {
        if ( treeViewNodeRef is not null )
            return treeViewNodeRef.ExpandAll();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Collapses all the expanded TreeView nodes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task CollapseAll()
    {
        if ( treeViewNodeRef is not null )
            return treeViewNodeRef.CollapseAll();

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Behavior class for handling drag and drop events
    /// </summary>
    internal TreeViewDragDropBehavior<TNode> DragDrop { get => field ??= new( this, () => InvokeAsync( StateHasChanged ) ); }

    /// <summary>
    /// Indicates if the node has child elements.
    /// </summary>
    protected Func<TNode, bool> DetermineHasChildNodes => HasChildNodes ?? ( node => false );

    /// <summary>
    /// Indicates the node's disabled state. Used for preventing selection.
    /// </summary>
    protected Func<TNode, bool> DetermineIsDisabled => IsDisabled ?? ( node => false );

    /// <summary>
    /// Gets the root node states rendered by the TreeView.
    /// </summary>
    internal IList<TreeViewNodeState<TNode>> RootNodeStates => treeViewNodeStates;

    /// <summary>
    /// Gets or sets the license checker for the user session.
    /// </summary>
    [Inject] internal BlazoriseLicenseChecker LicenseChecker { get; set; }

    /// <summary>
    /// Gets or sets the JS drag and drop module.
    /// </summary>
    [Inject] internal IJSDragDropModule JSDragDropModule { get; set; }

    /// <summary>
    /// Defines the name of the treenode expand icon.
    /// </summary>
    [Parameter] public IconName ExpandIconName { get; set; } = IconName.ChevronRight;

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
    [Parameter] public IconName CollapseIconName { get; set; } = IconName.ChevronDown;

    /// <summary>
    /// Defines the style of the treenode collapse icon.
    /// </summary>
    [Parameter] public IconStyle? CollapseIconStyle { get; set; }

    /// <summary>
    /// Defines the size of the treenode collapse icon.
    /// </summary>
    [Parameter] public IconSize? CollapseIconSize { get; set; }

    /// <summary>
    /// Collection of child TreeView items (child nodes)
    /// </summary>
    [Parameter] public IEnumerable<TNode> Nodes { get; set; }

    /// <summary>
    /// Template to display content for the node
    /// </summary>
    [Parameter] public RenderFragment<TNode> NodeContent { get; set; }

    /// <summary>
    /// Currently selected TreeView item/node.
    /// </summary>
    [Parameter] public TNode SelectedNode { get; set; }

    /// <summary>
    /// Occurs when the selected TreeView node has changed.
    /// </summary>
    [Parameter] public EventCallback<TNode> SelectedNodeChanged { get; set; }

    /// <summary>
    /// Currently selected TreeView items/nodes.
    /// </summary>
    [Parameter] public IList<TNode> SelectedNodes { get; set; }

    /// <summary>
    /// Occurs when the selected TreeView nodes has changed.
    /// </summary>
    [Parameter] public EventCallback<IList<TNode>> SelectedNodesChanged { get; set; }

    /// <summary>
    /// Defines the selection mode of the <see cref="TreeView{TNode}"/>.
    /// </summary>
    [Parameter]
    public TreeViewSelectionMode SelectionMode
    {
        get => treeViewState.SelectionMode;
        set
        {
            if ( treeViewState.SelectionMode == value )
                return;

            treeViewState.SelectionMode = value;
        }
    }

    /// <summary>
    /// Defines if the treenode should be automatically expanded. Note that it can happen only once when the tree is first loaded.
    /// </summary>
    [Parameter] public bool AutoExpandAll { get; set; }

    /// <summary>
    /// Controls if the child nodes, which are currently not expanded, are visible.
    /// This is useful for optimizing large TreeViews. See <see href="https://learn.microsoft.com/en-us/aspnet/core/blazor/components/virtualization">Docs for virtualization</see> for more info.
    /// </summary>
    [Parameter] public bool Virtualize { get; set; }

    /// <summary>
    /// List of currently expanded TreeView items (child nodes).
    /// </summary>
    [Parameter] public IList<TNode> ExpandedNodes { get; set; } = new List<TNode>();

    /// <summary>
    /// Occurs when the collection of expanded nodes has changed.
    /// </summary>
    [Parameter] public EventCallback<IList<TNode>> ExpandedNodesChanged { get; set; }

    /// <summary>
    /// Gets the list of child nodes for each node.
    /// </summary>
    [Parameter] public Func<TNode, IEnumerable<TNode>> GetChildNodes { get; set; }

    /// <summary>
    /// Indicates if the node has child elements.
    /// </summary>
    [Parameter] public Func<TNode, bool> HasChildNodes { get; set; }

    /// <summary>
    /// Gets the list of child nodes for each node.
    /// </summary>
    [Parameter] public Func<TNode, Task<IEnumerable<TNode>>> GetChildNodesAsync { get; set; }

    /// <summary>
    /// Indicates the node's disabled state. Used for preventing selection.
    /// </summary>
    [Parameter] public Func<TNode, bool> IsDisabled { get; set; }

    /// <summary>
    /// Indicates if the node has child elements.
    /// </summary>
    [Parameter] public Func<TNode, Task<bool>> HasChildNodesAsync { get; set; }

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
    /// The event is fired when the node is right clicked to show the context menu.
    /// </summary>
    [Parameter] public EventCallback<TreeViewNodeMouseEventArgs<TNode>> NodeContextMenu { get; set; }

    /// <summary>
    /// Used to prevent the default action for a <see cref="NodeContextMenu"/> event.
    /// </summary>
    [Parameter] public bool NodeContextMenuPreventDefault { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TreeView{TNode}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Enables native dragging of tree nodes.
    /// </summary>
    [Parameter] public bool Draggable { get; set; }

    /// <summary>
    /// Enables reordering dragged tree nodes before other nodes.
    /// </summary>
    [Parameter] public bool Reorderable { get; set; }

    /// <summary>
    /// Determines whether a node can be dragged.
    /// </summary>
    [Parameter] public Func<TNode, bool> CanDragNode { get; set; } = _ => true;

    /// <summary>
    /// Determines whether the dragged node can be dropped on the target node.
    /// </summary>
    [Parameter] public Func<TreeViewNodeDragEventArgs<TNode>, bool> CanDropNode { get; set; } = _ => true;

    /// <summary>
    /// Fired when a dragged node is dropped on a target node.
    /// </summary>
    [Parameter] public EventCallback<TreeViewNodeDragEventArgs<TNode>> NodeDropped { get; set; }

    #endregion
}