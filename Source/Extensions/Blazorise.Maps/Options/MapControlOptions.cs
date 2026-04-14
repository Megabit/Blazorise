namespace Blazorise.Maps;

/// <summary>
/// Represents map control options.
/// </summary>
public class MapControlOptions
{
    /// <summary>
    /// Gets or sets whether the zoom control is shown.
    /// </summary>
    public bool Zoom { get; set; } = true;

    /// <summary>
    /// Gets or sets whether attribution is shown.
    /// </summary>
    public bool Attribution { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the scale control is shown.
    /// </summary>
    public bool Scale { get; set; }
}