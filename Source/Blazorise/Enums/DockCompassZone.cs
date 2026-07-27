namespace Blazorise;

/// <summary>
/// Defines the exact compass zone shown while a dock pane is being moved.
/// </summary>
public enum DockCompassZone
{
    /// <summary>
    /// Docks the dragged pane above the nearest shell or split target.
    /// </summary>
    TopOuter,

    /// <summary>
    /// Docks the dragged pane above the pane under the pointer.
    /// </summary>
    TopInner,

    /// <summary>
    /// Docks the dragged pane to the left of the nearest shell or split target.
    /// </summary>
    LeftOuter,

    /// <summary>
    /// Docks the dragged pane to the left of the pane under the pointer.
    /// </summary>
    LeftInner,

    /// <summary>
    /// Tabs the dragged pane with the pane under the pointer.
    /// </summary>
    Center,

    /// <summary>
    /// Docks the dragged pane to the right of the pane under the pointer.
    /// </summary>
    RightInner,

    /// <summary>
    /// Docks the dragged pane to the right of the nearest shell or split target.
    /// </summary>
    RightOuter,

    /// <summary>
    /// Docks the dragged pane below the pane under the pointer.
    /// </summary>
    BottomInner,

    /// <summary>
    /// Docks the dragged pane below the nearest shell or split target.
    /// </summary>
    BottomOuter,
}