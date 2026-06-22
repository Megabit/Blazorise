#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Arranges docked panes around a central content surface.
/// </summary>
public partial class DockLayout : BaseComponent
{
    #region Members

    private readonly DockLayoutRegistry registry = new();

    private readonly DockDragState dragState = new();

    private readonly DockLayoutStateManager stateManager = new();

    private readonly DockLayoutContext context;

    private readonly DockLayoutTreeQuery treeQuery;

    private readonly DockLayoutSizer sizer;

    private readonly DockLayoutTreeBuilder treeBuilder;

    private readonly DockLayoutTreeMutator treeMutator;

    private DockLayoutState state;

    private DotNetObjectReference<DockLayout> dotNetObjectRef;

    private int nextNodeId;

    private int renderVersion;

    private static readonly DockCompassZoneInfo[] dockCompassZones =
    {
        new( DockZone.Top, DockCompassZone.TopOuter, "TopOuter" ),
        new( DockZone.Top, DockCompassZone.TopInner, "TopInner" ),
        new( DockZone.Left, DockCompassZone.LeftOuter, "LeftOuter" ),
        new( DockZone.Left, DockCompassZone.LeftInner, "LeftInner" ),
        new( DockZone.Center, DockCompassZone.Center, "Center" ),
        new( DockZone.Right, DockCompassZone.RightInner, "RightInner" ),
        new( DockZone.Right, DockCompassZone.RightOuter, "RightOuter" ),
        new( DockZone.Bottom, DockCompassZone.BottomInner, "BottomInner" ),
        new( DockZone.Bottom, DockCompassZone.BottomOuter, "BottomOuter" ),
    };

    #endregion

    #region Constructors

    /// <summary>
    /// Default <see cref="DockLayout"/> constructor.
    /// </summary>
    public DockLayout()
    {
        context = new( this );
        treeQuery = new( registry, stateManager, () => CurrentState );
        sizer = new( registry, stateManager, treeQuery, () => CurrentState );
        treeBuilder = new( registry, stateManager, treeQuery, sizer );
        treeMutator = new( treeQuery, sizer );
        DockCompassStyleBuilder = new( BuildDockCompassStyles );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockLayout() );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds styles for the dock compass element.
    /// </summary>
    /// <param name="builder">Style builder used to append the styles.</param>
    protected virtual void BuildDockCompassStyles( StyleBuilder builder )
    {
        builder.Append( $"left:{dragState.CompassX}px" );
        builder.Append( $"top:{dragState.CompassY}px" );
    }

    /// <inheritdoc/>
    protected internal override void DirtyStyles()
    {
        DockCompassStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    /// <summary>
    /// Returns the current mutable docking state.
    /// </summary>
    /// <returns>The current docking state instance.</returns>
    public DockLayoutState GetState()
    {
        EnsureCurrentStateInitialized();
        NormalizeCurrentState();

        return CurrentState;
    }

    /// <summary>
    /// Loads a docking state and applies it to the current layout.
    /// </summary>
    /// <param name="state">The docking state to load.</param>
    /// <returns>A task that completes after the state has been applied.</returns>
    public async Task LoadState( DockLayoutState state )
    {
        this.state = state ?? new();

        EnsureCurrentStateInitialized();

        await NotifyStateChanged();
    }

    /// <summary>
    /// Resets the docking state to the declarative layout definition.
    /// </summary>
    /// <returns>A task that completes after the state has been reset.</returns>
    public Task ResetState()
        => LoadState( new DockLayoutState() );

    /// <summary>
    /// Indicates whether a pane is currently open.
    /// </summary>
    /// <param name="paneName">The name of the pane to query.</param>
    /// <returns>True when the pane is open; otherwise false.</returns>
    public bool IsPaneOpen( string paneName )
    {
        return TryGetPane( paneName, out DockPane pane )
            && stateManager.EnsurePaneState( CurrentState, pane ).Visible;
    }

    /// <summary>
    /// Opens a pane that was previously closed.
    /// </summary>
    /// <param name="paneName">The name of the pane to open.</param>
    /// <returns>A task that completes after the pane has been opened.</returns>
    public async Task OpenPane( string paneName )
    {
        if ( !TryGetPane( paneName, out DockPane pane ) )
            return;

        DockPaneState paneState = stateManager.EnsurePaneState( CurrentState, pane );

        paneState.Visible = true;
        paneState.AutoHide = false;

        stateManager.RemoveRailItem( CurrentState, paneState.Name );
        AddPaneToLayout( paneState );

        await NotifyStateChanged();
    }

    /// <summary>
    /// Toggles a pane between opened and closed states.
    /// </summary>
    /// <param name="paneName">The name of the pane to toggle.</param>
    /// <returns>A task that completes after the pane visibility has been toggled.</returns>
    public Task TogglePane( string paneName )
    {
        if ( !TryGetPane( paneName, out DockPane pane ) )
            return Task.CompletedTask;

        DockPaneState paneState = stateManager.EnsurePaneState( CurrentState, pane );

        return paneState.Visible
            ? ClosePane( paneName )
            : OpenPane( paneName );
    }

    internal void RegisterPane( DockPane pane )
    {
        if ( registry.RegisterPane( pane ) )
            stateManager.EnsurePaneState( CurrentState, pane );
    }

    internal void RegisterContent( DockContent dockContent )
    {
        registry.RegisterContent( dockContent );
    }

    internal Task NotifyDefinitionChanged()
        => InvokeAsync( async () =>
        {
            EnsureCurrentStateInitialized();

            await NotifyStateChanged();
        } );

    internal void UnregisterPane( DockPane pane )
    {
        registry.UnregisterPane( pane );
    }

    internal DockPaneState GetPaneState( DockPane pane )
        => stateManager.EnsurePaneState( CurrentState, pane );

    internal DockPanePosition GetPanePosition( DockPane pane )
        => treeQuery.GetPanePosition( pane );

    internal IReadOnlyList<DockPaneState> GetPaneStates( DockPanePosition position )
        => CurrentState.Panes
            .Where( x => x.Visible && !x.AutoHide && x.Position == position )
            .OrderBy( x => x.Order )
            .ToArray();

    internal IReadOnlyList<DockRailItemState> GetRailItems( DockPanePosition position )
        => stateManager.GetRailItems( CurrentState, registry.Panes, position );

    internal bool HasPaneTabs( DockPanePosition position )
        => GetPaneStates( position ).Count > 1;

    internal bool TryGetPane( string paneName, out DockPane pane )
        => registry.TryGetPane( paneName, out pane );

    internal DockPaneState GetPaneState( string paneName )
        => FindPaneState( paneName );

    internal DockNodeState GetNode( string nodeId )
        => treeQuery.GetNode( nodeId );

    internal bool IsPaneActive( DockPane pane )
        => treeQuery.IsPaneActive( pane );

    internal string GetPaneCaption( string paneName )
        => TryGetPane( paneName, out DockPane pane ) ? pane.ResolvedCaption : paneName;

    internal DockPanePosition? GetDockNodePosition( DockNodeState node )
        => treeQuery.GetDockNodePosition( node );

    internal bool CanResizeDockNode( DockNodeState node )
        => treeQuery.CanResizeDockNode( node );

    internal DockPaneTabsPlacement GetDockNodeTabsPlacement( DockNodeState node, DockPanePosition position )
    {
        if ( node?.Panes is not null )
        {
            foreach ( string paneName in node.Panes )
            {
                if ( TryGetPane( paneName, out DockPane pane ) && pane.EffectiveTabsPlacement != DockPaneTabsPlacement.Default )
                    return pane.EffectiveTabsPlacement;
            }
        }

        return position == DockPanePosition.Center
            ? DockPaneTabsPlacement.Top
            : DockPaneTabsPlacement.Bottom;
    }

    internal string GetDockSplitStyle( DockNodeState node )
        => sizer.GetDockSplitStyle( node );

    internal bool IsDockPaneBordered( DockPanePosition position )
        => PaneBordered && position != DockPanePosition.Center;

    internal string GetActiveTabPaneName( DockNodeState node )
        => treeQuery.GetActiveTabPaneName( node );

    internal async Task ActivateTab( DockNodeState node, string paneName )
    {
        if ( node?.Kind == DockNodeKind.Tabs && node.Panes.Contains( paneName ) && node.ActivePane != paneName )
        {
            node.ActivePane = paneName;
            await NotifyStateChanged();
        }
    }

    internal Task ActivateTab( string nodeId, string paneName )
        => ActivateTab( GetNode( nodeId ), paneName );

    internal async Task ActivatePane( string paneName )
    {
        DockNodeState tabsNode = treeQuery.FindTabsNode( paneName );

        if ( tabsNode is not null && tabsNode.Panes.Contains( paneName ) && tabsNode.ActivePane != paneName )
        {
            tabsNode.ActivePane = paneName;
            await NotifyStateChanged();
        }
    }

    internal async Task TogglePaneAutoHide( DockPane pane )
    {
        if ( pane is null )
            return;

        DockPaneState paneState = stateManager.EnsurePaneState( CurrentState, pane );
        DockRailItemState railItem = stateManager.FindRailItem( CurrentState, paneState.Name );

        if ( railItem is not null || paneState.AutoHide )
        {
            await OpenPaneAutoHide( pane );
            return;
        }

        DockNodeState tabsNode = treeQuery.FindTabsNode( paneState.Name );
        DockRailItemState relatedTabRailItem = tabsNode is null
            ? stateManager.FindRailItemBySourceTabPaneName( CurrentState, paneState.Name )
            : null;
        DockRailItemState relatedSplitRailItem = tabsNode is null && relatedTabRailItem is null
            ? stateManager.FindRailItemBySourceTargetPaneName( CurrentState, paneState.Name )
            : null;
        DockRailItemState relatedRailItem = relatedTabRailItem ?? relatedSplitRailItem;
        bool relatedToHiddenSplitTarget = relatedSplitRailItem is not null;
        DockLayoutTreeQuery.DockNodePlacement sourcePlacement = treeQuery.FindPanePlacement( paneState.Name );
        DockLayoutTreeQuery.DockNodePlacement sourceGroupPlacement = sourcePlacement?.Parent is null
            ? null
            : treeQuery.FindDockNodePlacement( sourcePlacement.Parent );
        DockPanePosition sourcePosition = tabsNode is null
            ? relatedRailItem?.SourcePosition ?? treeQuery.GetPanePosition( pane )
            : treeQuery.GetDockNodePosition( tabsNode ) ?? paneState.Position;
        string sourceGroupId = tabsNode?.Id ?? relatedRailItem?.SourceGroupId ?? sourcePlacement?.Parent?.Id ?? paneState.Name;
        string sourceTabPaneName = tabsNode?.Panes.FirstOrDefault( x => x != paneState.Name ) ?? relatedTabRailItem?.PaneName;
        string sourceSize = tabsNode?.Size ?? relatedRailItem?.SourceSize ?? paneState.Size ?? pane.Size;
        string sourceTargetPaneName = relatedToHiddenSplitTarget
            ? relatedRailItem.PaneName
            : relatedRailItem?.SourceTargetPaneName ?? treeQuery.GetFirstDockNodePaneName( sourcePlacement?.Sibling );
        string sourceTargetNodeId = relatedToHiddenSplitTarget
            ? null
            : relatedRailItem?.SourceTargetNodeId ?? sourcePlacement?.Sibling?.Id;
        DockZone? sourceZone = relatedToHiddenSplitTarget
            ? InvertDockZone( relatedRailItem.SourceZone )
            : relatedRailItem?.SourceZone ?? sourcePlacement?.Zone;
        double? sourceSplitRatio = relatedRailItem?.SourceSplitRatio ?? sourcePlacement?.Parent?.Ratio;
        bool? sourceSplitUseRatio = relatedRailItem?.SourceSplitUseRatio ?? sourcePlacement?.Parent?.UseRatio;
        string sourceGroupTargetPaneName = relatedRailItem?.SourceGroupTargetPaneName ?? treeQuery.GetFirstDockNodePaneName( sourceGroupPlacement?.Sibling );
        string sourceGroupTargetNodeId = relatedRailItem?.SourceGroupTargetNodeId ?? sourceGroupPlacement?.Sibling?.Id;
        DockZone? sourceGroupZone = relatedRailItem?.SourceGroupZone ?? sourceGroupPlacement?.Zone;
        double? sourceGroupSplitRatio = relatedRailItem?.SourceGroupSplitRatio ?? sourceGroupPlacement?.Parent?.Ratio;
        bool? sourceGroupSplitUseRatio = relatedRailItem?.SourceGroupSplitUseRatio ?? sourceGroupPlacement?.Parent?.UseRatio;
        int sourceIndex = tabsNode?.Panes.IndexOf( paneState.Name ) ?? GetRelatedRailItemSourceIndex( relatedRailItem );

        paneState.AutoHide = true;
        paneState.Position = sourcePosition;

        stateManager.AddRailItem( CurrentState, GetRailPosition( sourcePosition ), new()
        {
            PaneName = paneState.Name,
            SourceGroupId = sourceGroupId,
            SourceTabPaneName = sourceTabPaneName,
            SourcePosition = sourcePosition,
            SourceSize = sourceSize,
            SourceSplitRatio = sourceSplitRatio,
            SourceSplitUseRatio = sourceSplitUseRatio,
            SourceGroupTargetPaneName = sourceGroupTargetPaneName,
            SourceGroupTargetNodeId = sourceGroupTargetNodeId,
            SourceGroupZone = sourceGroupZone,
            SourceGroupSplitRatio = sourceGroupSplitRatio,
            SourceGroupSplitUseRatio = sourceGroupSplitUseRatio,
            SourceTargetPaneName = sourceTargetPaneName,
            SourceTargetNodeId = sourceTargetNodeId,
            SourceZone = sourceZone,
            SourceIndex = sourceIndex,
        } );

        CurrentState.Root = treeMutator.RemovePaneNode( CurrentState.Root, paneState.Name );

        await NotifyStateChanged();
    }

    internal async Task OpenPaneAutoHide( DockPane pane )
    {
        if ( pane is null )
            return;

        DockPaneState paneState = stateManager.EnsurePaneState( CurrentState, pane );
        DockRailItemState railItem = stateManager.FindRailItem( CurrentState, paneState.Name );

        if ( railItem is not null )
        {
            stateManager.RemoveRailItem( CurrentState, railItem.PaneName );

            paneState.AutoHide = false;
            paneState.Position = railItem.SourcePosition;

            if ( !string.IsNullOrWhiteSpace( railItem.SourceSize ) )
                paneState.Size = railItem.SourceSize;

            if ( RestorePaneToSourceTabs( railItem, paneState.Name ) )
            {
                await NotifyStateChanged();
                return;
            }

            DockNodeState restoredNode = new()
            {
                Kind = DockNodeKind.Pane,
                PaneName = paneState.Name,
                Size = railItem.SourceSize,
            };

            bool restoredToTarget = railItem.SourceZone is not null
                && treeMutator.AddNodeToTarget(
                    CurrentState,
                    restoredNode,
                    railItem.SourceTargetPaneName,
                    railItem.SourceTargetNodeId,
                    railItem.SourceZone.Value,
                    railItem.SourceSplitRatio,
                    railItem.SourceSplitUseRatio );

            if ( !restoredToTarget )
            {
                restoredToTarget = railItem.SourceGroupZone is not null
                    && treeMutator.AddNodeToTarget(
                        CurrentState,
                        restoredNode,
                        railItem.SourceGroupTargetPaneName,
                        railItem.SourceGroupTargetNodeId,
                        railItem.SourceGroupZone.Value,
                        railItem.SourceGroupSplitRatio,
                        railItem.SourceGroupSplitUseRatio );
            }

            if ( !restoredToTarget )
                treeMutator.AddNodeToPosition( CurrentState, restoredNode, railItem.SourcePosition );

            if ( !string.IsNullOrWhiteSpace( railItem.SourceTabPaneName ) )
                stateManager.UpdateRailGroupSourceTabPane( CurrentState, railItem.SourceGroupId, paneState.Name );

            await NotifyStateChanged();

            return;
        }

        DockNodeState tabsNode = treeQuery.FindTabsNode( paneState.Name );

        if ( tabsNode is not null )
        {
            foreach ( string paneName in tabsNode.Panes )
                stateManager.SetPaneAutoHide( CurrentState, paneName, false );

            tabsNode.ActivePane = paneState.Name;
        }
        else
        {
            paneState.AutoHide = false;
        }

        await NotifyStateChanged();
    }

    internal async Task ClosePane( DockPane pane )
    {
        if ( pane is null )
            return;

        await ClosePane( pane.ResolvedName );
    }

    /// <summary>
    /// Closes a pane and removes it from the visible layout.
    /// </summary>
    /// <param name="paneName">The name of the pane to close.</param>
    /// <returns>A task that completes after the pane has been closed.</returns>
    public async Task ClosePane( string paneName )
    {
        if ( !TryGetPane( paneName, out DockPane pane ) )
            return;

        DockPaneState paneState = stateManager.EnsurePaneState( CurrentState, pane );

        paneState.Visible = false;
        paneState.AutoHide = false;

        stateManager.RemoveRailItem( CurrentState, paneState.Name );
        CurrentState.Root = treeMutator.RemovePaneNode( CurrentState.Root, paneState.Name );

        await NotifyStateChanged();
    }

    internal bool IsPaneTabCloseButtonVisible( string paneName, DockPanePosition position )
        => position == DockPanePosition.Center
            && TryGetPane( paneName, out DockPane pane )
            && pane.Closable
            && pane.EffectiveShowTabCloseButton;

    internal async Task BeginPaneResize( DockPane pane, string nodeId, DockPanePosition dock, PointerEventArgs eventArgs )
    {
        if ( pane?.Resizable != true )
            return;

        await BeginNodeResize(
            pane.ElementRef,
            pane.ResolvedName,
            nodeId,
            dock,
            eventArgs,
            pane.MinSize,
            pane.MaxSize );
    }

    internal async Task BeginNodeResize( ElementReference elementRef, string paneName, string nodeId, DockPanePosition dock, PointerEventArgs eventArgs, string minSize = null, string maxSize = null )
    {
        DockNodeState resizingNode = GetNode( paneName );

        if ( resizingNode?.Kind == DockNodeKind.Split && !CanResizeDockNode( resizingNode ) )
            return;

        await JSModule.BeginResize(
            DotNetObjectRef,
            elementRef,
            paneName,
            nodeId,
            dock.ToString(),
            eventArgs.ClientX,
            eventArgs.ClientY,
            minSize,
            maxSize );
    }

    internal Task BeginPaneTabDrag( string paneName, PointerEventArgs eventArgs )
    {
        if ( !TryGetPane( paneName, out DockPane pane ) )
            return Task.CompletedTask;

        return BeginPaneDrag( pane, eventArgs, false );
    }

    internal async Task BeginPaneDrag( DockPane pane, PointerEventArgs eventArgs, bool dragGroup )
    {
        if ( pane?.Movable != true )
            return;

        dragState.Group = dragGroup;

        await ActivatePane( pane.ResolvedName );

        await JSModule.BeginDrag(
            DotNetObjectRef,
            ElementRef,
            pane.ResolvedName,
            eventArgs.ClientX,
            eventArgs.ClientY,
            dragGroup );
    }

    /// <summary>
    /// Updates a dock pane size while a splitter resize operation is active.
    /// </summary>
    /// <param name="paneName">The resized pane name.</param>
    /// <param name="nodeId">The resized split node id.</param>
    /// <param name="ratio">The new split ratio.</param>
    /// <returns>A task that completes after the state is updated.</returns>
    [JSInvokable]
    public async Task NotifyDockPaneResized( string paneName, string nodeId, string ratio )
    {
        DockNodeState resizedNode = DockLayoutTreeQuery.FindNodeById( CurrentState.Root, nodeId );

        if ( resizedNode?.Kind == DockNodeKind.Split && double.TryParse( ratio, NumberStyles.Float, CultureInfo.InvariantCulture, out double splitRatio ) )
        {
            resizedNode.Ratio = Math.Clamp( splitRatio, 0.02d, 0.98d );
            resizedNode.UseRatio = true;
        }
        else
        {
            DockPaneState paneState = FindPaneState( paneName );

            if ( paneState is null )
                return;

            if ( resizedNode is not null )
                resizedNode.Size = ratio;
            else if ( treeQuery.FindTabsNode( paneName ) is DockNodeState tabsNode )
                tabsNode.Size = ratio;
            else
                paneState.Size = ratio;
        }

        await NotifyStateChanged();
    }

    /// <summary>
    /// Completes a dock pane splitter resize operation.
    /// </summary>
    /// <param name="paneName">The resized pane name.</param>
    /// <returns>A task that completes after the state is updated.</returns>
    [JSInvokable]
    public Task NotifyDockPaneResizeEnded( string paneName )
        => NotifyStateChanged();

    /// <summary>
    /// Updates the currently highlighted dock drop zone while a pane is dragged.
    /// </summary>
    /// <param name="paneName">The dragged pane name.</param>
    /// <param name="targetName">The pane currently under the pointer.</param>
    /// <param name="targetNodeId">The dock node currently under the pointer.</param>
    /// <param name="zone">The currently hovered drop zone.</param>
    /// <param name="compassZoneKey">The exact compass zone currently under the pointer.</param>
    /// <param name="compassX">The horizontal compass location relative to the layout.</param>
    /// <param name="compassY">The vertical compass location relative to the layout.</param>
    /// <returns>A completed task.</returns>
    [JSInvokable]
    public Task NotifyDockPaneDrag( string paneName, string targetName, string targetNodeId, string zone, string compassZoneKey, double compassX, double compassY )
    {
        dragState.Update( paneName, DockLayoutTreeMutator.ToDockZone( zone ), compassZoneKey, compassX, compassY );
        DirtyStyles();
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Completes a pane drag operation and docks the pane in the selected zone.
    /// </summary>
    /// <param name="paneName">The dragged pane name.</param>
    /// <param name="targetName">The pane currently under the pointer.</param>
    /// <param name="targetNodeId">The dock node currently under the pointer.</param>
    /// <param name="zone">The selected drop zone.</param>
    /// <returns>A task that completes after the state is updated.</returns>
    [JSInvokable]
    public async Task NotifyDockPaneDropped( string paneName, string targetName, string targetNodeId, string zone )
    {
        DockZone? targetZone = DockLayoutTreeMutator.ToDockZone( zone );
        DockPaneState paneState = FindPaneState( paneName );
        bool moveGroup = dragState.Group;

        dragState.Clear();

        if ( paneState is not null && targetZone is not null )
        {
            DockNodeState sourceTabsNode = moveGroup ? treeQuery.FindTabsNode( paneName ) : null;
            IReadOnlyList<string> movedPaneNames = sourceTabsNode?.Panes.Count > 1
                ? sourceTabsNode.Panes.ToArray()
                : new[] { paneName };
            targetName = ResolveDropTargetName( paneName, targetName, targetNodeId, moveGroup );

            DockPaneState targetState = FindPaneState( targetName );
            bool targetExists = !string.IsNullOrWhiteSpace( targetName ) && !movedPaneNames.Contains( targetName ) && DockLayoutTreeQuery.ContainsPane( CurrentState.Root, targetName );
            bool mergeWithTargetTabs = targetExists && targetZone.Value == DockZone.Center;
            DockPanePosition? targetPosition = treeMutator.GetDropPanePosition( targetState, targetZone.Value, mergeWithTargetTabs );

            if ( targetPosition is not null )
            {
                foreach ( string movedPaneName in movedPaneNames )
                {
                    DockPaneState movedPaneState = FindPaneState( movedPaneName );

                    if ( movedPaneState is not null )
                        movedPaneState.Position = targetPosition.Value;
                }
            }

            if ( sourceTabsNode is not null && sourceTabsNode.Panes.Count > 1 )
                treeMutator.MoveNodeToZone( CurrentState, sourceTabsNode, movedPaneNames, targetName, targetNodeId, targetZone.Value, mergeWithTargetTabs, targetExists );
            else
                treeMutator.MovePaneToZone( CurrentState, paneName, targetName, targetNodeId, targetZone.Value, mergeWithTargetTabs );

            await NotifyStateChanged();
        }

        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( dotNetObjectRef is not null )
            {
                dotNetObjectRef.Dispose();
                dotNetObjectRef = null;
            }

            if ( JSModule is not null )
                await JSModule.Cancel();
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( CurrentState.Root is null && registry.RootCollector.Nodes.Count > 0 )
        {
            EnsureCurrentStateInitialized();
            NormalizeCurrentState();
            await NotifyStateChanged();
        }
    }

    internal DockPaneState FindPaneState( string paneName )
        => stateManager.FindPaneState( CurrentState, paneName );

    private bool RestorePaneToSourceTabs( DockRailItemState railItem, string paneName )
    {
        if ( railItem is null || string.IsNullOrWhiteSpace( paneName ) )
            return false;

        DockNodeState sourceTabsNode = DockLayoutTreeQuery.FindNodeById( CurrentState.Root, railItem.SourceGroupId );

        if ( CanRestorePaneToSourceTabsNode( railItem, sourceTabsNode ) )
        {
            AddPaneToTabsNode( sourceTabsNode, paneName, railItem.SourceIndex );
            ApplyRestoredTabsNodeSize( sourceTabsNode, railItem.SourceSize );
            return true;
        }

        if ( CanRestorePaneToSourceTabPane( railItem ) )
        {
            treeMutator.MovePaneToZone( CurrentState, paneName, railItem.SourceTabPaneName, null, DockZone.Center, true );
            sourceTabsNode = treeQuery.FindTabsNode( paneName );

            if ( sourceTabsNode?.Kind == DockNodeKind.Tabs )
            {
                AddPaneToTabsNode( sourceTabsNode, paneName, railItem.SourceIndex );
                ApplyRestoredTabsNodeSize( sourceTabsNode, railItem.SourceSize );
                return true;
            }

            CurrentState.Root = treeMutator.RemovePaneNode( CurrentState.Root, paneName );
        }

        return false;
    }

    private bool CanRestorePaneToSourceTabsNode( DockRailItemState railItem, DockNodeState sourceTabsNode )
        => sourceTabsNode?.Kind == DockNodeKind.Tabs
            && treeQuery.GetDockNodePosition( sourceTabsNode ) == railItem.SourcePosition;

    private bool CanRestorePaneToSourceTabPane( DockRailItemState railItem )
    {
        if ( string.IsNullOrWhiteSpace( railItem?.SourceTabPaneName )
             || !DockLayoutTreeQuery.ContainsPane( CurrentState.Root, railItem.SourceTabPaneName ) )
            return false;

        if ( treeQuery.GetPanePosition( railItem.SourceTabPaneName ) != railItem.SourcePosition )
            return false;

        if ( railItem.SourceZone is null || string.IsNullOrWhiteSpace( railItem.SourceTargetPaneName ) && string.IsNullOrWhiteSpace( railItem.SourceTargetNodeId ) )
            return true;

        bool sourceTargetExists = !string.IsNullOrWhiteSpace( railItem.SourceTargetNodeId ) && DockLayoutTreeQuery.FindNodeById( CurrentState.Root, railItem.SourceTargetNodeId ) is not null
            || !string.IsNullOrWhiteSpace( railItem.SourceTargetPaneName ) && DockLayoutTreeQuery.ContainsPane( CurrentState.Root, railItem.SourceTargetPaneName );

        if ( !sourceTargetExists )
            return true;

        DockLayoutTreeQuery.DockNodePlacement sourcePlacement = treeQuery.FindPanePlacement( railItem.SourceTabPaneName );

        if ( sourcePlacement?.Zone != railItem.SourceZone )
            return false;

        return !string.IsNullOrWhiteSpace( railItem.SourceTargetNodeId ) && sourcePlacement.Sibling?.Id == railItem.SourceTargetNodeId
            || !string.IsNullOrWhiteSpace( railItem.SourceTargetPaneName ) && DockLayoutTreeQuery.ContainsPane( sourcePlacement.Sibling, railItem.SourceTargetPaneName );
    }

    private string ResolveDropTargetName( string paneName, string targetName, string targetNodeId, bool moveGroup )
    {
        if ( moveGroup || targetName != paneName )
            return targetName;

        DockNodeState targetNode = treeQuery.GetNode( targetNodeId );

        if ( targetNode?.Kind == DockNodeKind.Tabs && targetNode.Panes.Count > 1 )
            return targetNode.Panes.FirstOrDefault( x => x != paneName ) ?? targetName;

        DockNodeState sourceTabsNode = treeQuery.FindTabsNode( paneName );

        return sourceTabsNode?.Panes.Count > 1
            ? sourceTabsNode.Panes.FirstOrDefault( x => x != paneName ) ?? targetName
            : targetName;
    }

    private static void AddPaneToTabsNode( DockNodeState tabsNode, string paneName, int sourceIndex )
    {
        tabsNode.Panes.Remove( paneName );
        tabsNode.Panes.Insert( Math.Clamp( sourceIndex, 0, tabsNode.Panes.Count ), paneName );
        tabsNode.ActivePane = paneName;
    }

    private static void ApplyRestoredTabsNodeSize( DockNodeState tabsNode, string sourceSize )
    {
        if ( tabsNode is not null && !string.IsNullOrWhiteSpace( sourceSize ) )
            tabsNode.Size = sourceSize;
    }

    private static int GetRelatedRailItemSourceIndex( DockRailItemState relatedRailItem )
    {
        if ( relatedRailItem is null )
            return 0;

        return relatedRailItem.SourceIndex == 0 ? 1 : 0;
    }

    private static DockZone? InvertDockZone( DockZone? zone )
        => zone switch
        {
            DockZone.Left => DockZone.Right,
            DockZone.Right => DockZone.Left,
            DockZone.Top => DockZone.Bottom,
            DockZone.Bottom => DockZone.Top,
            _ => zone,
        };

    private static DockPanePosition GetRailPosition( DockPanePosition position )
        => position == DockPanePosition.Center ? DockPanePosition.Right : position;

    private void AddPaneToLayout( DockPaneState paneState )
    {
        if ( paneState is null || DockLayoutTreeQuery.ContainsPane( CurrentState.Root, paneState.Name ) )
            return;

        DockNodeState paneNode = new()
        {
            Kind = DockNodeKind.Pane,
            PaneName = paneState.Name,
            Size = paneState.Size,
        };

        treeMutator.AddNodeToPosition( CurrentState, paneNode, paneState.Position );
    }

    private async Task NotifyStateChanged()
    {
        NormalizeCurrentState();

        renderVersion++;
        DirtyClasses();
        DirtyStyles();

        if ( StateChanged.HasDelegate )
            await StateChanged.InvokeAsync( CurrentState );

        StateHasChanged();
    }

    private void NormalizeCurrentState()
    {
        stateManager.Normalize( CurrentState, registry, treeQuery, ref nextNodeId );
    }

    private void EnsureCurrentStateInitialized()
    {
        foreach ( DockPane pane in registry.RegisteredPanes )
            stateManager.EnsurePaneState( CurrentState, pane );

        if ( CurrentState.Root is null && registry.RootCollector.Nodes.Count > 0 )
            CurrentState.Root = treeBuilder.BuildInitialRoot( CurrentState );
    }

    #endregion

    #region Properties

    internal bool DockGuidesVisible => dragState.Visible;

    internal static IReadOnlyList<DockCompassZoneInfo> DockCompassZones => dockCompassZones;

    internal string ActiveDockCompassZoneKey => dragState.CompassZoneKey;

    internal DockZone? ActiveDockZone => dragState.Zone;

    private DockLayoutState CurrentState => state ??= new();

    internal DockNodeCollector RootCollector => registry.RootCollector;

    internal DockContent Content => registry.Content;

    internal DockLayoutContext Context => context;

    internal int RenderVersion => renderVersion;

    internal int DockGuidesVersion => dragState.Version;

    private DotNetObjectReference<DockLayout> DotNetObjectRef => dotNetObjectRef ??= DotNetObjectReference.Create( this );

    /// <summary>
    /// Gets or sets the dock compass style-builder.
    /// </summary>
    protected StyleBuilder DockCompassStyleBuilder { get; private set; }

    /// <summary>
    /// Gets the styles for dock compass container.
    /// </summary>
    protected string DockCompassStyleNames => DockCompassStyleBuilder.Styles;

    /// <summary>
    /// Gets the provider class for the dock drag preview.
    /// </summary>
    protected string DragPreviewClassName => ClassProvider.DockLayoutDragPreview();

    /// <summary>
    /// Gets the provider class for the dock drop preview.
    /// </summary>
    protected string DropPreviewClassName => ClassProvider.DockLayoutDropPreview();

    /// <summary>
    /// Gets the DockLayout JavaScript module.
    /// </summary>
    [Inject] protected IJSDockLayoutModule JSModule { get; set; }

    /// <summary>
    /// Defines whether non-document panes should render a visible border.
    /// </summary>
    [Parameter] public bool PaneBordered { get; set; } = true;

    /// <summary>
    /// Defines the mutable state used for docking, resizing, active tabs, and pane visibility. The same state can be saved with <see cref="GetState"/> and restored with <see cref="LoadState"/>.
    /// </summary>
    [Parameter]
    public DockLayoutState State
    {
        get => state;
        set => state = value;
    }

    /// <summary>
    /// Occurs after the docking state changes.
    /// </summary>
    [Parameter] public EventCallback<DockLayoutState> StateChanged { get; set; }

    /// <summary>
    /// Specifies the panes and content to be rendered inside the dock layout.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion

    #region Nested types

    /// <summary>
    /// Contains metadata for a dock compass zone.
    /// </summary>
    public sealed class DockCompassZoneInfo
    {
        internal DockCompassZoneInfo( DockZone zone, DockCompassZone compassZone, string key )
        {
            Zone = zone;
            CompassZone = compassZone;
            Key = key;
        }

        /// <summary>
        /// Gets the dock operation zone.
        /// </summary>
        public DockZone Zone { get; }

        /// <summary>
        /// Gets the exact visual compass zone.
        /// </summary>
        public DockCompassZone CompassZone { get; }

        /// <summary>
        /// Gets the key used to match the active compass zone.
        /// </summary>
        public string Key { get; }
    }

    #endregion
}