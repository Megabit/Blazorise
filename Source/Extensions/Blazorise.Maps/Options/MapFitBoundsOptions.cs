namespace Blazorise.Maps;

/// <summary>
/// Configures how bounds are fitted into the visible map area.
/// </summary>
public class MapFitBoundsOptions : MapAnimationOptions
{
    /// <summary>
    /// Adds padding in pixels around the fitted bounds.
    /// </summary>
    public MapPoint? Padding { get; set; }
}