#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise;

internal sealed class DockLayoutRegistry
{
    #region Members

    private readonly Dictionary<string, DockPane> panes = new();

    private readonly Dictionary<string, int> paneDefinitionVersions = new();

    private int contentDefinitionVersion;

    #endregion

    #region Methods

    public bool RegisterPane( DockPane pane )
    {
        string paneName = pane?.ResolvedName;

        if ( string.IsNullOrWhiteSpace( paneName ) )
            return false;

        bool changed = !panes.TryGetValue( paneName, out DockPane registeredPane )
            || !ReferenceEquals( registeredPane, pane )
            || !paneDefinitionVersions.TryGetValue( paneName, out int registeredDefinitionVersion )
            || registeredDefinitionVersion != pane.DefinitionVersion;

        panes[paneName] = pane;
        paneDefinitionVersions[paneName] = pane.DefinitionVersion;

        return changed;
    }

    public bool RegisterContent( DockContent content )
    {
        if ( content is null )
            return false;

        bool changed = !ReferenceEquals( Content, content ) || contentDefinitionVersion != content.DefinitionVersion;

        Content = content;
        contentDefinitionVersion = content.DefinitionVersion;

        return changed;
    }

    public void UnregisterPane( DockPane pane )
    {
        string paneName = pane?.ResolvedName;

        if ( string.IsNullOrWhiteSpace( paneName ) )
            return;

        panes.Remove( paneName );
        paneDefinitionVersions.Remove( paneName );
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