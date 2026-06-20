#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise;

internal sealed class DockLayoutRegistry
{
    #region Members

    private readonly Dictionary<string, DockPane> panes = new();

    #endregion

    #region Methods

    public bool RegisterPane( DockPane pane )
    {
        string paneName = pane?.ResolvedName;

        if ( string.IsNullOrWhiteSpace( paneName ) )
            return false;

        panes[paneName] = pane;

        return true;
    }

    public void RegisterContent( DockContent content )
    {
        Content = content;
    }

    public void UnregisterPane( DockPane pane )
    {
        string paneName = pane?.ResolvedName;

        if ( string.IsNullOrWhiteSpace( paneName ) )
            return;

        panes.Remove( paneName );
    }

    public bool TryGetPane( string paneName, out DockPane pane )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) )
        {
            pane = null;
            return false;
        }

        return panes.TryGetValue( paneName, out pane );
    }

    #endregion

    #region Properties

    public DockNodeCollector RootCollector { get; } = new();

    public DockContent Content { get; private set; }

    public IEnumerable<DockPane> RegisteredPanes => panes.Values;

    public IReadOnlyDictionary<string, DockPane> Panes => panes;

    #endregion
}