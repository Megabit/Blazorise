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

    private readonly Dictionary<string, DockPane> panes = new();

    private DockNodeCollector rootCollector = new();

    private DockLayoutState state;

    private DockContent content;

    private DockLayoutContext context;

    private string draggingPaneName;

    private DockZone? activeDockZone;

    private string activeDockCompassZoneKey;

    private bool draggingPaneGroup;

    private double dockCompassX;

    private double dockCompassY;

    private DotNetObjectReference<DockLayout> dotNetObjectRef;

    private int nextNodeId;

    private int renderVersion;

    private int dockGuidesVersion;

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

    private const string DefaultPaneSize = "16rem";

    private const string AutoHidePaneSize = "2rem";

    private const string CollapsedPaneSize = "2.5rem";

    private const string FlexibleFillTrack = "minmax(0,1fr)";

    #endregion

    #region Constructors

    /// <summary>
    /// Default <see cref="DockLayout"/> constructor.
    /// </summary>
    public DockLayout()
    {
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
        builder.Append( $"left:{dockCompassX}px" );
        builder.Append( $"top:{dockCompassY}px" );
    }

    /// <inheritdoc/>
    protected internal override void DirtyStyles()
    {
        DockCompassStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    internal void RegisterPane( DockPane pane )
    {
        string paneName = pane.ResolvedName;

        if ( string.IsNullOrWhiteSpace( paneName ) )
            return;

        panes[paneName] = pane;

        EnsurePaneState( pane );
    }

    internal void RegisterContent( DockContent dockContent )
    {
        content = dockContent;
    }

    internal Task NotifyDefinitionChanged()
        => InvokeAsync( async () =>
        {
            if ( CurrentState.Root is null && rootCollector.Nodes.Count > 0 )
                CurrentState.Root = BuildInitialRoot();

            await NotifyStateChanged();
        } );

    internal void UnregisterPane( DockPane pane )
    {
        string paneName = pane.ResolvedName;

        if ( string.IsNullOrWhiteSpace( paneName ) )
            return;

        panes.Remove( paneName );
    }

    internal DockPaneState GetPaneState( DockPane pane )
        => EnsurePaneState( pane );

    internal DockPanePosition GetPanePosition( DockPane pane )
        => FindPanePosition( CurrentState.Root, pane.ResolvedName )
            ?? FindPaneState( pane.ResolvedName )?.Position
            ?? GetInitialPanePosition( pane );

    internal IReadOnlyList<DockPaneState> GetPaneStates( DockPanePosition position )
        => CurrentState.Panes
            .Where( x => x.Visible && x.Position == position )
            .OrderBy( x => x.Order )
            .ToArray();

    internal bool HasPaneTabs( DockPanePosition position )
        => GetPaneStates( position ).Count > 1;

    internal bool TryGetPane( string paneName, out DockPane pane )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) )
        {
            pane = null;
            return false;
        }

        return panes.TryGetValue( paneName, out pane );
    }

    internal DockPaneState GetPaneState( string paneName )
        => FindPaneState( paneName );

    internal DockNodeState GetNode( string nodeId )
        => string.IsNullOrWhiteSpace( nodeId ) ? null : FindNodeById( CurrentState.Root, nodeId );

    internal bool IsPaneActive( DockPane pane )
    {
        DockNodeState tabsNode = FindTabsNode( CurrentState.Root, pane.ResolvedName );

        return tabsNode is null || GetActiveTabPaneName( tabsNode ) == pane.ResolvedName;
    }

    internal string GetPaneCaption( string paneName )
        => panes.TryGetValue( paneName, out DockPane pane ) ? pane.ResolvedCaption : paneName;

    internal DockPanePosition? GetDockNodePosition( DockNodeState node )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Pane )
            return GetPanePosition( node.PaneName );

        if ( node.Kind == DockNodeKind.Tabs )
        {
            foreach ( string paneName in node.Panes )
            {
                DockPanePosition? position = GetPanePosition( paneName );

                if ( position == DockPanePosition.Center )
                    return DockPanePosition.Center;
            }

            return GetPanePosition( GetActiveTabPaneName( node ) );
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            DockPanePosition? firstPosition = GetDockNodePosition( node.First );
            DockPanePosition? secondPosition = GetDockNodePosition( node.Second );

            if ( firstPosition == DockPanePosition.Center || secondPosition == DockPanePosition.Center )
                return DockPanePosition.Center;

            if ( firstPosition is not null && firstPosition == secondPosition )
                return firstPosition;

            return null;
        }

        return null;
    }

    internal DockPaneTabsPlacement GetDockNodeTabsPlacement( DockNodeState node, DockPanePosition position )
    {
        if ( node?.Panes is not null )
        {
            foreach ( string paneName in node.Panes )
            {
                if ( panes.TryGetValue( paneName, out DockPane pane ) && pane.EffectiveTabsPlacement != DockPaneTabsPlacement.Default )
                    return pane.EffectiveTabsPlacement;
            }
        }

        return position == DockPanePosition.Center
            ? DockPaneTabsPlacement.Top
            : DockPaneTabsPlacement.Bottom;
    }

    internal string GetDockSplitStyle( DockNodeState node )
    {
        if ( node is null || node.Kind != DockNodeKind.Split )
            return null;

        string firstFixedTrack = node.UseRatio ? null : GetDockNodeTrackSize( node.First, node.Orientation );
        string secondFixedTrack = node.UseRatio ? null : GetDockNodeTrackSize( node.Second, node.Orientation );
        string firstTrack = firstFixedTrack ?? ( secondFixedTrack is not null ? FlexibleFillTrack : GetFlexibleSplitTrack( node.Ratio ) );
        string secondTrack = secondFixedTrack ?? ( firstFixedTrack is not null ? FlexibleFillTrack : GetFlexibleSplitTrack( 1d - node.Ratio ) );

        return node.Orientation == DockSplitOrientation.Vertical
            ? $"grid-template-rows:{firstTrack} {secondTrack};"
            : $"grid-template-columns:{firstTrack} {secondTrack};";
    }

    internal bool IsPaneAutoHidden( string paneName )
        => FindPaneState( paneName )?.AutoHide == true;

    internal bool IsTabGroupAutoHidden( DockNodeState node )
    {
        if ( node?.Kind != DockNodeKind.Tabs )
            return false;

        string activePaneName = GetActiveTabPaneName( node );

        return IsPaneAutoHidden( activePaneName );
    }

    internal bool IsDockPaneBordered( DockPanePosition position )
        => PaneBordered && position != DockPanePosition.Center;

    internal string GetActiveTabPaneName( DockNodeState node )
    {
        if ( node is null || node.Kind != DockNodeKind.Tabs )
            return null;

        if ( !string.IsNullOrWhiteSpace( node.ActivePane ) && node.Panes.Contains( node.ActivePane ) )
            return node.ActivePane;

        return node.Panes.Count > 0 ? node.Panes[0] : null;
    }

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
        DockNodeState tabsNode = FindTabsNode( CurrentState.Root, paneName );

        if ( tabsNode is not null && tabsNode.Panes.Contains( paneName ) && tabsNode.ActivePane != paneName )
        {
            tabsNode.ActivePane = paneName;
            await NotifyStateChanged();
        }
    }

    internal async Task TogglePaneAutoHide( DockPane pane )
    {
        DockPaneState paneState = EnsurePaneState( pane );
        DockNodeState tabsNode = FindTabsNode( CurrentState.Root, paneState.Name );

        if ( tabsNode is not null )
        {
            bool autoHide = !paneState.AutoHide;

            foreach ( string paneName in tabsNode.Panes )
                SetPaneAutoHide( paneName, autoHide );
        }
        else
        {
            paneState.AutoHide = !paneState.AutoHide;
        }

        await NotifyStateChanged();
    }

    internal async Task OpenPaneAutoHide( DockPane pane )
    {
        DockPaneState paneState = EnsurePaneState( pane );
        DockNodeState tabsNode = FindTabsNode( CurrentState.Root, paneState.Name );

        if ( tabsNode is not null )
        {
            foreach ( string paneName in tabsNode.Panes )
                SetPaneAutoHide( paneName, false );

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

    internal async Task ClosePane( string paneName )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) || !panes.TryGetValue( paneName, out DockPane pane ) || !pane.Closable )
            return;

        DockPaneState paneState = EnsurePaneState( pane );

        paneState.Visible = false;
        CurrentState.Root = RemovePaneNode( CurrentState.Root, paneState.Name );

        await NotifyStateChanged();
    }

    internal bool IsPaneTabCloseButtonVisible( string paneName, DockPanePosition position )
        => position == DockPanePosition.Center
            && !string.IsNullOrWhiteSpace( paneName )
            && panes.TryGetValue( paneName, out DockPane pane )
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
        if ( string.IsNullOrWhiteSpace( paneName ) || !panes.TryGetValue( paneName, out DockPane pane ) )
            return Task.CompletedTask;

        return BeginPaneDrag( pane, eventArgs, false );
    }

    internal async Task BeginPaneDrag( DockPane pane, PointerEventArgs eventArgs, bool dragGroup )
    {
        if ( pane?.Movable != true )
            return;

        draggingPaneGroup = dragGroup;

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
        DockNodeState resizedNode = FindNodeById( CurrentState.Root, nodeId );

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
            else if ( FindTabsNode( CurrentState.Root, paneName ) is DockNodeState tabsNode )
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
        activeDockZone = ToDockZone( zone );
        activeDockCompassZoneKey = compassZoneKey;
        dockCompassX = compassX;
        dockCompassY = compassY;
        draggingPaneName = paneName;

        dockGuidesVersion++;
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
        DockZone? targetZone = ToDockZone( zone );
        DockPaneState paneState = FindPaneState( paneName );
        bool moveGroup = draggingPaneGroup;

        draggingPaneName = null;
        activeDockZone = null;
        activeDockCompassZoneKey = null;
        draggingPaneGroup = false;

        if ( paneState is not null && targetZone is not null )
        {
            DockNodeState sourceTabsNode = moveGroup ? FindTabsNode( CurrentState.Root, paneName ) : null;
            IReadOnlyList<string> movedPaneNames = sourceTabsNode?.Panes.Count > 1
                ? sourceTabsNode.Panes.ToArray()
                : new[] { paneName };
            DockPaneState targetState = FindPaneState( targetName );
            bool targetExists = !string.IsNullOrWhiteSpace( targetName ) && !movedPaneNames.Contains( targetName ) && ContainsPane( CurrentState.Root, targetName );
            bool mergeWithTargetTabs = targetExists && targetZone.Value == DockZone.Center;
            DockPanePosition? targetPosition = GetDropPanePosition( targetState, targetZone.Value, mergeWithTargetTabs );

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
                MoveNodeToZone( sourceTabsNode, movedPaneNames, targetName, targetNodeId, targetZone.Value, mergeWithTargetTabs, targetExists );
            else
                MovePaneToZone( paneName, targetName, targetNodeId, targetZone.Value, mergeWithTargetTabs );

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

        if ( CurrentState.Root is null && rootCollector.Nodes.Count > 0 )
        {
            CurrentState.Root = BuildInitialRoot();
            NormalizeCurrentState();
            await NotifyStateChanged();
        }
    }

    private DockPaneState EnsurePaneState( DockPane pane )
    {
        DockPaneState paneState = FindPaneState( pane.ResolvedName );

        if ( paneState is not null )
            return paneState;

        paneState = new()
        {
            Name = pane.ResolvedName,
            Position = GetInitialPanePosition( pane ),
            Size = pane.Size,
            Collapsed = pane.Collapsed,
            AutoHide = pane.AutoHide,
            Visible = pane.Visible,
            Order = CurrentState.Panes.Count,
        };

        CurrentState.Panes.Add( paneState );

        return paneState;
    }

    internal DockPaneState FindPaneState( string paneName )
        => CurrentState.Panes.FirstOrDefault( x => x.Name == paneName );

    private void SetPaneAutoHide( string paneName, bool autoHide )
    {
        DockPaneState paneState = FindPaneState( paneName );

        if ( paneState is not null )
            paneState.AutoHide = autoHide;
    }

    private static DockNodeState FindTabsNode( DockNodeState node, string paneName )
    {
        if ( node is null || string.IsNullOrWhiteSpace( paneName ) )
            return null;

        if ( node.Kind == DockNodeKind.Tabs && node.Panes.Contains( paneName ) )
            return node;

        if ( node.Kind == DockNodeKind.Split )
            return FindTabsNode( node.First, paneName ) ?? FindTabsNode( node.Second, paneName );

        return null;
    }

    private string GetDockNodeTrackSize( DockNodeState node, DockSplitOrientation orientation )
    {
        DockPanePosition? position = GetDockNodePosition( node );

        if ( position is null || !IsPanePositionCompatibleWithOrientation( position.Value, orientation ) )
            return null;

        if ( !string.IsNullOrWhiteSpace( node?.Size ) )
            return node.Size;

        DockPane pane = GetDockNodePane( node );

        if ( pane is null )
            return null;

        DockPaneState paneState = FindPaneState( pane.ResolvedName );

        if ( paneState?.Visible == false )
            return null;

        if ( paneState?.AutoHide == true )
            return AutoHidePaneSize;

        if ( paneState?.Collapsed == true )
            return CollapsedPaneSize;

        if ( node.Kind == DockNodeKind.Tabs && !string.IsNullOrWhiteSpace( node.Size ) )
            return node.Size;

        return paneState?.Size ?? pane.Size ?? GetDefaultDockPaneSize( position.Value );
    }

    private static string GetDefaultDockPaneSize( DockPanePosition position )
        => position == DockPanePosition.Top || position == DockPanePosition.Bottom
            ? "auto"
            : DefaultPaneSize;

    private DockPane GetDockNodePane( DockNodeState node )
    {
        if ( node is null )
            return null;

        string paneName = node.Kind switch
        {
            DockNodeKind.Pane => node.PaneName,
            DockNodeKind.Tabs => GetActiveTabPaneName( node ),
            DockNodeKind.Split => GetFirstDockNodePaneName( node ),
            _ => null,
        };

        return !string.IsNullOrWhiteSpace( paneName ) && panes.TryGetValue( paneName, out DockPane pane )
            ? pane
            : null;
    }

    private string GetFirstDockNodePaneName( DockNodeState node )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Pane )
            return node.PaneName;

        if ( node.Kind == DockNodeKind.Tabs )
            return GetActiveTabPaneName( node ) ?? node.Panes.FirstOrDefault();

        if ( node.Kind == DockNodeKind.Split )
            return GetFirstDockNodePaneName( node.First ) ?? GetFirstDockNodePaneName( node.Second );

        return null;
    }

    private static DockNodeState FindNodeById( DockNodeState node, string nodeId )
    {
        if ( node is null || string.IsNullOrWhiteSpace( nodeId ) )
            return null;

        if ( node.Id == nodeId )
            return node;

        if ( node.Kind == DockNodeKind.Split )
            return FindNodeById( node.First, nodeId ) ?? FindNodeById( node.Second, nodeId );

        return null;
    }

    private DockPanePosition? GetPanePosition( string paneName )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) )
            return null;

        if ( panes.TryGetValue( paneName, out DockPane pane ) )
            return GetPanePosition( pane );

        return FindPaneState( paneName )?.Position;
    }

    private static bool IsPanePositionCompatibleWithOrientation( DockPanePosition position, DockSplitOrientation orientation )
        => orientation == DockSplitOrientation.Horizontal
            ? position is DockPanePosition.Left or DockPanePosition.Right
            : position is DockPanePosition.Top or DockPanePosition.Bottom;

    private static DockPanePosition GetInitialPanePosition( DockPane pane )
        => pane.DockRole == DockRole.Document ? DockPanePosition.Center : pane.Dock;

    private static string GetFlexibleSplitTrack( double ratio )
    {
        double trackRatio = ratio > 0 && ratio < 1 ? ratio : 0.5;

        return $"minmax(0,{trackRatio.ToString( CultureInfo.InvariantCulture )}fr)";
    }

    private DockNodeState BuildInitialRoot()
    {
        if ( rootCollector.Nodes.Count == 1 && rootCollector.Nodes[0].Kind is DockNodeKind.Split or DockNodeKind.Tabs )
            return rootCollector.Nodes[0];

        DockNodeState center = BuildSimpleDockNode( DockPanePosition.Center )
            ?? rootCollector.Nodes.FirstOrDefault( x => x.Kind == DockNodeKind.Content )
            ?? new() { Kind = DockNodeKind.Content };
        DockNodeState left = BuildSimpleDockNode( DockPanePosition.Left );
        DockNodeState right = BuildSimpleDockNode( DockPanePosition.Right );
        DockNodeState top = BuildSimpleDockNode( DockPanePosition.Top );
        DockNodeState bottom = BuildSimpleDockNode( DockPanePosition.Bottom );
        DockNodeState root = center;

        if ( left is not null )
            root = CreateSplitNode( left, root, DockSplitOrientation.Horizontal, 0.18 );

        if ( right is not null )
            root = CreateSplitNode( root, right, DockSplitOrientation.Horizontal, 0.78 );

        if ( top is not null )
            root = CreateSplitNode( top, root, DockSplitOrientation.Vertical, 0.12 );

        if ( bottom is not null )
            root = CreateSplitNode( root, bottom, DockSplitOrientation.Vertical, 0.84 );

        return root;
    }

    private void EnsureNodeIds( DockNodeState node )
    {
        if ( node is null )
            return;

        if ( string.IsNullOrWhiteSpace( node.Id ) )
            node.Id = $"dock-node-{++nextNodeId}";

        if ( node.Kind == DockNodeKind.Split )
        {
            EnsureNodeIds( node.First );
            EnsureNodeIds( node.Second );
        }
    }

    private DockNodeState BuildSimpleDockNode( DockPanePosition position )
    {
        List<string> paneNames = panes.Values
            .Where( x => GetPaneState( x )?.Position == position )
            .OrderBy( x => FindPaneState( x.ResolvedName )?.Order ?? 0 )
            .Select( x => x.ResolvedName )
            .ToList();

        if ( paneNames.Count == 0 )
            return null;

        if ( paneNames.Count == 1 && !ShouldKeepSinglePaneTabNode( paneNames[0] ) )
        {
            return new()
            {
                Kind = DockNodeKind.Pane,
                PaneName = paneNames[0],
            };
        }

        return new()
        {
            Kind = DockNodeKind.Tabs,
            Panes = paneNames,
            ActivePane = paneNames[0],
            Size = position == DockPanePosition.Center ? null : GetDockGroupSize( paneNames ),
        };
    }

    private bool ShouldKeepSinglePaneTabNode( string paneName )
        => panes.TryGetValue( paneName, out DockPane pane ) && pane.DockRole == DockRole.Document && pane.EffectiveShowTab;

    private void MoveNodeToZone( DockNodeState movingNode, IReadOnlyList<string> movingPaneNames, string targetName, string targetNodeId, DockZone zone, bool mergeWithTargetTabs, bool targetExists )
    {
        DockNodeState originalRoot = CurrentState.Root;

        CurrentState.Root = RemoveTabsNode( CurrentState.Root, movingNode );

        if ( ( zone == DockZone.Center || mergeWithTargetTabs ) && targetExists )
        {
            foreach ( string movingPaneName in movingPaneNames )
                CurrentState.Root = AddPaneToTabs( CurrentState.Root, targetName, movingPaneName );

            return;
        }

        if ( targetExists )
        {
            CurrentState.Root = SplitTargetNode( CurrentState.Root, targetName, targetNodeId, movingNode, zone );
            return;
        }

        if ( zone == DockZone.Center )
        {
            string centerPaneName = GetFirstPaneName( DockPanePosition.Center, movingPaneNames );

            if ( !string.IsNullOrWhiteSpace( centerPaneName ) )
            {
                foreach ( string movingPaneName in movingPaneNames )
                    CurrentState.Root = AddPaneToTabs( CurrentState.Root, centerPaneName, movingPaneName );

                return;
            }

            CurrentState.Root = AddNodeToCenter( CurrentState.Root, movingNode );
            return;
        }

        DockPanePosition? position = ToDockPanePosition( zone );

        if ( position is null )
        {
            CurrentState.Root = originalRoot;
            return;
        }

        CurrentState.Root = position.Value switch
        {
            DockPanePosition.Left => CreateSplitNode( movingNode, CurrentState.Root, DockSplitOrientation.Horizontal, 0.22 ),
            DockPanePosition.Right => CreateSplitNode( CurrentState.Root, movingNode, DockSplitOrientation.Horizontal, 0.78 ),
            DockPanePosition.Top => CreateSplitNode( movingNode, CurrentState.Root, DockSplitOrientation.Vertical, 0.22 ),
            DockPanePosition.Bottom => CreateSplitNode( CurrentState.Root, movingNode, DockSplitOrientation.Vertical, 0.78 ),
            DockPanePosition.Center => CurrentState.Root,
            _ => CurrentState.Root,
        };
    }

    private void MovePaneToZone( string paneName, string targetName, string targetNodeId, DockZone zone, bool mergeWithTargetTabs )
    {
        DockNodeState originalRoot = CurrentState.Root;
        bool targetExists = !string.IsNullOrWhiteSpace( targetName ) && targetName != paneName && ContainsPane( CurrentState.Root, targetName );

        CurrentState.Root = RemovePaneNode( CurrentState.Root, paneName );

        DockNodeState paneNode = new()
        {
            Kind = DockNodeKind.Pane,
            PaneName = paneName,
        };

        if ( ( zone == DockZone.Center || mergeWithTargetTabs ) && targetExists )
        {
            CurrentState.Root = AddPaneToTabs( CurrentState.Root, targetName, paneName );
            return;
        }

        if ( targetExists )
        {
            CurrentState.Root = SplitTargetNode( CurrentState.Root, targetName, targetNodeId, paneNode, zone );
            return;
        }

        if ( zone == DockZone.Center )
        {
            string centerPaneName = GetFirstPaneName( DockPanePosition.Center, [paneName] );

            if ( !string.IsNullOrWhiteSpace( centerPaneName ) )
            {
                CurrentState.Root = AddPaneToTabs( CurrentState.Root, centerPaneName, paneName );
                return;
            }

            CurrentState.Root = AddNodeToCenter( CurrentState.Root, paneNode );
            return;
        }

        DockPanePosition? position = ToDockPanePosition( zone );

        if ( position is null )
        {
            CurrentState.Root = originalRoot;
            return;
        }

        CurrentState.Root = position.Value switch
        {
            DockPanePosition.Left => CreateSplitNode( paneNode, CurrentState.Root, DockSplitOrientation.Horizontal, 0.22 ),
            DockPanePosition.Right => CreateSplitNode( CurrentState.Root, paneNode, DockSplitOrientation.Horizontal, 0.78 ),
            DockPanePosition.Top => CreateSplitNode( paneNode, CurrentState.Root, DockSplitOrientation.Vertical, 0.22 ),
            DockPanePosition.Bottom => CreateSplitNode( CurrentState.Root, paneNode, DockSplitOrientation.Vertical, 0.78 ),
            DockPanePosition.Center => CurrentState.Root,
            _ => CurrentState.Root,
        };
    }

    private static bool ContainsPane( DockNodeState node, string paneName )
    {
        if ( node is null )
            return false;

        return node.Kind switch
        {
            DockNodeKind.Pane => node.PaneName == paneName,
            DockNodeKind.Tabs => node.Panes.Contains( paneName ),
            DockNodeKind.Split => ContainsPane( node.First, paneName ) || ContainsPane( node.Second, paneName ),
            _ => false,
        };
    }

    private DockPanePosition? FindPanePosition( DockNodeState node, string paneName )
        => FindPanePosition( node, paneName, DockPanePosition.Center );

    private DockPanePosition? FindPanePosition( DockNodeState node, string paneName, DockPanePosition inheritedPosition )
    {
        if ( node is null || string.IsNullOrWhiteSpace( paneName ) )
            return null;

        return node.Kind switch
        {
            DockNodeKind.Pane => node.PaneName == paneName ? ResolvePanePosition( paneName, inheritedPosition ) : null,
            DockNodeKind.Tabs => node.Panes.Contains( paneName ) ? ResolvePanePosition( paneName, inheritedPosition ) : null,
            DockNodeKind.Split => FindPanePosition( node.First, paneName, GetFirstSplitPosition( node, inheritedPosition ) )
                ?? FindPanePosition( node.Second, paneName, GetSecondSplitPosition( node, inheritedPosition ) ),
            _ => null,
        };
    }

    private DockPanePosition ResolvePanePosition( string paneName, DockPanePosition inheritedPosition )
        => panes.TryGetValue( paneName, out DockPane pane ) && pane.DockRole == DockRole.Document
            ? DockPanePosition.Center
            : inheritedPosition;

    private bool ShouldPreserveInheritedPosition( DockNodeState node, DockPanePosition inheritedPosition )
        => inheritedPosition != DockPanePosition.Center && HasOnlyPanesInPosition( node, inheritedPosition );

    private bool HasOnlyPanesInPosition( DockNodeState node, DockPanePosition position )
    {
        if ( node is null )
            return false;

        if ( node.Kind == DockNodeKind.Pane )
            return IsPaneInPosition( node.PaneName, position );

        if ( node.Kind == DockNodeKind.Tabs )
            return node.Panes.Count > 0 && node.Panes.All( paneName => IsPaneInPosition( paneName, position ) );

        if ( node.Kind == DockNodeKind.Split )
            return HasOnlyPanesInPosition( node.First, position ) && HasOnlyPanesInPosition( node.Second, position );

        return false;
    }

    private bool IsPaneInPosition( string paneName, DockPanePosition position )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) )
            return false;

        if ( panes.TryGetValue( paneName, out DockPane pane ) && pane.DockRole == DockRole.Document )
            return position == DockPanePosition.Center;

        return FindPaneState( paneName )?.Position == position;
    }

    private static DockPanePosition? GetDropPanePosition( DockPaneState targetState, DockZone zone, bool mergeWithTargetTabs )
    {
        if ( mergeWithTargetTabs )
            return targetState?.Position;

        if ( targetState is not null && targetState.Position != DockPanePosition.Center )
            return targetState.Position;

        return ToDockPanePosition( zone );
    }

    private DockPanePosition GetFirstSplitPosition( DockNodeState node, DockPanePosition inheritedPosition )
        => ShouldPreserveInheritedPosition( node, inheritedPosition )
            ? inheritedPosition
            : node.Orientation == DockSplitOrientation.Horizontal ? DockPanePosition.Left : DockPanePosition.Top;

    private DockPanePosition GetSecondSplitPosition( DockNodeState node, DockPanePosition inheritedPosition )
        => ShouldPreserveInheritedPosition( node, inheritedPosition )
            ? inheritedPosition
            : node.Orientation == DockSplitOrientation.Horizontal ? DockPanePosition.Right : DockPanePosition.Bottom;

    private DockNodeState AddPaneToTabs( DockNodeState node, string targetName, string paneName )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Pane && node.PaneName == targetName )
        {
            DockNodeState tabsNode = new()
            {
                Kind = DockNodeKind.Tabs,
                ActivePane = paneName,
            };

            tabsNode.Panes.Add( targetName );
            tabsNode.Panes.Add( paneName );
            SetDockTabsNodeSize( tabsNode );

            return tabsNode;
        }

        if ( node.Kind == DockNodeKind.Tabs && node.Panes.Contains( targetName ) )
        {
            if ( !node.Panes.Contains( paneName ) )
                node.Panes.Add( paneName );

            node.ActivePane = paneName;
            SetDockTabsNodeSize( node );

            return node;
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            node.First = AddPaneToTabs( node.First, targetName, paneName );
            node.Second = AddPaneToTabs( node.Second, targetName, paneName );
        }

        return node;
    }

    private void SetDockTabsNodeSize( DockNodeState node )
    {
        if ( IsCenterDockGroup( node.Panes ) )
            node.Size = null;
        else
            node.Size ??= GetDockGroupSize( node.Panes );
    }

    private string GetDockGroupSize( IEnumerable<string> paneNames )
    {
        foreach ( string paneName in paneNames )
        {
            string paneSize = GetDockPaneSize( paneName );

            if ( !string.IsNullOrWhiteSpace( paneSize ) )
                return paneSize;
        }

        return DefaultPaneSize;
    }

    private bool IsCenterDockGroup( IEnumerable<string> paneNames )
        => paneNames.Any( IsCenterDockPane );

    private bool IsCenterDockPane( string paneName )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) )
            return false;

        if ( panes.TryGetValue( paneName, out DockPane pane ) && pane.DockRole == DockRole.Document )
            return true;

        return FindPaneState( paneName )?.Position == DockPanePosition.Center;
    }

    private string GetDockPaneSize( string paneName )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) || !panes.TryGetValue( paneName, out DockPane pane ) )
            return null;

        DockPaneState paneState = FindPaneState( paneName );

        return paneState?.Size ?? pane.Size;
    }

    private string GetFirstPaneName( DockPanePosition position, IReadOnlyList<string> excludedPaneNames )
        => CurrentState.Panes
            .Where( x => x.Visible && x.Position == position && !excludedPaneNames.Contains( x.Name ) )
            .OrderBy( x => x.Order )
            .Select( x => x.Name )
            .FirstOrDefault();

    private static DockNodeState AddNodeToCenter( DockNodeState root, DockNodeState centerNode )
    {
        DockNodeState updatedRoot = ReplaceContentNode( root, centerNode, out bool replaced );

        return replaced ? updatedRoot : centerNode;
    }

    private static DockNodeState ReplaceContentNode( DockNodeState node, DockNodeState centerNode, out bool replaced )
    {
        replaced = false;

        if ( node is null )
            return centerNode;

        if ( node.Kind == DockNodeKind.Content )
        {
            replaced = true;
            return centerNode;
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            node.First = ReplaceContentNode( node.First, centerNode, out bool firstReplaced );

            if ( firstReplaced )
            {
                replaced = true;
                return node;
            }

            node.Second = ReplaceContentNode( node.Second, centerNode, out bool secondReplaced );
            replaced = secondReplaced;
        }

        return node;
    }

    private static DockNodeState RemoveTabsNode( DockNodeState node, DockNodeState tabsNode )
    {
        if ( node is null )
            return null;

        if ( ReferenceEquals( node, tabsNode ) )
            return null;

        if ( node.Kind == DockNodeKind.Split )
        {
            DockNodeState first = RemoveTabsNode( node.First, tabsNode );
            DockNodeState second = RemoveTabsNode( node.Second, tabsNode );

            if ( first is null )
                return second;

            if ( second is null )
                return first;

            node.First = first;
            node.Second = second;

            return node;
        }

        return node;
    }

    private DockNodeState SplitTargetNode( DockNodeState node, string targetName, string targetNodeId, DockNodeState paneNode, DockZone zone )
    {
        if ( node is null )
            return null;

        if ( !string.IsNullOrWhiteSpace( targetNodeId ) && node.Id == targetNodeId )
            return CreateTargetSplitNode( node, paneNode, zone );

        if ( node.Kind == DockNodeKind.Pane && node.PaneName == targetName )
            return CreateTargetSplitNode( node, paneNode, zone );

        if ( node.Kind == DockNodeKind.Tabs && node.Panes.Contains( targetName ) )
            return CreateTargetSplitNode( node, paneNode, zone );

        if ( node.Kind == DockNodeKind.Split )
        {
            node.First = SplitTargetNode( node.First, targetName, targetNodeId, paneNode, zone );
            node.Second = SplitTargetNode( node.Second, targetName, targetNodeId, paneNode, zone );
        }

        return node;
    }

    private DockNodeState CreateTargetSplitNode( DockNodeState targetNode, DockNodeState paneNode, DockZone zone )
    {
        string targetSize = GetDockNodeSize( targetNode );
        DockNodeState splitNode = zone switch
        {
            DockZone.Left => CreateSplitNode( paneNode, targetNode, DockSplitOrientation.Horizontal, 0.32 ),
            DockZone.Right => CreateSplitNode( targetNode, paneNode, DockSplitOrientation.Horizontal, 0.68 ),
            DockZone.Top => CreateSplitNode( paneNode, targetNode, DockSplitOrientation.Vertical, 0.32 ),
            DockZone.Bottom => CreateSplitNode( targetNode, paneNode, DockSplitOrientation.Vertical, 0.68 ),
            _ => targetNode,
        };

        if ( splitNode?.Kind == DockNodeKind.Split )
        {
            splitNode.Size = targetSize;
            splitNode.UseRatio = true;
        }

        return splitNode;
    }

    private string GetDockNodeSize( DockNodeState node )
        => node?.Kind switch
        {
            DockNodeKind.Pane => GetDockPaneSize( node.PaneName ),
            DockNodeKind.Tabs => node.Size ?? GetDockGroupSize( node.Panes ),
            DockNodeKind.Split => node.Size,
            _ => null,
        };

    private static DockNodeState RemovePaneNode( DockNodeState node, string paneName )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Pane )
            return node.PaneName == paneName ? null : node;

        if ( node.Kind == DockNodeKind.Tabs )
        {
            node.Panes.Remove( paneName );

            if ( node.Panes.Count == 0 )
                return null;

            if ( node.ActivePane == paneName )
                node.ActivePane = node.Panes[0];

            return node;
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            DockNodeState first = RemovePaneNode( node.First, paneName );
            DockNodeState second = RemovePaneNode( node.Second, paneName );

            if ( first is null )
                return second;

            if ( second is null )
                return first;

            node.First = first;
            node.Second = second;

            return node;
        }

        return node;
    }

    private static DockNodeState CreateSplitNode( DockNodeState first, DockNodeState second, DockSplitOrientation orientation, double ratio )
        => first is null ? second : second is null ? first : new()
        {
            Kind = DockNodeKind.Split,
            First = first,
            Second = second,
            Orientation = orientation,
            Ratio = ratio,
        };

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
        if ( CurrentState.Root is not null )
        {
            EnsureNodeIds( CurrentState.Root );
            CurrentState.Root = DockLayoutNormalizer.Normalize( CurrentState.Root, panes, CurrentState.Panes );
            EnsureNodeIds( CurrentState.Root );
            SyncPanePositionsFromTree();
        }
    }

    private void SyncPanePositionsFromTree()
    {
        foreach ( DockPaneState paneState in CurrentState.Panes )
        {
            DockPanePosition? position = FindPanePosition( CurrentState.Root, paneState.Name );

            if ( position is not null )
                paneState.Position = position.Value;
        }
    }

    private static DockPanePosition? ToDockPanePosition( DockZone zone )
        => zone switch
        {
            DockZone.Center => DockPanePosition.Center,
            DockZone.Left => DockPanePosition.Left,
            DockZone.Right => DockPanePosition.Right,
            DockZone.Top => DockPanePosition.Top,
            DockZone.Bottom => DockPanePosition.Bottom,
            _ => null,
        };

    private static DockZone? ToDockZone( string value )
    {
        if ( Enum.TryParse<DockZone>( value, out DockZone zone ) )
            return zone;

        return null;
    }

    #endregion

    #region Properties

    internal bool DockGuidesVisible => draggingPaneName is not null && ( dockCompassX != 0d || dockCompassY != 0d );

    internal static IReadOnlyList<DockCompassZoneInfo> DockCompassZones => dockCompassZones;

    internal string ActiveDockCompassZoneKey => activeDockCompassZoneKey;

    internal DockZone? ActiveDockZone => activeDockZone;

    private DockLayoutState CurrentState => state ??= new();

    internal DockNodeCollector RootCollector => rootCollector;

    internal DockContent Content => content;

    internal DockLayoutContext Context => context ??= new( this );

    internal int RenderVersion => renderVersion;

    internal int DockGuidesVersion => dockGuidesVersion;

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
    /// Specifies the panes and content to be rendered inside the dock layout.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Defines whether non-document panes should render a visible border.
    /// </summary>
    [Parameter] public bool PaneBordered { get; set; } = true;

    /// <summary>
    /// Defines the mutable state used for docking, resizing, active tabs, and pane visibility.
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