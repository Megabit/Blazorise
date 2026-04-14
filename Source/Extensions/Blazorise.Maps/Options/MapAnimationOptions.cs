namespace Blazorise.Maps;

/// <summary>
/// Configures animation for map movement.
/// </summary>
public class MapAnimationOptions
{
    /// <summary>
    /// Enables animated map movement.
    /// </summary>
    public bool Animated { get; set; } = true;

    /// <summary>
    /// Defines animation duration in seconds.
    /// </summary>
    public double? Duration { get; set; }
}