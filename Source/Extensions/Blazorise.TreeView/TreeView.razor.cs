#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.TreeView.Extensions;
using Blazorise.TreeView.Internal;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView;

public partial class TreeView<TNode> : BaseComponent, IDisposable
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
                await foreach ( var nodeState in e.NewItems.ToNodeStates( HasChildNodesAsync, HasChildNodes, ( node ) => ExpandedNodes?.Contains( node ) == true, IsDisabled ) )
                {
                    treeViewNodeStates.Add( nodeState );
                }
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

        await foreach ( var nodeState in Nodes.ToNodeStates( HasChildNodesAsync, HasChildNodes, ( node ) => ExpandedNodes?.Contains( node ) == true, IsDisabled ) )
        {
            treeViewNodeStates.Add( nodeState );
        }

        ////traverse nodeStateTree and fill in children for expanded nodes
        //foreach ( var nodeState in treeViewNodeStates )
        //{
        //    if ( nodeState.Expanded && nodeState.HasChildren )
        //    {
        //        nodeState.Children = nodeState.n
        //    }
        //}

        await InvokeAsync( StateHasChanged );
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
    [Parameter] public Func<TNode, bool> HasChildNodes { get; set; } = node => false;

    /// <summary>
    /// Gets the list of child nodes for each node.
    /// </summary>
    [Parameter] public Func<TNode, Task<IEnumerable<TNode>>> GetChildNodesAsync { get; set; }

    /// <summary>
    /// Indicates the node's disabled state. Used for preventing selection.
    /// </summary>
    [Parameter] public Func<TNode, bool> IsDisabled { get; set; } = node => false;

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
    /// Specifies the content to be rendered inside this <see cref="TreeView{TNode}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}