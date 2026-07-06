#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise;

internal static class DockLayoutNormalizer
{
    #region Methods

    public static DockNodeState Normalize( DockNodeState root, IReadOnlyDictionary<string, DockPane> panes, IReadOnlyList<DockPaneState> paneStates )
    {
        HashSet<string> usedPaneNames = new();

        return NormalizeNode( root, panes, paneStates, usedPaneNames );
    }

    private static DockNodeState NormalizeNode( DockNodeState node, IReadOnlyDictionary<string, DockPane> panes, IReadOnlyList<DockPaneState> paneStates, HashSet<string> usedPaneNames )
    {
        if ( node is null )
            return null;

        return node.Kind switch
        {
            DockNodeKind.Pane => NormalizePaneNode( node, panes, paneStates, usedPaneNames ),
            DockNodeKind.Tabs => NormalizeTabsNode( node, panes, paneStates, usedPaneNames ),
            DockNodeKind.Split => NormalizeSplitNode( node, panes, paneStates, usedPaneNames ),
            DockNodeKind.Content => node,
            _ => null,
        };
    }

    private static DockNodeState NormalizePaneNode( DockNodeState node, IReadOnlyDictionary<string, DockPane> panes, IReadOnlyList<DockPaneState> paneStates, HashSet<string> usedPaneNames )
    {
        if ( !IsPaneAvailable( node.PaneName, panes, paneStates ) )
            return null;

        if ( !usedPaneNames.Add( node.PaneName ) )
            return null;

        return node;
    }

    private static DockNodeState NormalizeTabsNode( DockNodeState node, IReadOnlyDictionary<string, DockPane> panes, IReadOnlyList<DockPaneState> paneStates, HashSet<string> usedPaneNames )
    {
        List<string> paneNames = node.Panes
            .Where( paneName => IsPaneAvailable( paneName, panes, paneStates ) )
            .Where( usedPaneNames.Add )
            .ToList();

        if ( paneNames.Count == 0 )
            return null;

        node.Panes.Clear();

        foreach ( string paneName in paneNames )
            node.Panes.Add( paneName );

        if ( string.IsNullOrWhiteSpace( node.ActivePane ) || !node.Panes.Contains( node.ActivePane ) )
            node.ActivePane = node.Panes[0];

        bool centerTabsNode = IsCenterTabsNode( node, panes, paneStates );

        if ( centerTabsNode )
            node.Size = null;

        if ( node.Panes.Count == 1 && !ShouldKeepSinglePaneTabNode( node.Panes[0], panes ) )
        {
            PreserveCollapsedTabSize( node.Panes[0], node.Size, paneStates );

            return new()
            {
                Kind = DockNodeKind.Pane,
                PaneName = node.Panes[0],
                Size = node.Size,
            };
        }

        if ( !centerTabsNode )
            node.Size ??= GetDockGroupSize( node.Panes, panes, paneStates );

        return node;
    }

    private static DockNodeState NormalizeSplitNode( DockNodeState node, IReadOnlyDictionary<string, DockPane> panes, IReadOnlyList<DockPaneState> paneStates, HashSet<string> usedPaneNames )
    {
        DockNodeState first = NormalizeNode( node.First, panes, paneStates, usedPaneNames );
        DockNodeState second = NormalizeNode( node.Second, panes, paneStates, usedPaneNames );

        if ( first is null )
            return second;

        if ( second is null )
            return first;

        node.First = first;
        node.Second = second;
        node.Ratio = node.Ratio > 0d && node.Ratio < 1d ? node.Ratio : 0.5d;

        return node;
    }

    private static bool IsPaneAvailable( string paneName, IReadOnlyDictionary<string, DockPane> panes, IReadOnlyList<DockPaneState> paneStates )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) || !panes.TryGetValue( paneName, out DockPane pane ) )
            return false;

        DockPaneState paneState = paneStates.FirstOrDefault( x => x.Name == paneName );

        return ( paneState?.Visible ?? pane.Visible )
            && paneState?.AutoHide != true;
    }

    private static bool ShouldKeepSinglePaneTabNode( string paneName, IReadOnlyDictionary<string, DockPane> panes )
        => panes.TryGetValue( paneName, out DockPane pane ) && pane.Role == DockRole.Document && pane.EffectiveShowTab;

    private static bool IsCenterTabsNode( DockNodeState node, IReadOnlyDictionary<string, DockPane> panes, IReadOnlyList<DockPaneState> paneStates )
        => node.Panes.Any( paneName => IsCenterPane( paneName, panes, paneStates ) );

    private static bool IsCenterPane( string paneName, IReadOnlyDictionary<string, DockPane> panes, IReadOnlyList<DockPaneState> paneStates )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) )
            return false;

        if ( panes.TryGetValue( paneName, out DockPane pane ) && IsCenterPane( pane ) )
            return true;

        DockPaneState paneState = paneStates.FirstOrDefault( x => x.Name == paneName );

        return paneState?.Position == DockPanePosition.Center;
    }

    private static bool IsCenterPane( DockPane pane )
        => pane is not null && ( pane.Role == DockRole.Document || pane.Dock == DockPanePosition.Center );

    private static void PreserveCollapsedTabSize( string paneName, string size, IReadOnlyList<DockPaneState> paneStates )
    {
        if ( string.IsNullOrWhiteSpace( size ) )
            return;

        DockPaneState paneState = paneStates.FirstOrDefault( x => x.Name == paneName );

        if ( paneState is not null && string.IsNullOrWhiteSpace( paneState.Size ) && IsPaneSizeValue( size ) )
            paneState.Size = size;
    }

    private static bool IsPaneSizeValue( string size )
        => size.IndexOf( "fr", StringComparison.Ordinal ) < 0
            && !size.StartsWith( "minmax(", StringComparison.OrdinalIgnoreCase );

    private static string GetDockGroupSize( IEnumerable<string> paneNames, IReadOnlyDictionary<string, DockPane> panes, IReadOnlyList<DockPaneState> paneStates )
    {
        foreach ( string paneName in paneNames )
        {
            DockPaneState paneState = paneStates.FirstOrDefault( x => x.Name == paneName );

            if ( !string.IsNullOrWhiteSpace( paneState?.Size ) )
                return paneState.Size;

            if ( panes.TryGetValue( paneName, out DockPane pane ) && !string.IsNullOrWhiteSpace( pane.Size ) )
                return pane.Size;
        }

        return null;
    }

    #endregion
}