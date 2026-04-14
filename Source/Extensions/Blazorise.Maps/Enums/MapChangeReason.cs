namespace Blazorise.Maps;

/// <summary>
/// Identifies why the map view changed.
/// </summary>
public enum MapChangeReason
{
    /// <summary>
    /// The reason is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// The view changed because of user interaction.
    /// </summary>
    User,

    /// <summary>
    /// The view changed because of an API call.
    /// </summary>
    Programmatic,

    /// <summary>
    /// The view changed during initial rendering.
    /// </summary>
    Initial,
}