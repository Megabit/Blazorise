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
/// Arranges docked panels around a central content surface.
/// </summary>
public partial class DockLayout : BaseComponent
{
    #region Members

    private readonly Dictionary<string, DockPanel> panels = new();

    private DockNodeCollector rootCollector = new();

    private DockLayoutState state;

    private DockContent content;

    private string draggingPanelName;

    private DockZone? activeDockZone;

    private string activeDockTargetName;

    private bool draggingPanelGroup;

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

    private const string DefaultPanelSize = "16rem";

    private const string AutoHidePanelSize = "2rem";

    private const string CollapsedPanelSize = "2.5rem";

    private const string FlexibleFillTrack = "minmax(0,1fr)";

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockLayout() );

        base.BuildClasses( builder );
    }

    internal void RegisterPanel( DockPanel panel )
    {
        string panelName = panel.ResolvedName;

        if ( string.IsNullOrWhiteSpace( panelName ) )
            return;

        panels[panelName] = panel;
        EnsurePanelState( panel );
        EnsureActivePanel( panel.Dock );
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

    internal void UnregisterPanel( DockPanel panel )
    {
        string panelName = panel.ResolvedName;

        if ( string.IsNullOrWhiteSpace( panelName ) )
            return;

        panels.Remove( panelName );

        foreach ( KeyValuePair<DockPanelPosition, string> activePanelName in CurrentState.ActivePanels.ToArray() )
        {
            if ( activePanelName.Value == panelName )
                CurrentState.ActivePanels.Remove( activePanelName.Key );
        }
    }

    internal DockPanelState GetPanelState( DockPanel panel )
        => EnsurePanelState( panel );

    internal IReadOnlyList<DockPanelState> GetPanelStates( DockPanelPosition position )
        => CurrentState.Panels
            .Where( x => x.Visible && x.Position == position )
            .OrderBy( x => x.Order )
            .ToArray();

    internal bool HasPanelTabs( DockPanelPosition position )
        => GetPanelStates( position ).Count > 1;

    internal bool IsPanelActive( DockPanel panel )
    {
        DockPanelState panelState = EnsurePanelState( panel );

        EnsureActivePanel( panelState.Position );

        return CurrentState.ActivePanels.TryGetValue( panelState.Position, out string activePanelName ) && activePanelName == panelState.Name;
    }

    internal string GetPanelTabClass( DockPanelState panelState )
    {
        EnsureActivePanel( panelState.Position );

        bool active = CurrentState.ActivePanels.TryGetValue( panelState.Position, out string activePanelName ) && activePanelName == panelState.Name;

        return ClassProvider.DockPanelTab( active );
    }

    internal string GetPanelCaption( string panelName )
        => panels.TryGetValue( panelName, out DockPanel panel ) ? panel.ResolvedCaption : panelName;

    internal string GetDockTabClass( DockNodeState node, string panelName )
        => ClassProvider.DockPanelTab( GetActiveTabPanelName( node ) == panelName );

    internal string GetDockPanelTabsClass()
        => ClassProvider.DockPanelTabs();

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

    internal bool IsPanelAutoHidden( string panelName )
        => FindPanelState( panelName )?.AutoHide == true;

    internal bool IsTabGroupAutoHidden( DockNodeState node )
    {
        if ( node?.Kind != DockNodeKind.Tabs )
            return false;

        string activePanelName = GetActiveTabPanelName( node );

        return IsPanelAutoHidden( activePanelName );
    }

    internal RenderFragment RenderTabbedPanelGroup( DockNodeState node )
        => builder =>
        {
            if ( node?.Kind != DockNodeKind.Tabs )
                return;

            if ( IsTabGroupAutoHidden( node ) )
            {
                builder.AddContent( 0, RenderAutoHideTabs( node ) );
                return;
            }

            string activePanelName = GetActiveTabPanelName( node );

            if ( string.IsNullOrWhiteSpace( activePanelName ) || !panels.TryGetValue( activePanelName, out DockPanel activePanel ) )
                return;

            DockPanelState activePanelState = FindPanelState( activePanelName );
            int sequence = 0;

            builder.OpenComponent<CascadingValue<DockPanel>>( sequence++ );
            builder.AddAttribute( sequence++, "Value", activePanel );
            builder.AddAttribute( sequence++, "IsFixed", false );
            builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
            {
                int childSequence = 0;

                childBuilder.OpenElement( childSequence++, "aside" );
                childBuilder.AddAttribute( childSequence++, "class", GetRenderedTabsHostClass( activePanel, activePanelState ) );
                childBuilder.AddAttribute( childSequence++, "style", GetRenderedPanelStyle( activePanel, activePanelState, node.Size ) );
                childBuilder.AddAttribute( childSequence++, "data-dock-panel-name", activePanelName );
                childBuilder.AddElementReferenceCapture( childSequence++, elementReference => activePanel.ElementRef = elementReference );
                childBuilder.AddContent( childSequence++, activePanel.ChildContent );
                childBuilder.OpenElement( childSequence++, "div" );
                childBuilder.AddAttribute( childSequence++, "class", GetDockPanelTabsClass() );

                foreach ( string panelName in node.Panels )
                {
                    childBuilder.OpenElement( childSequence++, "button" );
                    childBuilder.AddAttribute( childSequence++, "type", "button" );
                    childBuilder.AddAttribute( childSequence++, "class", GetDockTabClass( node, panelName ) );
                    childBuilder.AddAttribute( childSequence++, "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>( this, eventArgs => BeginPanelTabDrag( panelName, eventArgs ) ) );
                    childBuilder.AddAttribute( childSequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => ActivateTab( node, panelName ) ) );
                    childBuilder.AddContent( childSequence++, GetPanelCaption( panelName ) );
                    childBuilder.CloseElement();
                }

                childBuilder.CloseElement();

                if ( activePanel.Resizable )
                {
                    childBuilder.OpenComponent<DockSplitter>( childSequence++ );
                    childBuilder.AddAttribute( childSequence++, "Dock", activePanel.EffectivePosition );
                    childBuilder.CloseComponent();
                }

                childBuilder.CloseElement();
            } ) );
            builder.CloseComponent();
        };

    internal RenderFragment RenderAutoHideTabs( DockNodeState node )
        => builder =>
        {
            if ( node?.Kind != DockNodeKind.Tabs )
                return;

            string activePanelName = GetActiveTabPanelName( node );

            if ( string.IsNullOrWhiteSpace( activePanelName ) || !panels.TryGetValue( activePanelName, out DockPanel activePanel ) )
                return;

            int sequence = 0;

            builder.OpenElement( sequence++, "aside" );
            builder.AddAttribute( sequence++, "class", GetRenderedPanelClass( activePanel, FindPanelState( activePanelName ) ) );
            builder.AddAttribute( sequence++, "style", GetRenderedPanelStyle( activePanel, FindPanelState( activePanelName ) ) );
            builder.AddAttribute( sequence++, "data-dock-panel-name", activePanelName );
            builder.AddElementReferenceCapture( sequence++, elementReference => activePanel.ElementRef = elementReference );

            foreach ( string panelName in node.Panels )
            {
                if ( !panels.TryGetValue( panelName, out DockPanel panel ) )
                    continue;

                builder.OpenElement( sequence++, "button" );
                builder.AddAttribute( sequence++, "type", "button" );
                builder.AddAttribute( sequence++, "class", GetRenderedPanelAutoHideTabClass( panel ) );
                builder.AddAttribute( sequence++, "title", panel.ResolvedCaption );
                builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => OpenPanelAutoHide( panel ) ) );
                builder.AddContent( sequence++, panel.ResolvedCaption );
                builder.CloseElement();
            }

            builder.CloseElement();
        };

    internal string GetActiveTabPanelName( DockNodeState node )
    {
        if ( node is null || node.Kind != DockNodeKind.Tabs )
            return null;

        if ( !string.IsNullOrWhiteSpace( node.ActivePanel ) && node.Panels.Contains( node.ActivePanel ) )
            return node.ActivePanel;

        return node.Panels.Count > 0 ? node.Panels[0] : null;
    }

    internal Task ActivateTab( DockNodeState node, string panelName )
    {
        if ( node?.Kind == DockNodeKind.Tabs && node.Panels.Contains( panelName ) )
        {
            node.ActivePanel = panelName;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    internal Task ActivatePanel( string panelName )
    {
        DockPanelState panelState = FindPanelState( panelName );

        if ( panelState is not null )
        {
            CurrentState.ActivePanels[panelState.Position] = panelState.Name;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    internal async Task TogglePanelAutoHide( DockPanel panel )
    {
        DockPanelState panelState = EnsurePanelState( panel );
        DockNodeState tabsNode = FindTabsNode( CurrentState.Root, panelState.Name );

        if ( tabsNode is not null )
        {
            bool autoHide = !panelState.AutoHide;

            foreach ( string panelName in tabsNode.Panels )
                SetPanelAutoHide( panelName, autoHide );
        }
        else
        {
            panelState.AutoHide = !panelState.AutoHide;
        }

        await NotifyStateChanged();
    }

    internal async Task OpenPanelAutoHide( DockPanel panel )
    {
        DockPanelState panelState = EnsurePanelState( panel );
        DockNodeState tabsNode = FindTabsNode( CurrentState.Root, panelState.Name );

        if ( tabsNode is not null )
        {
            foreach ( string panelName in tabsNode.Panels )
                SetPanelAutoHide( panelName, false );

            tabsNode.ActivePanel = panelState.Name;
        }
        else
        {
            panelState.AutoHide = false;
        }

        await NotifyStateChanged();
    }

    internal async Task ClosePanel( DockPanel panel )
    {
        DockPanelState panelState = EnsurePanelState( panel );

        panelState.Visible = false;
        CurrentState.Root = RemovePanelNode( CurrentState.Root, panelState.Name );

        await NotifyStateChanged();
    }

    internal RenderFragment RenderPanel( string panelName )
        => builder =>
        {
            if ( string.IsNullOrWhiteSpace( panelName ) || !panels.TryGetValue( panelName, out DockPanel panel ) )
                return;

            DockPanelState panelState = FindPanelState( panelName );

            if ( panelState?.Visible == false )
                return;

            int sequence = 0;

            builder.OpenComponent<CascadingValue<DockPanel>>( sequence++ );
            builder.AddAttribute( sequence++, "Value", panel );
            builder.AddAttribute( sequence++, "IsFixed", false );
            builder.AddAttribute( sequence++, "ChildContent", (RenderFragment)( childBuilder =>
            {
                int childSequence = 0;

                childBuilder.OpenElement( childSequence++, "aside" );
                childBuilder.AddAttribute( childSequence++, "class", GetRenderedPanelClass( panel, panelState ) );
                childBuilder.AddAttribute( childSequence++, "style", GetRenderedPanelStyle( panel, panelState ) );
                childBuilder.AddAttribute( childSequence++, "data-dock-panel-name", panel.ResolvedName );
                childBuilder.AddElementReferenceCapture( childSequence++, elementReference => panel.ElementRef = elementReference );

                if ( panelState?.AutoHide == true )
                {
                    childBuilder.OpenElement( childSequence++, "button" );
                    childBuilder.AddAttribute( childSequence++, "type", "button" );
                    childBuilder.AddAttribute( childSequence++, "class", GetRenderedPanelAutoHideTabClass( panel ) );
                    childBuilder.AddAttribute( childSequence++, "title", panel.ResolvedCaption );
                    childBuilder.AddAttribute( childSequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => OpenPanelAutoHide( panel ) ) );
                    childBuilder.AddContent( childSequence++, panel.ResolvedCaption );
                    childBuilder.CloseElement();
                }
                else
                {
                    childBuilder.AddContent( childSequence++, panel.ChildContent );

                    if ( panel.Resizable )
                    {
                        childBuilder.OpenComponent<DockSplitter>( childSequence++ );
                        childBuilder.AddAttribute( childSequence++, "Dock", panel.EffectivePosition );
                        childBuilder.CloseComponent();
                    }
                }

                childBuilder.CloseElement();
            } ) );
            builder.CloseComponent();
        };

    internal RenderFragment RenderContent()
        => builder =>
        {
            if ( content is null )
                return;

            int sequence = 0;

            builder.OpenElement( sequence++, "main" );
            builder.AddAttribute( sequence++, "class", content.ClassNames );
            builder.AddAttribute( sequence++, "style", content.StyleNames );
            builder.AddContent( sequence++, content.ChildContent );
            builder.CloseElement();
        };

    internal async Task BeginPanelResize( DockPanel panel, PointerEventArgs eventArgs )
    {
        if ( panel?.Resizable != true )
            return;

        await JSModule.BeginResize(
            DotNetObjectRef,
            panel.ElementRef,
            panel.ResolvedName,
            panel.EffectivePosition.ToString(),
            eventArgs.ClientX,
            eventArgs.ClientY,
            panel.MinSize,
            panel.MaxSize );
    }

    internal Task BeginPanelTabDrag( string panelName, PointerEventArgs eventArgs )
    {
        if ( string.IsNullOrWhiteSpace( panelName ) || !panels.TryGetValue( panelName, out DockPanel panel ) )
            return Task.CompletedTask;

        return BeginPanelDrag( panel, eventArgs, false );
    }

    internal async Task BeginPanelDrag( DockPanel panel, PointerEventArgs eventArgs, bool dragGroup )
    {
        if ( panel?.Movable != true )
            return;

        draggingPanelGroup = dragGroup;

        await ActivatePanel( panel.ResolvedName );

        await JSModule.BeginDrag(
            DotNetObjectRef,
            ElementRef,
            panel.ResolvedName,
            eventArgs.ClientX,
            eventArgs.ClientY,
            dragGroup );
    }

    /// <summary>
    /// Updates a dock panel size while a splitter resize operation is active.
    /// </summary>
    /// <param name="panelName">The resized panel name.</param>
    /// <param name="size">The new panel size.</param>
    /// <returns>A task that completes after the state is updated.</returns>
    [JSInvokable]
    public async Task NotifyDockPanelResized( string panelName, string size )
    {
        DockPanelState panelState = FindPanelState( panelName );

        if ( panelState is null )
            return;

        DockNodeState tabsNode = FindTabsNode( CurrentState.Root, panelName );

        if ( tabsNode is not null )
            tabsNode.Size = size;
        else
            panelState.Size = size;

        await NotifyStateChanged();
    }

    /// <summary>
    /// Completes a dock panel splitter resize operation.
    /// </summary>
    /// <param name="panelName">The resized panel name.</param>
    /// <returns>A task that completes after the state is updated.</returns>
    [JSInvokable]
    public Task NotifyDockPanelResizeEnded( string panelName )
        => NotifyStateChanged();

    /// <summary>
    /// Updates the currently highlighted dock drop zone while a panel is dragged.
    /// </summary>
    /// <param name="panelName">The dragged panel name.</param>
    /// <param name="targetName">The panel currently under the pointer.</param>
    /// <param name="zone">The currently hovered drop zone.</param>
    /// <param name="compassX">The horizontal compass location relative to the layout.</param>
    /// <param name="compassY">The vertical compass location relative to the layout.</param>
    /// <returns>A completed task.</returns>
    [JSInvokable]
    public Task NotifyDockPanelDrag( string panelName, string targetName, string zone, double compassX, double compassY )
    {
        activeDockZone = ToDockZone( zone );
        activeDockTargetName = targetName;
        dockCompassX = compassX;
        dockCompassY = compassY;
        draggingPanelName = panelName;

        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Completes a panel drag operation and docks the panel in the selected zone.
    /// </summary>
    /// <param name="panelName">The dragged panel name.</param>
    /// <param name="targetName">The panel currently under the pointer.</param>
    /// <param name="zone">The selected drop zone.</param>
    /// <returns>A task that completes after the state is updated.</returns>
    [JSInvokable]
    public async Task NotifyDockPanelDropped( string panelName, string targetName, string zone )
    {
        DockZone? targetZone = ToDockZone( zone );
        DockPanelState panelState = FindPanelState( panelName );
        bool moveGroup = draggingPanelGroup;

        draggingPanelName = null;
        activeDockZone = null;
        activeDockTargetName = null;
        draggingPanelGroup = false;

        if ( panelState is not null && targetZone is not null )
        {
            DockNodeState sourceTabsNode = moveGroup ? FindTabsNode( CurrentState.Root, panelName ) : null;
            IReadOnlyList<string> movedPanelNames = sourceTabsNode?.Panels.Count > 1
                ? sourceTabsNode.Panels.ToArray()
                : new[] { panelName };
            DockPanelState targetState = FindPanelState( targetName );
            bool targetExists = !string.IsNullOrWhiteSpace( targetName ) && !movedPanelNames.Contains( targetName ) && ContainsPanel( CurrentState.Root, targetName );
            bool mergeWithTargetTabs = targetState is not null && targetState.Position != panelState.Position;
            DockPanelPosition? targetPosition = targetZone == DockZone.Center || mergeWithTargetTabs
                ? targetState?.Position
                : ToDockPanelPosition( targetZone.Value );

            if ( targetPosition is not null )
            {
                foreach ( string movedPanelName in movedPanelNames )
                {
                    DockPanelState movedPanelState = FindPanelState( movedPanelName );

                    if ( movedPanelState is not null )
                        movedPanelState.Position = targetPosition.Value;
                }
            }

            if ( sourceTabsNode is not null && sourceTabsNode.Panels.Count > 1 )
                MoveNodeToZone( sourceTabsNode, movedPanelNames, targetName, targetZone.Value, mergeWithTargetTabs, targetExists );
            else
                MovePanelToZone( panelName, targetName, targetZone.Value, mergeWithTargetTabs );

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

    private DockPanelState EnsurePanelState( DockPanel panel )
    {
        DockPanelState panelState = FindPanelState( panel.ResolvedName );

        if ( panelState is not null )
            return panelState;

        panelState = new()
        {
            Name = panel.ResolvedName,
            Position = panel.Dock,
            Size = panel.Size,
            Collapsed = panel.Collapsed,
            AutoHide = panel.AutoHide,
            Visible = panel.Visible,
            Order = CurrentState.Panels.Count,
        };

        CurrentState.Panels.Add( panelState );

        return panelState;
    }

    private void EnsureActivePanel( DockPanelPosition position )
    {
        if ( CurrentState.ActivePanels.ContainsKey( position ) )
            return;

        DockPanelState panelState = CurrentState.Panels
            .Where( x => x.Visible && x.Position == position )
            .OrderBy( x => x.Order )
            .FirstOrDefault();

        if ( panelState is not null )
            CurrentState.ActivePanels[position] = panelState.Name;
    }

    private DockPanelState FindPanelState( string panelName )
        => CurrentState.Panels.FirstOrDefault( x => x.Name == panelName );

    private void SetPanelAutoHide( string panelName, bool autoHide )
    {
        DockPanelState panelState = FindPanelState( panelName );

        if ( panelState is not null )
            panelState.AutoHide = autoHide;
    }

    private static DockNodeState FindTabsNode( DockNodeState node, string panelName )
    {
        if ( node is null || string.IsNullOrWhiteSpace( panelName ) )
            return null;

        if ( node.Kind == DockNodeKind.Tabs && node.Panels.Contains( panelName ) )
            return node;

        if ( node.Kind == DockNodeKind.Split )
            return FindTabsNode( node.First, panelName ) ?? FindTabsNode( node.Second, panelName );

        return null;
    }

    private string GetDockNodeTrackSize( DockNodeState node, DockSplitOrientation orientation )
    {
        DockPanel panel = GetDockNodePanel( node );

        if ( panel is null || !IsPanelPositionCompatibleWithOrientation( panel.EffectivePosition, orientation ) )
            return null;

        DockPanelState panelState = FindPanelState( panel.ResolvedName );

        if ( panelState?.Visible == false )
            return null;

        if ( panelState?.AutoHide == true )
            return AutoHidePanelSize;

        if ( panelState?.Collapsed == true )
            return CollapsedPanelSize;

        if ( node.Kind == DockNodeKind.Tabs && !string.IsNullOrWhiteSpace( node.Size ) )
            return node.Size;

        return panelState?.Size ?? panel.Size ?? DefaultPanelSize;
    }

    private DockPanel GetDockNodePanel( DockNodeState node )
    {
        if ( node is null )
            return null;

        string panelName = node.Kind switch
        {
            DockNodeKind.Panel => node.PanelName,
            DockNodeKind.Tabs => GetActiveTabPanelName( node ),
            _ => null,
        };

        return !string.IsNullOrWhiteSpace( panelName ) && panels.TryGetValue( panelName, out DockPanel panel )
            ? panel
            : null;
    }

    private static bool IsPanelPositionCompatibleWithOrientation( DockPanelPosition position, DockSplitOrientation orientation )
        => orientation == DockSplitOrientation.Horizontal
            ? position is DockPanelPosition.Left or DockPanelPosition.Right
            : position is DockPanelPosition.Top or DockPanelPosition.Bottom;

    private static string GetFlexibleSplitTrack( double ratio )
    {
        double trackRatio = ratio > 0 && ratio < 1 ? ratio : 0.5;

        return $"minmax(0,{trackRatio.ToString( CultureInfo.InvariantCulture )}fr)";
    }

    private DockNodeState BuildInitialRoot()
    {
        if ( rootCollector.Nodes.Count == 1 && rootCollector.Nodes[0].Kind is DockNodeKind.Split or DockNodeKind.Tabs )
            return rootCollector.Nodes[0];

        DockNodeState center = rootCollector.Nodes.FirstOrDefault( x => x.Kind == DockNodeKind.Content ) ?? new() { Kind = DockNodeKind.Content };
        DockNodeState left = BuildSimpleDockNode( DockPanelPosition.Left );
        DockNodeState right = BuildSimpleDockNode( DockPanelPosition.Right );
        DockNodeState top = BuildSimpleDockNode( DockPanelPosition.Top );
        DockNodeState bottom = BuildSimpleDockNode( DockPanelPosition.Bottom );
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

    private DockNodeState BuildSimpleDockNode( DockPanelPosition position )
    {
        List<string> panelNames = panels.Values
            .Where( x => x.Dock == position )
            .OrderBy( x => FindPanelState( x.ResolvedName )?.Order ?? 0 )
            .Select( x => x.ResolvedName )
            .ToList();

        if ( panelNames.Count == 0 )
            return null;

        if ( panelNames.Count == 1 )
        {
            return new()
            {
                Kind = DockNodeKind.Panel,
                PanelName = panelNames[0],
            };
        }

        return new()
        {
            Kind = DockNodeKind.Tabs,
            Panels = panelNames,
            ActivePanel = panelNames[0],
            Size = GetDockGroupSize( panelNames ),
        };
    }

    private void MoveNodeToZone( DockNodeState movingNode, IReadOnlyList<string> movingPanelNames, string targetName, DockZone zone, bool mergeWithTargetTabs, bool targetExists )
    {
        DockNodeState originalRoot = CurrentState.Root;

        CurrentState.Root = RemoveTabsNode( CurrentState.Root, movingNode );

        if ( ( zone == DockZone.Center || mergeWithTargetTabs ) && targetExists )
        {
            foreach ( string movingPanelName in movingPanelNames )
                CurrentState.Root = AddPanelToTabs( CurrentState.Root, targetName, movingPanelName );

            return;
        }

        if ( targetExists )
        {
            CurrentState.Root = SplitTargetPanel( CurrentState.Root, targetName, movingNode, zone );
            return;
        }

        DockPanelPosition? position = ToDockPanelPosition( zone );

        if ( position is null )
        {
            CurrentState.Root = originalRoot;
            return;
        }

        CurrentState.Root = position.Value switch
        {
            DockPanelPosition.Left => CreateSplitNode( movingNode, CurrentState.Root, DockSplitOrientation.Horizontal, 0.22 ),
            DockPanelPosition.Right => CreateSplitNode( CurrentState.Root, movingNode, DockSplitOrientation.Horizontal, 0.78 ),
            DockPanelPosition.Top => CreateSplitNode( movingNode, CurrentState.Root, DockSplitOrientation.Vertical, 0.22 ),
            DockPanelPosition.Bottom => CreateSplitNode( CurrentState.Root, movingNode, DockSplitOrientation.Vertical, 0.78 ),
            _ => CurrentState.Root,
        };
    }

    private void MovePanelToZone( string panelName, string targetName, DockZone zone, bool mergeWithTargetTabs )
    {
        DockNodeState originalRoot = CurrentState.Root;
        bool targetExists = !string.IsNullOrWhiteSpace( targetName ) && targetName != panelName && ContainsPanel( CurrentState.Root, targetName );

        CurrentState.Root = RemovePanelNode( CurrentState.Root, panelName );

        DockNodeState panelNode = new()
        {
            Kind = DockNodeKind.Panel,
            PanelName = panelName,
        };

        if ( ( zone == DockZone.Center || mergeWithTargetTabs ) && targetExists )
        {
            CurrentState.Root = AddPanelToTabs( CurrentState.Root, targetName, panelName );
            return;
        }

        if ( targetExists )
        {
            CurrentState.Root = SplitTargetPanel( CurrentState.Root, targetName, panelNode, zone );
            return;
        }

        DockPanelPosition? position = ToDockPanelPosition( zone );

        if ( position is null )
        {
            CurrentState.Root = originalRoot;
            return;
        }

        CurrentState.Root = position.Value switch
        {
            DockPanelPosition.Left => CreateSplitNode( panelNode, CurrentState.Root, DockSplitOrientation.Horizontal, 0.22 ),
            DockPanelPosition.Right => CreateSplitNode( CurrentState.Root, panelNode, DockSplitOrientation.Horizontal, 0.78 ),
            DockPanelPosition.Top => CreateSplitNode( panelNode, CurrentState.Root, DockSplitOrientation.Vertical, 0.22 ),
            DockPanelPosition.Bottom => CreateSplitNode( CurrentState.Root, panelNode, DockSplitOrientation.Vertical, 0.78 ),
            _ => CurrentState.Root,
        };
    }

    private static bool ContainsPanel( DockNodeState node, string panelName )
    {
        if ( node is null )
            return false;

        return node.Kind switch
        {
            DockNodeKind.Panel => node.PanelName == panelName,
            DockNodeKind.Tabs => node.Panels.Contains( panelName ),
            DockNodeKind.Split => ContainsPanel( node.First, panelName ) || ContainsPanel( node.Second, panelName ),
            _ => false,
        };
    }

    private DockNodeState AddPanelToTabs( DockNodeState node, string targetName, string panelName )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Panel && node.PanelName == targetName )
        {
            return new()
            {
                Kind = DockNodeKind.Tabs,
                Panels = { targetName, panelName },
                ActivePanel = panelName,
                Size = GetDockPanelSize( targetName ),
            };
        }

        if ( node.Kind == DockNodeKind.Tabs && node.Panels.Contains( targetName ) )
        {
            node.Size ??= GetDockPanelSize( targetName );

            if ( !node.Panels.Contains( panelName ) )
                node.Panels.Add( panelName );

            node.ActivePanel = panelName;

            return node;
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            node.First = AddPanelToTabs( node.First, targetName, panelName );
            node.Second = AddPanelToTabs( node.Second, targetName, panelName );
        }

        return node;
    }

    private string GetDockGroupSize( IEnumerable<string> panelNames )
    {
        foreach ( string panelName in panelNames )
        {
            string panelSize = GetDockPanelSize( panelName );

            if ( !string.IsNullOrWhiteSpace( panelSize ) )
                return panelSize;
        }

        return DefaultPanelSize;
    }

    private string GetDockPanelSize( string panelName )
    {
        if ( string.IsNullOrWhiteSpace( panelName ) || !panels.TryGetValue( panelName, out DockPanel panel ) )
            return null;

        DockPanelState panelState = FindPanelState( panelName );

        return panelState?.Size ?? panel.Size;
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

    private static DockNodeState SplitTargetPanel( DockNodeState node, string targetName, DockNodeState panelNode, DockZone zone )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Panel && node.PanelName == targetName )
            return CreateTargetSplitNode( node, panelNode, zone );

        if ( node.Kind == DockNodeKind.Tabs && node.Panels.Contains( targetName ) )
            return CreateTargetSplitNode( node, panelNode, zone );

        if ( node.Kind == DockNodeKind.Split )
        {
            node.First = SplitTargetPanel( node.First, targetName, panelNode, zone );
            node.Second = SplitTargetPanel( node.Second, targetName, panelNode, zone );
        }

        return node;
    }

    private static DockNodeState CreateTargetSplitNode( DockNodeState targetNode, DockNodeState panelNode, DockZone zone )
        => zone switch
        {
            DockZone.Left => CreateSplitNode( panelNode, targetNode, DockSplitOrientation.Horizontal, 0.32 ),
            DockZone.Right => CreateSplitNode( targetNode, panelNode, DockSplitOrientation.Horizontal, 0.68 ),
            DockZone.Top => CreateSplitNode( panelNode, targetNode, DockSplitOrientation.Vertical, 0.32 ),
            DockZone.Bottom => CreateSplitNode( targetNode, panelNode, DockSplitOrientation.Vertical, 0.68 ),
            _ => targetNode,
        };

    private static DockNodeState RemovePanelNode( DockNodeState node, string panelName )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Panel )
            return node.PanelName == panelName ? null : node;

        if ( node.Kind == DockNodeKind.Tabs )
        {
            node.Panels.Remove( panelName );

            if ( node.Panels.Count == 0 )
                return null;

            if ( node.ActivePanel == panelName )
                node.ActivePanel = node.Panels[0];

            return node.Panels.Count == 1
                ? new()
                {
                    Kind = DockNodeKind.Panel,
                    PanelName = node.Panels[0],
                }
                : node;
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            DockNodeState first = RemovePanelNode( node.First, panelName );
            DockNodeState second = RemovePanelNode( node.Second, panelName );

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

    private string GetRenderedPanelClass( DockPanel panel, DockPanelState panelState )
    {
        ClassBuilder builder = new( classBuilder =>
        {
            bool collapsed = panelState?.Collapsed == true || panelState?.AutoHide == true;

            classBuilder.Append( ClassProvider.DockPanel( panel.EffectivePosition, panel.Resizable, collapsed ) );
            classBuilder.Append( ClassProvider.DockPanelPosition( panel.EffectivePosition ) );
            classBuilder.Append( ClassProvider.DockPanelResizable( panel.Resizable ) );
            classBuilder.Append( ClassProvider.DockPanelCollapsed( collapsed ) );
            classBuilder.Append( ClassProvider.DockPanelAutoHide( panelState?.AutoHide == true ) );
        } );

        return builder.Class;
    }

    private string GetRenderedTabsHostClass( DockPanel panel, DockPanelState panelState )
    {
        ClassBuilder builder = new( classBuilder =>
        {
            classBuilder.Append( GetRenderedPanelClass( panel, panelState ) );
            classBuilder.Append( "dock-tabs-host" );
        } );

        return builder.Class;
    }

    private string GetRenderedPanelAutoHideTabClass( DockPanel panel )
        => ClassProvider.DockPanelAutoHideTab( panel.EffectivePosition );

    private string GetRenderedPanelStyle( DockPanel panel, DockPanelState panelState, string size = null )
    {
        string panelSize = size ?? panelState?.Size ?? panel.Size;
        bool autoHide = panelState?.AutoHide == true;

        StyleBuilder builder = new( styleBuilder =>
        {
            styleBuilder.Append( $"--dock-panel-size:{panelSize}", !autoHide && !string.IsNullOrWhiteSpace( panelSize ) );
            styleBuilder.Append( $"--dock-panel-min-size:{panel.MinSize}", !autoHide && !string.IsNullOrWhiteSpace( panel.MinSize ) );
            styleBuilder.Append( $"--dock-panel-max-size:{panel.MaxSize}", !autoHide && !string.IsNullOrWhiteSpace( panel.MaxSize ) );
        } );

        return builder.Styles;
    }

    private int GetNextOrder( DockPanelPosition position )
    {
        int? order = CurrentState.Panels
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

    private string GetDockCompassStyle()
    {
        StyleBuilder builder = new( styleBuilder =>
        {
            styleBuilder.Append( $"left:{dockCompassX}px" );
            styleBuilder.Append( $"top:{dockCompassY}px" );
        } );

        return builder.Styles;
    }

    internal string GetDockCompassClass()
        => ClassProvider.DockLayoutCompass();

    internal string GetDockCompassZoneClass( DockZone zone )
        => ClassProvider.DockLayoutCompassZone( zone, activeDockZone == zone );

    internal string GetDockShellGuideClass( DockZone zone )
        => ClassProvider.DockLayoutShellGuide( zone, activeDockTargetName is null && activeDockZone == zone );

    private static DockPanelPosition? ToDockPanelPosition( DockZone zone )
        => zone switch
        {
            DockZone.Left => DockPanelPosition.Left,
            DockZone.Right => DockPanelPosition.Right,
            DockZone.Top => DockPanelPosition.Top,
            DockZone.Bottom => DockPanelPosition.Bottom,
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

    /// <summary>
    /// Specifies the panels and content to be rendered inside the dock layout.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Defines the mutable state used for docking, resizing, active tabs, and panel visibility.
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
    /// Gets the DockLayout JavaScript module.
    /// </summary>
    [Inject] protected IJSDockLayoutModule JSModule { get; set; }

    internal bool DockGuidesVisible => draggingPanelName is not null && activeDockZone is not null;

    internal static IReadOnlyList<DockZone> DockZones => dockZones;

    private DockLayoutState CurrentState => state ??= new();

    internal DockNodeCollector RootCollector => rootCollector;

    private DotNetObjectReference<DockLayout> DotNetObjectRef => dotNetObjectRef ??= DotNetObjectReference.Create( this );

    #endregion
}