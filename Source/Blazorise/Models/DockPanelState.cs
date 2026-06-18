namespace Blazorise;

/// <summary>
/// Represents the mutable state for a docked panel.
/// </summary>
public class DockPanelState
{
    /// <summary>
    /// Identifies the panel inside the parent <see cref="DockLayout"/>.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Defines where the panel is currently docked.
    /// </summary>
    public DockPanelPosition Position { get; set; }

    /// <summary>
    /// Defines the current panel size, such as <c>280px</c>, <c>18rem</c>, or <c>25%</c>.
    /// </summary>
    public string Size { get; set; }

    /// <summary>
    /// Indicates whether the panel content is collapsed.
    /// </summary>
    public bool Collapsed { get; set; }

    /// <summary>
    /// Indicates whether the panel is pinned to its docked side or auto-hidden.
    /// </summary>
    public bool AutoHide { get; set; }

    /// <summary>
    /// Indicates whether the panel is visible in the layout.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the order of the panel within its docked side.
    /// </summary>
    public int Order { get; set; }
}