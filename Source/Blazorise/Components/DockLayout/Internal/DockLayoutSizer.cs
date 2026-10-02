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

    private const string FlexibleFillTrack = "minmax(0,1fr)";

    private readonly DockLayoutRegistry registry;

    private readonly DockLayoutStateManager stateManager;

    private readonly DockLayoutTreeQuery query;

    private readonly Func<DockLayoutState> getState;

    private readonly Func<string, string> getVariableName;

    #endregion

    #region Constructors

    public DockLayoutSizer( DockLayoutRegistry registry, DockLayoutStateManager stateManager, DockLayoutTreeQuery query, Func<DockLayoutState> getState, Func<string, string> getVariableName = null )
    {
        this.registry = registry;
        this.stateManager = stateManager;
        this.query = query;
        this.getState = getState;
        this.getVariableName = getVariableName ?? ( name => $"--dock-{name}" );
    }

    #endregion

    #region Methods

    public string GetDockSplitStyle( DockNodeState node )
    {
        if ( node is null || node.Kind != DockNodeKind.Split )
        {
            return null;
        }

        var firstFixedTrack = node.UseRatio ? null : GetDockNodeTrackSize( node.First, node.Orientation );
        var secondFixedTrack = node.UseRatio ? null : GetDockNodeTrackSize( node.Second, node.Orientation );
        var firstTrack = firstFixedTrack ?? ( secondFixedTrack is not null ? FlexibleFillTrack : GetFlexibleSplitTrack( node.Ratio ) );
        var secondTrack = secondFixedTrack ?? ( firstFixedTrack is not null ? FlexibleFillTrack : GetFlexibleSplitTrack( 1d - node.Ratio ) );

        var startVariable = getVariableName( "split-start-size" );
        var endVariable = getVariableName( "split-end-size" );
        var gridProperty = node.Orientation == Orientation.Vertical ? "grid-template-rows" : "grid-template-columns";

        return $"{startVariable}:{firstTrack};{endVariable}:{secondTrack};{gridProperty}:var({startVariable}) var({endVariable});";
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

    public string GetDockNodeMinimumSize( DockNodeState node, Orientation resizeOrientation )
    {
        if ( node?.Kind != DockNodeKind.Split )
        {
            return GetDockPaneMinimumSize( node, resizeOrientation );
        }

        var firstMinimum = GetDockChildMinimumSize( node, node.First, resizeOrientation );
        var secondMinimum = GetDockChildMinimumSize( node, node.Second, resizeOrientation );

        var splitGap = $"var({getVariableName( "split-gap" )}, 0px)";

        return node.Orientation == resizeOrientation
            ? $"calc({firstMinimum} + {secondMinimum} + {splitGap})"
            : $"max({firstMinimum}, {secondMinimum})";
    }

    public string GetDockNodeMaximumSize( DockNodeState node, Orientation resizeOrientation )
    {
        if ( node?.Kind == DockNodeKind.Split )
            return null;

        DockPane pane = GetDockNodePane( node );

        return IsPaneSizeConstraintApplicable( node, resizeOrientation )
            ? pane?.MaxSize
            : null;
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

        return paneState is not null ? paneState.Size : pane.Size;
    }

    private string GetDockNodeTrackSize( DockNodeState node, Orientation orientation )
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

        string paneSize = paneState is not null ? paneState.Size : pane.Size;

        return paneSize ?? GetDefaultDockPaneSize( position.Value );
    }

    private string GetDockChildMinimumSize( DockNodeState parent, DockNodeState child, Orientation resizeOrientation )
    {
        if ( parent?.UseRatio == false && parent.Orientation == resizeOrientation )
        {
            string fixedTrackSize = GetDockNodeTrackSize( child, resizeOrientation );

            if ( !string.IsNullOrWhiteSpace( fixedTrackSize ) && !string.Equals( fixedTrackSize, "auto", StringComparison.OrdinalIgnoreCase ) )
                return fixedTrackSize;
        }

        return GetDockNodeMinimumSize( child, resizeOrientation );
    }

    private string GetDockPaneMinimumSize( DockNodeState node, Orientation resizeOrientation )
    {
        DockPane pane = GetDockNodePane( node );

        return !IsPaneSizeConstraintApplicable( node, resizeOrientation ) || string.IsNullOrWhiteSpace( pane?.MinSize )
            ? DefaultMinimumPaneSize
            : pane.MinSize;
    }

    private bool IsPaneSizeConstraintApplicable( DockNodeState node, Orientation resizeOrientation )
    {
        DockPanePosition? position = query.GetDockNodePosition( node );

        return position == DockPanePosition.Center
            || position is not null && IsPanePositionCompatibleWithOrientation( position.Value, resizeOrientation );
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
        if ( registry.TryGetPane( paneName, out DockPane pane ) && pane.Role == DockPaneRole.Document )
            return true;

        DockPaneState paneState = stateManager.FindPaneState( state, paneName );

        return paneState is not null
            ? paneState.Position == DockPanePosition.Center
            : pane?.PanePosition == DockPanePosition.Center;
    }

    private static string GetDefaultDockPaneSize( DockPanePosition position )
        => position == DockPanePosition.Top || position == DockPanePosition.Bottom
            ? "auto"
            : DefaultPaneSize;

    private static bool IsPanePositionCompatibleWithOrientation( DockPanePosition position, Orientation orientation )
        => orientation == Orientation.Horizontal
            ? position is DockPanePosition.Left or DockPanePosition.Right
            : position is DockPanePosition.Top or DockPanePosition.Bottom;

    private static string GetFlexibleSplitTrack( double ratio )
    {
        double trackRatio = ratio > 0 && ratio < 1 ? ratio : 0.5;

        return $"minmax(0,{trackRatio.ToString( CultureInfo.InvariantCulture )}fr)";
    }

    #endregion
}