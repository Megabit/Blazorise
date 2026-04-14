namespace Blazorise.Maps;

/// <summary>
/// Represents animation options for map movement.
/// </summary>
public class MapAnimationOptions
{
    /// <summary>
    /// Gets or sets whether animation is enabled.
    /// </summary>
    public bool Animate { get; set; } = true;

    /// <summary>
    /// Gets or sets animation duration in seconds.
    /// </summary>
    public double? Duration { get; set; }
}