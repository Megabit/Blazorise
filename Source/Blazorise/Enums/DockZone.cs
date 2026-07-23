namespace Blazorise;

/// <summary>
/// Defines the available docking zones shown while a pane is being moved.
/// </summary>
public enum DockZone
{
    /// <summary>
    /// Tabs the dragged pane with the target pane.
    /// </summary>
    Center,

    /// <summary>
    /// Docks the dragged pane to the left side of the target.
    /// </summary>
    Left,

    /// <summary>
    /// Docks the dragged pane above the target.
    /// </summary>
    Top,

    /// <summary>
    /// Docks the dragged pane to the right side of the target.
    /// </summary>
    Right,

    /// <summary>
    /// Docks the dragged pane below the target.
    /// </summary>
    Bottom,
}