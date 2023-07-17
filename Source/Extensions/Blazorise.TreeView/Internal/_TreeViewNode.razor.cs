#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.TreeView.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.TreeView.Internal;

public partial class _TreeViewNode<TNode> : BaseComponent
{
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

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-tree-view-node" );
        builder.Append( "b-tree-view-node-collapsed", !Expanded );

        base.BuildClasses( builder );
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

        NotifyCollectionChangedEventHandler childrenChangedHandler = ( ( sender, e ) =>
        {
            OnChildrenChanged( sender, e, nodeState, childNodes );
        } );

        if ( childNodes is INotifyCollectionChanged observableCollection )
        {
            observableCollection.CollectionChanged += childrenChangedHandler;
        }

        if ( !nodeState.Children.Select( x => x.Node ).AreEqual( childNodes ) )
        {
            await ReloadChildren( nodeState, childNodes );
        }

    }

    private async Task ReloadChildren( TreeViewNodeState<TNode> nodeState, IEnumerable<TNode> childNodes )
    {
        nodeState.Children.Clear();

        await foreach ( var childNodeState in childNodes.ToNodeStates( HasChildNodesAsync, HasChildNodes, ( node ) => ExpandedNodes?.Contains( node ) == true ) )
        {
            nodeState.Children.Add( childNodeState );
        }
    }

    private async void OnChildrenChanged( object sender, NotifyCollectionChangedEventArgs e, TreeViewNodeState<TNode> nodeState, IEnumerable<TNode> childNodes )
    {
        if ( e.Action == NotifyCollectionChangedAction.Add )
        {
            await foreach ( var childNodeState in e.NewItems.ToNodeStates( HasChildNodesAsync, HasChildNodes, ( node ) => ExpandedNodes?.Contains( node ) == true ) )
            {
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

    #endregion

    #region Properties

    [Parameter] public IEnumerable<TreeViewNodeState<TNode>> NodeStates { get; set; }

    [Parameter] public RenderFragment<TNode> NodeContent { get; set; }

    [Parameter] public IList<TNode> ExpandedNodes { get; set; } = new List<TNode>();

    [Parameter] public EventCallback<IList<TNode>> ExpandedNodesChanged { get; set; }

    [Parameter] public Func<TNode, IEnumerable<TNode>> GetChildNodes { get; set; }

    [Parameter] public Func<TNode, bool> HasChildNodes { get; set; } = node => false;

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
    /// Gets or sets node styling.
    /// </summary>
    [Parameter] public Action<TNode, NodeStyling> NodeStyling { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="_TreeViewNode{TNode}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// A reference to the parent node component.
    /// </summary>
    [CascadingParameter] public _TreeViewNode<TNode> ParentNode { get; set; }

    #endregion
}