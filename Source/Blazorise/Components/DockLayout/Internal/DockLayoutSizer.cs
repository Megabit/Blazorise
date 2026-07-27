#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise;

internal sealed class DockLayoutSizer
{
    #region Members

    private const string DefaultPaneSize = "16rem";

    private const string AutoHidePaneSize = "2rem";

    private const string CollapsedPaneSize = "2.5rem";

    private const string DefaultMinimumPaneSize = "2rem";

    private const string SplitGapSize = "var(--dock-split-gap, 0px)";

    private const string FlexibleFillTrack = "minmax(0,1fr)";

    private readonly DockLayoutRegistry registry;

    private readonly DockLayoutStateManager stateManager;

    private readonly DockLayoutTreeQuery query;

    private readonly Func<DockLayoutState> getState;

    #endregion

    #region Constructors

    public DockLayoutSizer( DockLayoutRegistry registry, DockLayoutStateManager stateManager, DockLayoutTreeQuery query, Func<DockLayoutState> getState )
    {
        this.registry = registry;
        this.stateManager = stateManager;
        this.query = query;
        this.getState = getState;
    }

    #endregion

    #region Methods

    public string GetDockSplitStyle( DockNodeState node )
    {
        if ( node is null || node.Kind != DockNodeKind.Split )
            return null;

        string firstFixedTrack = node.UseRatio ? null : GetDockNodeTrackSize( node.First, node.Orientation );
        string secondFixedTrack = node.UseRatio ? null : GetDockNodeTrackSize( node.Second, node.Orientation );
        string firstTrack = firstFixedTrack ?? ( secondFixedTrack is not null ? FlexibleFillTrack : GetFlexibleSplitTrack( node.Ratio ) );
        string secondTrack = secondFixedTrack ?? ( firstFixedTrack is not null ? FlexibleFillTrack : GetFlexibleSplitTrack( 1d - node.Ratio ) );

        return node.Orientation == DockSplitOrientation.Vertical
            ? $"--dock-split-start-size:{firstTrack};--dock-split-end-size:{secondTrack};grid-template-rows:var(--dock-split-start-size) var(--dock-split-end-size);"
            : $"--dock-split-start-size:{firstTrack};--dock-split-end-size:{secondTrack};grid-template-columns:var(--dock-split-start-size) var(--dock-split-end-size);";
    }

    public string GetDockGroupSize( DockLayoutState state, IEnumerable<string> paneNames )
    {
        foreach ( string paneName in paneNames )
        {
            string paneSize = GetDockPaneSize( state, paneName );

            if ( !string.IsNullOrWhiteSpace( paneSize ) )
                return paneSize;
        }

        return DefaultPaneSize;
    }

    public string GetDockNodeMinimumSize( DockNodeState node, DockSplitOrientation resizeOrientation )
    {
        if ( node?.Kind != DockNodeKind.Split )
            return GetDockPaneMinimumSize( node );

        string firstMinimum = GetDockChildMinimumSize( node, node.First, resizeOrientation );
        string secondMinimum = GetDockChildMinimumSize( node, node.Second, resizeOrientation );

        return node.Orientation == resizeOrientation
            ? $"calc({firstMinimum} + {secondMinimum} + {SplitGapSize})"
            : $"max({firstMinimum}, {secondMinimum})";
    }

    public bool IsCenterDockGroup( DockLayoutState state, IEnumerable<string> paneNames )
        => paneNames.Any( paneName => IsCenterDockPane( state, paneName ) );

    public string GetDockNodeSize( DockLayoutState state, DockNodeState node )
        => node?.Kind switch
        {
            DockNodeKind.Pane => GetDockPaneSize( state, node.PaneName ),
            DockNodeKind.Tabs => node.Size ?? GetDockGroupSize( state, node.Panes ),
            DockNodeKind.Split => node.Size,
            _ => null,
        };

    public string GetResolvedDockNodeSize( DockLayoutState state, DockNodeState node )
    {
        string size = GetDockNodeSize( state, node );

        if ( !string.IsNullOrWhiteSpace( size ) )
            return size;

        DockPanePosition? position = query.GetDockNodePosition( node );

        return position is null or DockPanePosition.Center
            ? null
            : GetDefaultDockPaneSize( position.Value );
    }

    public string GetDockPaneSize( DockLayoutState state, string paneName )
    {
        if ( !registry.TryGetPane( paneName, out DockPane pane ) )
            return null;

        DockPaneState paneState = stateManager.FindPaneState( state, paneName );

        return paneState?.Size ?? pane.Size;
    }

    private string GetDockNodeTrackSize( DockNodeState node, DockSplitOrientation orientation )
    {
        DockPanePosition? position = query.GetDockNodePosition( node );

        if ( position is null || !IsPanePositionCompatibleWithOrientation( position.Value, orientation ) )
            return null;

        if ( node?.Kind == DockNodeKind.Split && !string.IsNullOrWhiteSpace( node.Size ) )
            return node.Size;

        DockPane pane = GetDockNodePane( node );

        if ( pane is null )
            return null;

        DockPaneState paneState = stateManager.FindPaneState( getState(), pane.ResolvedName );

        if ( paneState?.Visible == false )
            return null;

        if ( paneState?.AutoHide == true )
            return AutoHidePaneSize;

        if ( paneState?.Collapsed == true )
            return CollapsedPaneSize;

        if ( !string.IsNullOrWhiteSpace( node.Size ) )
            return node.Size;

        return paneState?.Size ?? pane.Size ?? GetDefaultDockPaneSize( position.Value );
    }

    private string GetDockChildMinimumSize( DockNodeState parent, DockNodeState child, DockSplitOrientation resizeOrientation )
    {
        if ( parent?.UseRatio == false && parent.Orientation == resizeOrientation )
        {
            string fixedTrackSize = GetDockNodeTrackSize( child, resizeOrientation );

            if ( !string.IsNullOrWhiteSpace( fixedTrackSize ) && !string.Equals( fixedTrackSize, "auto", StringComparison.OrdinalIgnoreCase ) )
                return fixedTrackSize;
        }

        return GetDockNodeMinimumSize( child, resizeOrientation );
    }

    private string GetDockPaneMinimumSize( DockNodeState node )
    {
        DockPane pane = GetDockNodePane( node );

        return string.IsNullOrWhiteSpace( pane?.MinSize )
            ? DefaultMinimumPaneSize
            : pane.MinSize;
    }

    private DockPane GetDockNodePane( DockNodeState node )
    {
        if ( node is null )
            return null;

        string paneName = node.Kind switch
        {
            DockNodeKind.Pane => node.PaneName,
            DockNodeKind.Tabs => query.GetActiveTabPaneName( node ),
            DockNodeKind.Split => query.GetFirstDockNodePaneName( node ),
            _ => null,
        };

        return registry.TryGetPane( paneName, out DockPane pane ) ? pane : null;
    }

    private bool IsCenterDockPane( DockLayoutState state, string paneName )
    {
        if ( registry.TryGetPane( paneName, out DockPane pane ) && IsCenterPane( pane ) )
            return true;

        return stateManager.FindPaneState( state, paneName )?.Position == DockPanePosition.Center;
    }

    private static bool IsCenterPane( DockPane pane )
        => pane is not null && ( pane.Role == DockRole.Document || pane.PanePosition == DockPanePosition.Center );

    private static string GetDefaultDockPaneSize( DockPanePosition position )
        => position == DockPanePosition.Top || position == DockPanePosition.Bottom
            ? "auto"
            : DefaultPaneSize;

    private static bool IsPanePositionCompatibleWithOrientation( DockPanePosition position, DockSplitOrientation orientation )
        => orientation == DockSplitOrientation.Horizontal
            ? position is DockPanePosition.Left or DockPanePosition.Right
            : position is DockPanePosition.Top or DockPanePosition.Bottom;

    private static string GetFlexibleSplitTrack( double ratio )
    {
        double trackRatio = ratio > 0 && ratio < 1 ? ratio : 0.5;

        return $"minmax(0,{trackRatio.ToString( CultureInfo.InvariantCulture )}fr)";
    }

    #endregion
}