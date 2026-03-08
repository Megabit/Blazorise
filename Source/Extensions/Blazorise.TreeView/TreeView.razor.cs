#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Licensing;
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

    private void OnCollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
    {
        if ( e.Action == NotifyCollectionChangedAction.Add )
        {
            InvokeAsync( async () =>
            {
                IEnumerable<TNode> newNodes = e.NewItems?.OfType<TNode>() ?? Enumerable.Empty<TNode>();

                foreach ( var nodeState in await CreateNodeStatesAsync( newNodes ) )
                {
                    AddTreeViewNodeState( nodeState );
                }

                StateHasChanged();
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

        foreach ( var nodeState in await CreateNodeStatesAsync( Nodes ) )
        {
            AddTreeViewNodeState( nodeState );
        }

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Triggers reload of a specific <see cref="TreeView{TNode}"/> node.
    /// </summary>
    /// <param name="node">Node to reload.</param>
    /// <returns>Returns the awaitable task.</returns>
    public async Task ReloadNode( TNode node )
    {
        if ( treeViewNodeStates.IsNullOrEmpty() )
            return;

        if ( !TryFindNodeState( treeViewNodeStates, node, out IList<TreeViewNodeState<TNode>> nodeStates, out int nodeIndex ) )
            return;

        TreeViewNodeState<TNode> previousNodeState = nodeStates[nodeIndex];
        TreeViewNodeState<TNode> updatedNodeState = await CreateNodeStateAsync( node );

        await SynchronizeReloadedNodeState( previousNodeState, updatedNodeState );

        nodeStates[nodeIndex] = updatedNodeState;

        DirtyClasses();
        await InvokeAsync( StateHasChanged );
    }

    private void AddTreeViewNodeState( TreeViewNodeState<TNode> treeViewNodeState )
    {
        var maxRowsLimit = BlazoriseLicenseLimitsHelper.GetTreeViewRowsLimit( LicenseChecker );

        if ( maxRowsLimit.HasValue )
        {
            if ( treeViewNodeStates?.Count >= maxRowsLimit.Value )
            {
                return;
            }
        }

        treeViewNodeStates.Add( treeViewNodeState );
    }

    internal async Task<TreeViewNodeState<TNode>> CreateNodeStateAsync( TNode node )
    {
        bool hasChildren = HasChildNodesAsync is not null
            ? await HasChildNodesAsync( node )
            : DetermineHasChildNodes( node );

        bool expanded = ExpandedNodes?.Contains( node ) == true;
        bool disabled = DetermineIsDisabled( node );

        return new TreeViewNodeState<TNode>( node, hasChildren, expanded, disabled );
    }

    internal async Task<List<TreeViewNodeState<TNode>>> CreateNodeStatesAsync( IEnumerable<TNode> nodes )
    {
        List<TreeViewNodeState<TNode>> nodeStates = new();

        foreach ( TNode node in nodes ?? Enumerable.Empty<TNode>() )
        {
            nodeStates.Add( await CreateNodeStateAsync( node ) );
        }

        return nodeStates;
    }

    internal async Task<IEnumerable<TNode>> ResolveChildNodesAsync( TNode node )
    {
        return GetChildNodesAsync is not null
            ? await GetChildNodesAsync( node )
            : GetChildNodes is not null
                ? GetChildNodes( node )
                : null;
    }

    private bool TryFindNodeState( IList<TreeViewNodeState<TNode>> nodeStates, TNode node, out IList<TreeViewNodeState<TNode>> parentNodeStates, out int nodeIndex )
    {
        if ( nodeStates is not null )
        {
            for ( int i = 0; i < nodeStates.Count; i++ )
            {
                TreeViewNodeState<TNode> nodeState = nodeStates[i];

                if ( nodeState.Node.IsEqual( node ) )
                {
                    parentNodeStates = nodeStates;
                    nodeIndex = i;
                    return true;
                }

                if ( TryFindNodeState( nodeState.Children, node, out parentNodeStates, out nodeIndex ) )
                    return true;
            }
        }

        parentNodeStates = null;
        nodeIndex = -1;
        return false;
    }

    private async Task SynchronizeReloadedNodeState( TreeViewNodeState<TNode> previousNodeState, TreeViewNodeState<TNode> updatedNodeState )
    {
        HashSet<TNode> loadedNodes = GetLoadedNodes( previousNodeState );
        HashSet<TNode> currentNodes = await GetCurrentNodesAsync( updatedNodeState.Node );

        if ( !updatedNodeState.HasChildren )
        {
            updatedNodeState.Expanded = false;
        }

        bool expandedNodesChanged = RemoveMissingNodes( ExpandedNodes, loadedNodes, currentNodes );

        if ( !updatedNodeState.HasChildren )
        {
            expandedNodesChanged |= ExpandedNodes.Remove( updatedNodeState.Node );
        }

        if ( expandedNodesChanged )
        {
            await ExpandedNodesChanged.InvokeAsync( ExpandedNodes );
        }

        if ( loadedNodes.Contains( treeViewState.SelectedNode ) && !currentNodes.Contains( treeViewState.SelectedNode ) )
        {
            treeViewState = treeViewState with { SelectedNode = default };
            await SelectedNodeChanged.InvokeAsync( treeViewState.SelectedNode );
        }

        if ( RemoveMissingNodes( treeViewState.SelectedNodes, loadedNodes, currentNodes ) )
        {
            await SelectedNodesChanged.InvokeAsync( treeViewState.SelectedNodes );
        }
    }

    private HashSet<TNode> GetLoadedNodes( TreeViewNodeState<TNode> nodeState )
    {
        HashSet<TNode> nodes = new( EqualityComparer<TNode>.Default );

        CollectLoadedNodes( nodeState, nodes );

        return nodes;
    }

    private void CollectLoadedNodes( TreeViewNodeState<TNode> nodeState, HashSet<TNode> nodes )
    {
        if ( nodeState is null || !nodes.Add( nodeState.Node ) )
            return;

        foreach ( TreeViewNodeState<TNode> childNodeState in nodeState.Children ?? Enumerable.Empty<TreeViewNodeState<TNode>>() )
        {
            CollectLoadedNodes( childNodeState, nodes );
        }
    }

    private async Task<HashSet<TNode>> GetCurrentNodesAsync( TNode node )
    {
        HashSet<TNode> nodes = new( EqualityComparer<TNode>.Default );

        await CollectCurrentNodesAsync( node, nodes );

        return nodes;
    }

    private async Task CollectCurrentNodesAsync( TNode node, HashSet<TNode> nodes )
    {
        if ( !nodes.Add( node ) )
            return;

        IEnumerable<TNode> childNodes = await ResolveChildNodesAsync( node );

        foreach ( TNode childNode in childNodes ?? Enumerable.Empty<TNode>() )
        {
            await CollectCurrentNodesAsync( childNode, nodes );
        }
    }

    private static bool RemoveMissingNodes( IList<TNode> nodes, HashSet<TNode> previousNodes, HashSet<TNode> currentNodes )
    {
        if ( nodes is null )
            return false;

        bool nodesChanged = false;

        for ( int i = nodes.Count - 1; i >= 0; i-- )
        {
            TNode node = nodes[i];

            if ( previousNodes.Contains( node ) && !currentNodes.Contains( node ) )
            {
                nodes.RemoveAt( i );
                nodesChanged = true;
            }
        }

        return nodesChanged;
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
    /// Indicates if the node has child elements.
    /// </summary>
    protected Func<TNode, bool> DetermineHasChildNodes => HasChildNodes ?? ( node => false );

    /// <summary>
    /// Indicates the node's disabled state. Used for preventing selection.
    /// </summary>
    protected Func<TNode, bool> DetermineIsDisabled => IsDisabled ?? ( node => false );

    /// <summary>
    /// Gets or sets the license checker for the user session.
    /// </summary>
    [Inject] internal BlazoriseLicenseChecker LicenseChecker { get; set; }

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
    [Parameter]
    public TNode SelectedNode { get; set; }

    /// <summary>
    /// Occurs when the selected TreeView node has changed.
    /// </summary>
    [Parameter] public EventCallback<TNode> SelectedNodeChanged { get; set; }

    /// <summary>
    /// Currently selected TreeView items/nodes.
    /// </summary>
    [Parameter]
    public IList<TNode> SelectedNodes { get; set; }

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

    #endregion
}