namespace Blazorise;

/// <summary>
/// Defines the placement strategy strategy of a toast element.
/// </summary>
public enum ToasterPlacementStrategy
{
    /// <summary>
    /// Positioned relative to the screen coordinates.
    /// </summary>
    Fixed,

    /// <summary>
    /// Positioned relative to the closest positioned ancestor.
    /// </summary>
    Absolute,
}
