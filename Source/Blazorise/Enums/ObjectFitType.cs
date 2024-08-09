namespace Blazorise;

/// <summary>
/// Specifies how an object should fit within its container.
/// </summary>
public enum ObjectFitType
{
    /// <summary>
    /// The default object fit behavior.
    /// </summary>
    Default,

    /// <summary>
    /// The object does not scale to fit the container and may be clipped.
    /// </summary>
    None,

    /// <summary>
    /// The object is scaled to maintain its aspect ratio while fitting within the container.
    /// </summary>
    Contain,

    /// <summary>
    /// The object is scaled to cover the entire container, possibly clipping some parts.
    /// </summary>
    Cover,

    /// <summary>
    /// The object is scaled to completely fill the container. 
    /// Its aspect ratio is not preserved, and the object may be stretched or compressed.
    /// </summary>
    Fill,

    /// <summary>
    /// The object is scaled to fill the container without preserving its aspect ratio.
    /// </summary>
    Scale,
}