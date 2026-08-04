#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise;

/// <summary>
/// Provides data for the <see cref="DockPane.Closing"/> event.
/// </summary>
public class DockPaneClosingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new <see cref="DockPaneClosingEventArgs"/>.
    /// </summary>
    /// <param name="cancel">True if closing should be cancelled.</param>
    /// <param name="paneName">The pane name.</param>
    public DockPaneClosingEventArgs( bool cancel, string paneName )
        : base( cancel )
    {
        PaneName = paneName;
    }

    /// <summary>
    /// Gets the pane name.
    /// </summary>
    public string PaneName { get; }
}