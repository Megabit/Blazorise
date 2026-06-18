namespace Blazorise;

/// <summary>
/// Represents the mutable state for a docked pane.
/// </summary>
public class DockPaneState
{
    /// <summary>
    /// Identifies the pane inside the parent <see cref="DockLayout"/>.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Defines where the pane is currently docked.
    /// </summary>
    public DockPanePosition Position { get; set; }

    /// <summary>
    /// Defines the current pane size, such as <c>280px</c>, <c>18rem</c>, or <c>25%</c>.
    /// </summary>
    public string Size { get; set; }

    /// <summary>
    /// Indicates whether the pane content is collapsed.
    /// </summary>
    public bool Collapsed { get; set; }

    /// <summary>
    /// Indicates whether the pane is pinned to its docked side or auto-hidden.
    /// </summary>
    public bool AutoHide { get; set; }

    /// <summary>
    /// Indicates whether the pane is visible in the layout.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the order of the pane within its docked side.
    /// </summary>
    public int Order { get; set; }
}