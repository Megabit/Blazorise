#region Using directives
using System;
using System.Collections.Generic;
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

    private bool paneBordered = true;

    private DotNetObjectReference<DockLayout> dotNetObjectRef;

    private string activeAutoHidePaneName;

    private bool autoHideOutsideHandlerEnabled;

    private int nextNodeId;

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
    /// Forces rendered dock content to refresh without changing the docking state. Call this after
    /// mutating the <see cref="State"/> instance directly.
    /// </summary>
    /// <returns>A task that completes after the refresh has been scheduled.</returns>
    public Task Refresh()
        => InvokeAsync( () =>
        {
            context.NotifyChanged( new( DockLayoutChangeKind.Tree ) );
            StateHasChanged();
        } );

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
        activeAutoHidePaneName = null;

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
        DockPaneRestoreState restorePlacement = paneState.RestorePlacement;
        DockRailItemState railItem = stateManager.RemoveRailItem( CurrentState, paneState.Name );

        ClearActiveAutoHidePane( paneState.Name );

        paneState.Visible = true;
        paneState.AutoHide = false;
        paneState.RestorePlacement = null;

        if ( restorePlacement is null && railItem is not null )
            restorePlacement = CreateRestorePlacement( railItem );

        if ( restorePlacement is null || !RestorePaneToSourcePlacement( restorePlacement, paneState ) )
            AddPaneToLayout( paneState );

        await NotifyStateChanged();
    }

    /// <summary>
    /// Shows a pane by opening it, activating it, or expanding its auto-hide flyout.
    /// </summary>
    /// <param name="paneName">The name of the pane to show.</param>
    /// <returns>A task that completes after the pane has been shown.</returns>
    public async Task ShowPane( string paneName )
    {
        if ( !TryGetPane( paneName, out DockPane pane ) )
            return;

        DockPaneState paneState = stateManager.EnsurePaneState( CurrentState, pane );

        if ( !paneState.Visible )
        {
            await OpenPane( paneName );
            return;
        }

        if ( stateManager.FindRailItem( CurrentState, paneState.Name ) is not null || paneState.AutoHide )
        {
            await ExpandPaneAutoHide( pane );
            return;
        }

        await ActivatePane( paneName );
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
        => registry.RegisterContent( dockContent );

    internal Task RefreshPane( string paneName )
        => InvokeAsync( () => context.NotifyChanged( new( DockLayoutChangeKind.Pane, PaneName: paneName ) ) );

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

    internal IReadOnlyList<DockRailItemState> GetRailItems( DockPanePosition position )
        => stateManager.GetRailItems( CurrentState, registry.Panes, position );

    internal bool TryGetPane( string paneName, out DockPane pane )
        => registry.TryGetPane( paneName, out pane );

    internal DockPaneState GetPaneState( string paneName )
        => FindPaneState( paneName );

    internal DockNodeState GetNode( string nodeId )
        => treeQuery.GetNode( nodeId );

    internal string GetPaneCaption( string paneName )
        => TryGetPane( paneName, out DockPane pane ) ? pane.ResolvedCaption : paneName;

    internal DockPanePosition? GetDockNodePosition( DockNodeState node )
        => treeQuery.GetDockNodePosition( node );

    internal bool CanResizeDockNode( DockNodeState node )
        => treeQuery.CanResizeDockNode( node );

    internal bool CanResizeDockSplit( string nodeId )
    {
        DockNodeState node = GetNode( nodeId );

        return node?.Kind == DockNodeKind.Split
            && ( CanResizeDockNode( node.First ) || CanResizeDockNode( node.Second ) );
    }

    internal string GetDockNodeElementId( string nodeId )
        => string.IsNullOrWhiteSpace( nodeId ) ? null : $"{ElementId}-{nodeId}";

    internal ResizeHandleTargets GetDockResizeTargets( string nodeId )
    {
        DockNodeState node = GetNode( nodeId );

        if ( node?.Kind != DockNodeKind.Split || node.First is null || node.Second is null )
            return null;

        string resizeElementId = GetDockNodeElementId( node.Id );

        return new()
        {
            Start = CreateDockResizeTarget( node.First, node.Orientation, resizeElementId, "--dock-split-start-size" ),
            End = CreateDockResizeTarget( node.Second, node.Orientation, resizeElementId, "--dock-split-end-size" ),
        };
    }

    private ResizeHandleTarget CreateDockResizeTarget( DockNodeState node, DockSplitOrientation resizeOrientation, string resizeElementId, string resizeProperty )
    {
        DockPane pane = GetDockResizePane( node );

        return new()
        {
            ElementId = GetDockNodeElementId( node.Id ),
            ResizeElementId = resizeElementId,
            ResizeProperty = resizeProperty,
            MinSize = sizer.GetDockNodeMinimumSize( node, resizeOrientation ),
            MaxSize = pane?.MaxSize,
        };
    }

    private DockPane GetDockResizePane( DockNodeState node )
    {
        string paneName = node?.Kind switch
        {
            DockNodeKind.Pane => node.PaneName,
            DockNodeKind.Tabs => GetActiveTabPaneName( node ),
            _ => null,
        };

        return TryGetPane( paneName, out DockPane pane ) ? pane : null;
    }

    internal async Task ResizeDockSplit( string nodeId, ResizeHandleEventArgs eventArgs )
    {
        if ( eventArgs?.Canceled == true || eventArgs?.EndSize is null )
            return;

        DockNodeState node = GetNode( nodeId );
        double totalSize = eventArgs.StartSize + eventArgs.EndSize.Value;

        if ( node?.Kind != DockNodeKind.Split || totalSize <= 0 )
            return;

        bool startResizable = CanResizeDockNode( node.First );
        bool endResizable = CanResizeDockNode( node.Second );

        if ( startResizable != endResizable )
        {
            node.UseRatio = false;

            if ( startResizable )
                SetDockNodeSize( node.First, eventArgs.StartSize );
            else
                SetDockNodeSize( node.Second, eventArgs.EndSize.Value );
        }
        else
        {
            node.Ratio = Math.Clamp( eventArgs.StartSize / totalSize, 0.02d, 0.98d );
            node.UseRatio = true;
        }

        await NotifyStateChanged( new( DockLayoutChangeKind.Node, node.Id ) );
    }

    private void SetDockNodeSize( DockNodeState node, double size )
    {
        if ( node is null )
            return;

        string value = FormattableString.Invariant( $"{size:0.####}px" );

        node.Size = value;

        if ( node.Kind == DockNodeKind.Pane )
        {
            DockPaneState paneState = FindPaneState( node.PaneName );

            if ( paneState is not null )
                paneState.Size = value;
        }
    }

    internal DockPaneTabPosition GetDockNodeTabPosition( DockNodeState node, DockPanePosition position )
    {
        if ( node?.Panes is not null )
        {
            foreach ( string paneName in node.Panes )
            {
                if ( TryGetPane( paneName, out DockPane pane ) && pane.EffectiveTabPosition != DockPaneTabPosition.Default )
                    return pane.EffectiveTabPosition;
            }
        }

        return position == DockPanePosition.Center
            ? DockPaneTabPosition.Top
            : DockPaneTabPosition.Bottom;
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
            await NotifyStateChanged( new( DockLayoutChangeKind.Node, node.Id ) );
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
            await NotifyStateChanged( new( DockLayoutChangeKind.Node, tabsNode.Id ) );
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
            await PinPaneAutoHide( pane );
            return;
        }

        DockPaneRestoreState restorePlacement = CapturePaneRestorePlacement( paneState );

        paneState.AutoHide = true;
        paneState.Position = restorePlacement.SourcePosition;

        stateManager.AddRailItem( CurrentState, GetRailPosition( restorePlacement.SourcePosition ), CreateRailItem( paneState.Name, restorePlacement ) );

        ClearActiveAutoHidePane( paneState.Name );

        CurrentState.Root = treeMutator.RemovePaneNode( CurrentState.Root, paneState.Name );

        await NotifyStateChanged();
    }

    internal Task ExpandPaneAutoHide( DockPane pane )
    {
        if ( pane is null )
            return Task.CompletedTask;

        DockPaneState paneState = stateManager.EnsurePaneState( CurrentState, pane );
        DockRailItemState railItem = stateManager.FindRailItem( CurrentState, paneState.Name );

        if ( railItem is null || !paneState.Visible )
            return Task.CompletedTask;

        paneState.AutoHide = true;
        paneState.Position = railItem.SourcePosition;
        activeAutoHidePaneName = paneState.Name;

        StateHasChanged();

        return Task.CompletedTask;
    }

    internal Task CollapsePaneAutoHide()
    {
        if ( activeAutoHidePaneName is null )
            return Task.CompletedTask;

        activeAutoHidePaneName = null;
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Collapses the active auto-hide flyout after an outside interaction.
    /// </summary>
    /// <returns>A task that completes after the flyout has been collapsed.</returns>
    [JSInvokable]
    public Task NotifyDockAutoHideOutsidePointerDown()
        => CollapsePaneAutoHide();

    internal async Task PinPaneAutoHide( DockPane pane )
    {
        if ( pane is null )
            return;

        DockPaneState paneState = stateManager.EnsurePaneState( CurrentState, pane );
        DockRailItemState railItem = stateManager.FindRailItem( CurrentState, paneState.Name );

        if ( railItem is not null )
        {
            stateManager.RemoveRailItem( CurrentState, railItem.PaneName );
            ClearActiveAutoHidePane( railItem.PaneName );

            DockPaneRestoreState restorePlacement = CreateRestorePlacement( railItem );

            paneState.AutoHide = false;
            paneState.Position = restorePlacement.SourcePosition;
            paneState.RestorePlacement = null;

            RestorePaneToSourcePlacement( restorePlacement, paneState );

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

        ClearActiveAutoHidePane( paneState.Name );

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
        DockRailItemState railItem = stateManager.FindRailItem( CurrentState, paneState.Name );

        if ( !paneState.Visible )
            return;

        paneState.RestorePlacement = railItem is not null
            ? CreateRestorePlacement( railItem )
            : CapturePaneRestorePlacement( paneState );
        paneState.Visible = false;
        paneState.AutoHide = false;

        stateManager.RemoveRailItem( CurrentState, paneState.Name );
        ClearActiveAutoHidePane( paneState.Name );
        CurrentState.Root = treeMutator.RemovePaneNode( CurrentState.Root, paneState.Name );

        await NotifyStateChanged();
    }

    internal bool IsPaneTabCloseButtonVisible( string paneName, DockPanePosition position )
        => position == DockPanePosition.Center
            && TryGetPane( paneName, out DockPane pane )
            && pane.Closable
            && pane.EffectiveShowTabCloseButton;

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

        await DocumentObserver.EnsureInitializedAsync();

        await JSModule.BeginDrag(
            DotNetObjectRef,
            ElementRef,
            pane.ResolvedName,
            eventArgs.PointerId,
            eventArgs.ClientX,
            eventArgs.ClientY,
            dragGroup );
    }

    /// <summary>
    /// Updates the currently highlighted dock drop zone while a pane is dragged.
    /// </summary>
    /// <param name="paneName">The dragged pane name.</param>
    /// <param name="zone">The currently hovered drop zone.</param>
    /// <param name="compassZoneKey">The exact compass zone currently under the pointer.</param>
    /// <param name="compassX">The horizontal compass location relative to the layout.</param>
    /// <param name="compassY">The vertical compass location relative to the layout.</param>
    /// <returns>A completed task.</returns>
    [JSInvokable]
    public Task NotifyDockPaneDrag( string paneName, string zone, string compassZoneKey, double compassX, double compassY )
    {
        bool guidesVisible = DockGuidesVisible;

        dragState.Update( paneName, DockLayoutTreeMutator.ToDockZone( zone ), compassZoneKey, compassX, compassY );

        if ( guidesVisible != DockGuidesVisible )
            StateHasChanged();
        else if ( DockGuidesVisible )
            context.NotifyChanged( new( DockLayoutChangeKind.Compass ) );

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

            return;
        }

        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( JSModule is not null )
            {
                if ( dotNetObjectRef is not null && autoHideOutsideHandlerEnabled )
                {
                    await JSModule.SetAutoHideOutsideHandler( dotNetObjectRef, ElementRef, false );
                    autoHideOutsideHandlerEnabled = false;
                }

                await JSModule.Cancel();
            }

            if ( dotNetObjectRef is not null )
            {
                dotNetObjectRef.Dispose();
                dotNetObjectRef = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( firstRender && CurrentState.Root is null && registry.RootCollector.Nodes.Count > 0 )
        {
            EnsureCurrentStateInitialized();
            await NotifyStateChanged();
        }

        await UpdateAutoHideOutsideHandler();
    }

    private async Task UpdateAutoHideOutsideHandler()
    {
        if ( JSModule is null )
            return;

        bool enabled = ActiveAutoHidePane is not null;

        if ( autoHideOutsideHandlerEnabled == enabled )
            return;

        autoHideOutsideHandlerEnabled = enabled;

        if ( enabled )
        {
            await DocumentObserver.EnsureInitializedAsync();
            await JSModule.SetAutoHideOutsideHandler( DotNetObjectRef, ElementRef, true );
        }
        else if ( dotNetObjectRef is not null )
        {
            await JSModule.SetAutoHideOutsideHandler( dotNetObjectRef, ElementRef, false );
        }
    }

    internal DockPaneState FindPaneState( string paneName )
        => stateManager.FindPaneState( CurrentState, paneName );

    private DockPaneRestoreState CapturePaneRestorePlacement( DockPaneState paneState )
    {
        DockNodeState tabsNode = treeQuery.FindTabsNode( paneState.Name );
        DockPaneRestoreReference relatedTabRestore = tabsNode is null
            ? FindRestoreReferenceBySourceTabPaneName( paneState.Name )
            : null;
        DockPaneRestoreReference relatedSplitRestore = tabsNode is null && relatedTabRestore is null
            ? FindRestoreReferenceBySourceTargetPaneName( paneState.Name )
            : null;
        DockPaneRestoreReference relatedRestore = relatedTabRestore ?? relatedSplitRestore;
        bool relatedToHiddenSplitTarget = relatedSplitRestore is not null;
        DockLayoutTreeQuery.DockNodePlacement sourcePlacement = treeQuery.FindPanePlacement( paneState.Name );
        DockLayoutTreeQuery.DockNodePlacement sourceGroupPlacement = sourcePlacement?.Parent is null
            ? null
            : treeQuery.FindDockNodePlacement( sourcePlacement.Parent );

        return new()
        {
            SourceGroupId = tabsNode?.Id ?? relatedRestore?.RestorePlacement.SourceGroupId ?? sourcePlacement?.Parent?.Id ?? paneState.Name,
            SourceTabPaneName = tabsNode?.Panes.FirstOrDefault( x => x != paneState.Name ) ?? relatedTabRestore?.PaneName,
            SourcePosition = tabsNode is null
                ? relatedRestore?.RestorePlacement.SourcePosition ?? treeQuery.GetPanePosition( paneState.Name ) ?? paneState.Position
                : treeQuery.GetDockNodePosition( tabsNode ) ?? paneState.Position,
            SourceSize = tabsNode?.Size ?? relatedRestore?.RestorePlacement.SourceSize ?? paneState.Size,
            SourceSplitRatio = relatedRestore?.RestorePlacement.SourceSplitRatio ?? sourcePlacement?.Parent?.Ratio,
            SourceSplitUseRatio = relatedRestore?.RestorePlacement.SourceSplitUseRatio ?? sourcePlacement?.Parent?.UseRatio,
            SourceGroupTargetPaneName = relatedRestore?.RestorePlacement.SourceGroupTargetPaneName ?? treeQuery.GetFirstDockNodePaneName( sourceGroupPlacement?.Sibling ),
            SourceGroupTargetNodeId = relatedRestore?.RestorePlacement.SourceGroupTargetNodeId ?? sourceGroupPlacement?.Sibling?.Id,
            SourceGroupZone = relatedRestore?.RestorePlacement.SourceGroupZone ?? sourceGroupPlacement?.Zone,
            SourceGroupSplitRatio = relatedRestore?.RestorePlacement.SourceGroupSplitRatio ?? sourceGroupPlacement?.Parent?.Ratio,
            SourceGroupSplitUseRatio = relatedRestore?.RestorePlacement.SourceGroupSplitUseRatio ?? sourceGroupPlacement?.Parent?.UseRatio,
            SourceTargetPaneName = relatedToHiddenSplitTarget
                ? relatedRestore.PaneName
                : relatedRestore?.RestorePlacement.SourceTargetPaneName ?? treeQuery.GetFirstDockNodePaneName( sourcePlacement?.Sibling ),
            SourceTargetNodeId = relatedToHiddenSplitTarget
                ? null
                : relatedRestore?.RestorePlacement.SourceTargetNodeId ?? sourcePlacement?.Sibling?.Id,
            SourceZone = relatedToHiddenSplitTarget
                ? InvertDockZone( relatedRestore.RestorePlacement.SourceZone )
                : relatedRestore?.RestorePlacement.SourceZone ?? sourcePlacement?.Zone,
            SourceIndex = tabsNode?.Panes.IndexOf( paneState.Name ) ?? GetRelatedRestoreSourceIndex( relatedRestore ),
        };
    }

    private bool RestorePaneToSourcePlacement( DockPaneRestoreState restorePlacement, DockPaneState paneState )
    {
        if ( restorePlacement is null || paneState is null )
            return false;

        if ( !string.IsNullOrWhiteSpace( restorePlacement.SourceSize ) )
            paneState.Size = restorePlacement.SourceSize;

        if ( RestorePaneToSourceTabs( restorePlacement, paneState.Name ) )
            return true;

        DockNodeState restoredNode = new()
        {
            Kind = DockNodeKind.Pane,
            PaneName = paneState.Name,
            Size = restorePlacement.SourceSize,
        };

        bool restoredToTarget = restorePlacement.SourceZone is not null
            && treeMutator.AddNodeToTarget(
                CurrentState,
                restoredNode,
                restorePlacement.SourceTargetPaneName,
                restorePlacement.SourceTargetNodeId,
                restorePlacement.SourceZone.Value,
                restorePlacement.SourceSplitRatio,
                restorePlacement.SourceSplitUseRatio );

        if ( !restoredToTarget )
        {
            restoredToTarget = restorePlacement.SourceGroupZone is not null
                && treeMutator.AddNodeToTarget(
                    CurrentState,
                    restoredNode,
                    restorePlacement.SourceGroupTargetPaneName,
                    restorePlacement.SourceGroupTargetNodeId,
                    restorePlacement.SourceGroupZone.Value,
                    restorePlacement.SourceGroupSplitRatio,
                    restorePlacement.SourceGroupSplitUseRatio );
        }

        if ( !restoredToTarget )
            treeMutator.AddNodeToPosition( CurrentState, restoredNode, restorePlacement.SourcePosition );

        if ( !string.IsNullOrWhiteSpace( restorePlacement.SourceTabPaneName ) )
        {
            stateManager.UpdateRailGroupSourceTabPane( CurrentState, restorePlacement.SourceGroupId, paneState.Name );
            UpdateClosedRestoreGroupSourceTabPane( restorePlacement.SourceGroupId, paneState.Name );
        }

        return true;
    }

    private bool RestorePaneToSourceTabs( DockPaneRestoreState restorePlacement, string paneName )
    {
        if ( restorePlacement is null || string.IsNullOrWhiteSpace( paneName ) )
            return false;

        DockNodeState sourceTabsNode = DockLayoutTreeQuery.FindNodeById( CurrentState.Root, restorePlacement.SourceGroupId );

        if ( CanRestorePaneToSourceTabsNode( restorePlacement, sourceTabsNode ) )
        {
            AddPaneToTabsNode( sourceTabsNode, paneName, restorePlacement.SourceIndex );
            ApplyRestoredTabsNodeSize( sourceTabsNode, restorePlacement.SourceSize );
            return true;
        }

        if ( CanRestorePaneToSourceTabPane( restorePlacement ) )
        {
            treeMutator.MovePaneToZone( CurrentState, paneName, restorePlacement.SourceTabPaneName, null, DockZone.Center, true );
            sourceTabsNode = treeQuery.FindTabsNode( paneName );

            if ( sourceTabsNode?.Kind == DockNodeKind.Tabs )
            {
                AddPaneToTabsNode( sourceTabsNode, paneName, restorePlacement.SourceIndex );
                ApplyRestoredTabsNodeSize( sourceTabsNode, restorePlacement.SourceSize );
                return true;
            }

            CurrentState.Root = treeMutator.RemovePaneNode( CurrentState.Root, paneName );
        }

        return false;
    }

    private bool CanRestorePaneToSourceTabsNode( DockPaneRestoreState restorePlacement, DockNodeState sourceTabsNode )
        => sourceTabsNode?.Kind == DockNodeKind.Tabs
            && treeQuery.GetDockNodePosition( sourceTabsNode ) == restorePlacement.SourcePosition;

    private bool CanRestorePaneToSourceTabPane( DockPaneRestoreState restorePlacement )
    {
        if ( string.IsNullOrWhiteSpace( restorePlacement?.SourceTabPaneName )
             || !DockLayoutTreeQuery.ContainsPane( CurrentState.Root, restorePlacement.SourceTabPaneName ) )
            return false;

        if ( treeQuery.GetPanePosition( restorePlacement.SourceTabPaneName ) != restorePlacement.SourcePosition )
            return false;

        if ( restorePlacement.SourceZone is null || string.IsNullOrWhiteSpace( restorePlacement.SourceTargetPaneName ) && string.IsNullOrWhiteSpace( restorePlacement.SourceTargetNodeId ) )
            return true;

        bool sourceTargetExists = !string.IsNullOrWhiteSpace( restorePlacement.SourceTargetNodeId ) && DockLayoutTreeQuery.FindNodeById( CurrentState.Root, restorePlacement.SourceTargetNodeId ) is not null
            || !string.IsNullOrWhiteSpace( restorePlacement.SourceTargetPaneName ) && DockLayoutTreeQuery.ContainsPane( CurrentState.Root, restorePlacement.SourceTargetPaneName );

        if ( !sourceTargetExists )
            return true;

        DockLayoutTreeQuery.DockNodePlacement sourcePlacement = treeQuery.FindPanePlacement( restorePlacement.SourceTabPaneName );

        if ( sourcePlacement?.Zone != restorePlacement.SourceZone )
            return false;

        return !string.IsNullOrWhiteSpace( restorePlacement.SourceTargetNodeId ) && sourcePlacement.Sibling?.Id == restorePlacement.SourceTargetNodeId
            || !string.IsNullOrWhiteSpace( restorePlacement.SourceTargetPaneName ) && DockLayoutTreeQuery.ContainsPane( sourcePlacement.Sibling, restorePlacement.SourceTargetPaneName );
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

    private DockPaneRestoreReference FindRestoreReferenceBySourceTabPaneName( string paneName )
    {
        DockRailItemState railItem = stateManager.FindRailItemBySourceTabPaneName( CurrentState, paneName );

        if ( railItem is not null )
            return new( railItem.PaneName, CreateRestorePlacement( railItem ) );

        DockPaneState paneState = CurrentState.Panes.FirstOrDefault( x => x.Visible == false && x.RestorePlacement?.SourceTabPaneName == paneName );

        return paneState is null ? null : new( paneState.Name, paneState.RestorePlacement );
    }

    private DockPaneRestoreReference FindRestoreReferenceBySourceTargetPaneName( string paneName )
    {
        DockRailItemState railItem = stateManager.FindRailItemBySourceTargetPaneName( CurrentState, paneName );

        if ( railItem is not null )
            return new( railItem.PaneName, CreateRestorePlacement( railItem ) );

        DockPaneState paneState = CurrentState.Panes.FirstOrDefault( x => x.Visible == false && x.RestorePlacement?.SourceTargetPaneName == paneName );

        return paneState is null ? null : new( paneState.Name, paneState.RestorePlacement );
    }

    private void UpdateClosedRestoreGroupSourceTabPane( string sourceGroupId, string sourceTabPaneName )
    {
        if ( string.IsNullOrWhiteSpace( sourceGroupId ) || string.IsNullOrWhiteSpace( sourceTabPaneName ) )
            return;

        foreach ( DockPaneState paneState in CurrentState.Panes.Where( x => x.Visible == false && x.RestorePlacement?.SourceGroupId == sourceGroupId ) )
            paneState.RestorePlacement.SourceTabPaneName = sourceTabPaneName;
    }

    private static DockRailItemState CreateRailItem( string paneName, DockPaneRestoreState restorePlacement )
    {
        if ( restorePlacement is null )
            return null;

        return new()
        {
            PaneName = paneName,
            SourceGroupId = restorePlacement.SourceGroupId,
            SourceTabPaneName = restorePlacement.SourceTabPaneName,
            SourcePosition = restorePlacement.SourcePosition,
            SourceSize = restorePlacement.SourceSize,
            SourceSplitRatio = restorePlacement.SourceSplitRatio,
            SourceSplitUseRatio = restorePlacement.SourceSplitUseRatio,
            SourceGroupTargetPaneName = restorePlacement.SourceGroupTargetPaneName,
            SourceGroupTargetNodeId = restorePlacement.SourceGroupTargetNodeId,
            SourceGroupZone = restorePlacement.SourceGroupZone,
            SourceGroupSplitRatio = restorePlacement.SourceGroupSplitRatio,
            SourceGroupSplitUseRatio = restorePlacement.SourceGroupSplitUseRatio,
            SourceTargetPaneName = restorePlacement.SourceTargetPaneName,
            SourceTargetNodeId = restorePlacement.SourceTargetNodeId,
            SourceZone = restorePlacement.SourceZone,
            SourceIndex = restorePlacement.SourceIndex,
        };
    }

    private static DockPaneRestoreState CreateRestorePlacement( DockRailItemState railItem )
    {
        if ( railItem is null )
            return null;

        return new()
        {
            SourceGroupId = railItem.SourceGroupId,
            SourceTabPaneName = railItem.SourceTabPaneName,
            SourcePosition = railItem.SourcePosition,
            SourceSize = railItem.SourceSize,
            SourceSplitRatio = railItem.SourceSplitRatio,
            SourceSplitUseRatio = railItem.SourceSplitUseRatio,
            SourceGroupTargetPaneName = railItem.SourceGroupTargetPaneName,
            SourceGroupTargetNodeId = railItem.SourceGroupTargetNodeId,
            SourceGroupZone = railItem.SourceGroupZone,
            SourceGroupSplitRatio = railItem.SourceGroupSplitRatio,
            SourceGroupSplitUseRatio = railItem.SourceGroupSplitUseRatio,
            SourceTargetPaneName = railItem.SourceTargetPaneName,
            SourceTargetNodeId = railItem.SourceTargetNodeId,
            SourceZone = railItem.SourceZone,
            SourceIndex = railItem.SourceIndex,
        };
    }

    private static int GetRelatedRestoreSourceIndex( DockPaneRestoreReference relatedRestore )
    {
        if ( relatedRestore is null )
            return 0;

        return relatedRestore.RestorePlacement.SourceIndex == 0 ? 1 : 0;
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

    private void ClearActiveAutoHidePane( string paneName )
    {
        if ( activeAutoHidePaneName == paneName )
            activeAutoHidePaneName = null;
    }

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

    private sealed record DockPaneRestoreReference( string PaneName, DockPaneRestoreState RestorePlacement );

    private async Task NotifyStateChanged()
    {
        await CommitStateChanged();

        context.NotifyChanged( new( DockLayoutChangeKind.Tree ) );
        StateHasChanged();
    }

    private async Task NotifyStateChanged( DockLayoutChange change )
    {
        await CommitStateChanged();

        context.NotifyChanged( change );
    }

    private async Task CommitStateChanged()
    {
        NormalizeCurrentState();

        await StateChanged.InvokeAsync( CurrentState );
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

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    internal bool DockGuidesVisible => dragState.Visible;

    internal static IReadOnlyList<DockCompassZoneInfo> DockCompassZones => dockCompassZones;

    internal string ActiveDockCompassZoneKey => dragState.CompassZoneKey;

    internal DockZone? ActiveDockZone => dragState.Zone;

    internal double DockCompassX => dragState.CompassX;

    internal double DockCompassY => dragState.CompassY;

    private DockLayoutState CurrentState => state ??= new();

    internal DockNodeCollector RootCollector => registry.RootCollector;

    internal DockContent Content => registry.Content;

    internal DockLayoutContext Context => context;

    internal DockPane ActiveAutoHidePane
        => activeAutoHidePaneName is not null
            && stateManager.FindRailItem( CurrentState, activeAutoHidePaneName ) is not null
            && TryGetPane( activeAutoHidePaneName, out DockPane pane )
                ? pane
                : null;

    private DotNetObjectReference<DockLayout> DotNetObjectRef => dotNetObjectRef ??= DotNetObjectReference.Create( this );

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
    /// Gets the shared document observer.
    /// </summary>
    [Inject] protected IDocumentObserver DocumentObserver { get; set; }

    /// <summary>
    /// Defines whether non-document panes should render a visible border.
    /// </summary>
    [Parameter]
    public bool PaneBordered
    {
        get => paneBordered;
        set
        {
            if ( paneBordered == value )
                return;

            paneBordered = value;
            context.NotifyChanged( new( DockLayoutChangeKind.Tree ) );
        }
    }

    /// <summary>
    /// Defines the mutable state used for docking, resizing, active tabs, and pane visibility. The same state can be saved with <see cref="GetState"/> and restored with <see cref="LoadState"/>.
    /// In-place changes to the assigned instance are not detected; apply them with <see cref="LoadState"/> or follow them with <see cref="Refresh"/>.
    /// </summary>
    [Parameter]
    public DockLayoutState State
    {
        get => state;
        set
        {
            if ( ReferenceEquals( state, value ) )
                return;

            state = value;
            context.NotifyChanged( new( DockLayoutChangeKind.Tree ) );
        }
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