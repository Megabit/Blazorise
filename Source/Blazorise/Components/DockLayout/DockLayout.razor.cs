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

    private string draggingPaneName;

    private DockZone? activeDockZone;

    private string activeDockTargetName;

    private bool draggingPaneGroup;

    private double dockCompassX;

    private double dockCompassY;

    private DotNetObjectReference<DockLayout> dotNetObjectRef;

    private static readonly DockZone[] dockZones =
    {
        DockZone.Center,
        DockZone.Left,
        DockZone.Top,
        DockZone.Right,
        DockZone.Bottom,
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

        DockPaneState paneState = EnsurePaneState( pane );

        EnsureActivePane( paneState.Position );
    }

    internal void RegisterContent( DockContent dockContent )
    {
        content = dockContent;
    }

    internal void NotifyDefinitionChanged()
    {
        _ = InvokeAsync( async () =>
        {
            if ( CurrentState.Root is null && rootCollector.Nodes.Count > 0 )
                CurrentState.Root = BuildInitialRoot();

            await NotifyStateChanged();
        } );
    }

    internal void UnregisterPane( DockPane pane )
    {
        string paneName = pane.ResolvedName;

        if ( string.IsNullOrWhiteSpace( paneName ) )
            return;

        panes.Remove( paneName );

        foreach ( KeyValuePair<DockPanePosition, string> activePaneName in CurrentState.ActivePanes.ToArray() )
        {
            if ( activePaneName.Value == paneName )
                CurrentState.ActivePanes.Remove( activePaneName.Key );
        }
    }

    internal DockPaneState GetPaneState( DockPane pane )
        => EnsurePaneState( pane );

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

    internal bool IsPaneActive( DockPane pane )
    {
        DockPaneState paneState = EnsurePaneState( pane );

        EnsureActivePane( paneState.Position );

        return CurrentState.ActivePanes.TryGetValue( paneState.Position, out string activePaneName ) && activePaneName == paneState.Name;
    }

    internal string GetPaneTabClass( DockPaneState paneState )
    {
        EnsureActivePane( paneState.Position );

        bool active = CurrentState.ActivePanes.TryGetValue( paneState.Position, out string activePaneName ) && activePaneName == paneState.Name;

        return ClassProvider.DockPaneTab( active );
    }

    internal string GetPaneCaption( string paneName )
        => panes.TryGetValue( paneName, out DockPane pane ) ? pane.ResolvedCaption : paneName;

    internal string GetDockTabClass( DockNodeState node, string paneName )
        => ClassProvider.DockPaneTab( GetActiveTabPaneName( node ) == paneName );

    internal string GetDockPaneTabsClass()
        => ClassProvider.DockPaneTabs();

    internal string GetDockSplitStyle( DockNodeState node )
    {
        if ( node is null || node.Kind != DockNodeKind.Split )
            return null;

        string firstFixedTrack = GetDockNodeTrackSize( node.First, node.Orientation );
        string secondFixedTrack = GetDockNodeTrackSize( node.Second, node.Orientation );
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

    internal string GetActiveTabPaneName( DockNodeState node )
    {
        if ( node is null || node.Kind != DockNodeKind.Tabs )
            return null;

        if ( !string.IsNullOrWhiteSpace( node.ActivePane ) && node.Panes.Contains( node.ActivePane ) )
            return node.ActivePane;

        return node.Panes.Count > 0 ? node.Panes[0] : null;
    }

    internal Task ActivateTab( DockNodeState node, string paneName )
    {
        if ( node?.Kind == DockNodeKind.Tabs && node.Panes.Contains( paneName ) )
        {
            node.ActivePane = paneName;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    internal Task ActivatePane( string paneName )
    {
        DockPaneState paneState = FindPaneState( paneName );

        if ( paneState is not null )
        {
            CurrentState.ActivePanes[paneState.Position] = paneState.Name;
            StateHasChanged();
        }

        return Task.CompletedTask;
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
        DockPaneState paneState = EnsurePaneState( pane );

        paneState.Visible = false;
        CurrentState.Root = RemovePaneNode( CurrentState.Root, paneState.Name );

        await NotifyStateChanged();
    }

    internal async Task BeginPaneResize( DockPane pane, PointerEventArgs eventArgs )
    {
        if ( pane?.Resizable != true )
            return;

        await JSModule.BeginResize(
            DotNetObjectRef,
            pane.ElementRef,
            pane.ResolvedName,
            pane.EffectivePosition.ToString(),
            eventArgs.ClientX,
            eventArgs.ClientY,
            pane.MinSize,
            pane.MaxSize );
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
    /// <param name="size">The new pane size.</param>
    /// <returns>A task that completes after the state is updated.</returns>
    [JSInvokable]
    public async Task NotifyDockPaneResized( string paneName, string size )
    {
        DockPaneState paneState = FindPaneState( paneName );

        if ( paneState is null )
            return;

        DockNodeState tabsNode = FindTabsNode( CurrentState.Root, paneName );

        if ( tabsNode is not null )
            tabsNode.Size = size;
        else
            paneState.Size = size;

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
    /// <param name="zone">The currently hovered drop zone.</param>
    /// <param name="compassX">The horizontal compass location relative to the layout.</param>
    /// <param name="compassY">The vertical compass location relative to the layout.</param>
    /// <returns>A completed task.</returns>
    [JSInvokable]
    public Task NotifyDockPaneDrag( string paneName, string targetName, string zone, double compassX, double compassY )
    {
        activeDockZone = ToDockZone( zone );
        activeDockTargetName = targetName;
        dockCompassX = compassX;
        dockCompassY = compassY;
        draggingPaneName = paneName;

        DirtyStyles();
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Completes a pane drag operation and docks the pane in the selected zone.
    /// </summary>
    /// <param name="paneName">The dragged pane name.</param>
    /// <param name="targetName">The pane currently under the pointer.</param>
    /// <param name="zone">The selected drop zone.</param>
    /// <returns>A task that completes after the state is updated.</returns>
    [JSInvokable]
    public async Task NotifyDockPaneDropped( string paneName, string targetName, string zone )
    {
        DockZone? targetZone = ToDockZone( zone );
        DockPaneState paneState = FindPaneState( paneName );
        bool moveGroup = draggingPaneGroup;

        draggingPaneName = null;
        activeDockZone = null;
        activeDockTargetName = null;
        draggingPaneGroup = false;

        if ( paneState is not null && targetZone is not null )
        {
            DockNodeState sourceTabsNode = moveGroup ? FindTabsNode( CurrentState.Root, paneName ) : null;
            IReadOnlyList<string> movedPaneNames = sourceTabsNode?.Panes.Count > 1
                ? sourceTabsNode.Panes.ToArray()
                : new[] { paneName };
            DockPaneState targetState = FindPaneState( targetName );
            bool targetExists = !string.IsNullOrWhiteSpace( targetName ) && !movedPaneNames.Contains( targetName ) && ContainsPane( CurrentState.Root, targetName );
            bool mergeWithTargetTabs = targetState is not null && targetState.Position != paneState.Position;
            DockPanePosition? targetPosition = mergeWithTargetTabs
                ? targetState?.Position
                : ToDockPanePosition( targetZone.Value );

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
                MoveNodeToZone( sourceTabsNode, movedPaneNames, targetName, targetZone.Value, mergeWithTargetTabs, targetExists );
            else
                MovePaneToZone( paneName, targetName, targetZone.Value, mergeWithTargetTabs );

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

    private void EnsureActivePane( DockPanePosition position )
    {
        if ( CurrentState.ActivePanes.ContainsKey( position ) )
            return;

        DockPaneState paneState = CurrentState.Panes
            .Where( x => x.Visible && x.Position == position )
            .OrderBy( x => x.Order )
            .FirstOrDefault();

        if ( paneState is not null )
            CurrentState.ActivePanes[position] = paneState.Name;
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
        DockPane pane = GetDockNodePane( node );

        if ( pane is null || !IsPanePositionCompatibleWithOrientation( pane.EffectivePosition, orientation ) )
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

        return paneState?.Size ?? pane.Size ?? DefaultPaneSize;
    }

    private DockPane GetDockNodePane( DockNodeState node )
    {
        if ( node is null )
            return null;

        string paneName = node.Kind switch
        {
            DockNodeKind.Pane => node.PaneName,
            DockNodeKind.Tabs => GetActiveTabPaneName( node ),
            _ => null,
        };

        return !string.IsNullOrWhiteSpace( paneName ) && panes.TryGetValue( paneName, out DockPane pane )
            ? pane
            : null;
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

    private DockNodeState BuildSimpleDockNode( DockPanePosition position )
    {
        List<string> paneNames = panes.Values
            .Where( x => GetPaneState( x )?.Position == position )
            .OrderBy( x => FindPaneState( x.ResolvedName )?.Order ?? 0 )
            .Select( x => x.ResolvedName )
            .ToList();

        if ( paneNames.Count == 0 )
            return null;

        if ( paneNames.Count == 1 )
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
            Size = GetDockGroupSize( paneNames ),
        };
    }

    private void MoveNodeToZone( DockNodeState movingNode, IReadOnlyList<string> movingPaneNames, string targetName, DockZone zone, bool mergeWithTargetTabs, bool targetExists )
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
            CurrentState.Root = SplitTargetPane( CurrentState.Root, targetName, movingNode, zone );
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

    private void MovePaneToZone( string paneName, string targetName, DockZone zone, bool mergeWithTargetTabs )
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
            CurrentState.Root = SplitTargetPane( CurrentState.Root, targetName, paneNode, zone );
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

    private DockNodeState AddPaneToTabs( DockNodeState node, string targetName, string paneName )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Pane && node.PaneName == targetName )
        {
            return new()
            {
                Kind = DockNodeKind.Tabs,
                Panes = { targetName, paneName },
                ActivePane = paneName,
                Size = GetDockPaneSize( targetName ),
            };
        }

        if ( node.Kind == DockNodeKind.Tabs && node.Panes.Contains( targetName ) )
        {
            node.Size ??= GetDockPaneSize( targetName );

            if ( !node.Panes.Contains( paneName ) )
                node.Panes.Add( paneName );

            node.ActivePane = paneName;

            return node;
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            node.First = AddPaneToTabs( node.First, targetName, paneName );
            node.Second = AddPaneToTabs( node.Second, targetName, paneName );
        }

        return node;
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

    private static DockNodeState SplitTargetPane( DockNodeState node, string targetName, DockNodeState paneNode, DockZone zone )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Pane && node.PaneName == targetName )
            return CreateTargetSplitNode( node, paneNode, zone );

        if ( node.Kind == DockNodeKind.Tabs && node.Panes.Contains( targetName ) )
            return CreateTargetSplitNode( node, paneNode, zone );

        if ( node.Kind == DockNodeKind.Split )
        {
            node.First = SplitTargetPane( node.First, targetName, paneNode, zone );
            node.Second = SplitTargetPane( node.Second, targetName, paneNode, zone );
        }

        return node;
    }

    private static DockNodeState CreateTargetSplitNode( DockNodeState targetNode, DockNodeState paneNode, DockZone zone )
        => zone switch
        {
            DockZone.Left => CreateSplitNode( paneNode, targetNode, DockSplitOrientation.Horizontal, 0.32 ),
            DockZone.Right => CreateSplitNode( targetNode, paneNode, DockSplitOrientation.Horizontal, 0.68 ),
            DockZone.Top => CreateSplitNode( paneNode, targetNode, DockSplitOrientation.Vertical, 0.32 ),
            DockZone.Bottom => CreateSplitNode( targetNode, paneNode, DockSplitOrientation.Vertical, 0.68 ),
            _ => targetNode,
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

            return node.Panes.Count == 1
                ? new()
                {
                    Kind = DockNodeKind.Pane,
                    PaneName = node.Panes[0],
                }
                : node;
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

    private int GetNextOrder( DockPanePosition position )
    {
        int? order = CurrentState.Panes
            .Where( x => x.Position == position )
            .Select( x => (int?)x.Order )
            .Max();

        return order.GetValueOrDefault() + 1;
    }

    private async Task NotifyStateChanged()
    {
        DirtyClasses();
        DirtyStyles();

        if ( StateChanged.HasDelegate )
            await StateChanged.InvokeAsync( CurrentState );

        StateHasChanged();
    }

    internal string GetDockCompassClass()
        => ClassProvider.DockLayoutCompass();

    internal string GetDockCompassZoneClass( DockZone zone )
        => ClassProvider.DockLayoutCompassZone( zone, activeDockZone == zone );

    internal string GetDockShellGuideClass( DockZone zone )
        => ClassProvider.DockLayoutShellGuide( zone, activeDockTargetName is null && activeDockZone == zone );

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

    internal bool DockGuidesVisible => draggingPaneName is not null && activeDockZone is not null;

    internal static IReadOnlyList<DockZone> DockZones => dockZones;

    private DockLayoutState CurrentState => state ??= new();

    internal DockNodeCollector RootCollector => rootCollector;

    internal DockContent Content => content;

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
    /// Gets the DockLayout JavaScript module.
    /// </summary>
    [Inject] protected IJSDockLayoutModule JSModule { get; set; }

    /// <summary>
    /// Specifies the panes and content to be rendered inside the dock layout.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

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
}