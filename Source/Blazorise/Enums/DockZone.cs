namespace Blazorise;

/// <summary>
/// Defines the available docking zones shown while a panel is being moved.
/// </summary>
public enum DockZone
{
    /// <summary>
    /// Tabs the dragged panel with the target panel.
    /// </summary>
    Center,

    /// <summary>
    /// Docks the dragged panel to the left side of the target.
    /// </summary>
    Left,

    /// <summary>
    /// Docks the dragged panel above the target.
    /// </summary>
    Top,

    /// <summary>
    /// Docks the dragged panel to the right side of the target.
    /// </summary>
    Right,

    /// <summary>
    /// Docks the dragged panel below the target.
    /// </summary>
    Bottom,
}