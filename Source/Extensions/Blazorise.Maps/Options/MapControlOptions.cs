namespace Blazorise.Maps;

/// <summary>
/// Configures built-in map controls.
/// </summary>
public class MapControlOptions
{
    /// <summary>
    /// Shows or hides the zoom control.
    /// </summary>
    public bool Zoom { get; set; } = true;

    /// <summary>
    /// Shows or hides provider attribution.
    /// </summary>
    public bool Attribution { get; set; } = true;

    /// <summary>
    /// Shows or hides the scale control.
    /// </summary>
    public bool Scale { get; set; }
}