namespace Blazorise;

/// <summary>
/// Defines the browser touch-action behavior for gesture detection.
/// </summary>
public enum GestureTouchAction
{
    /// <summary>
    /// Browser default touch behavior.
    /// </summary>
    Auto,

    /// <summary>
    /// Disables browser handling for all panning and zooming gestures.
    /// </summary>
    None,

    /// <summary>
    /// Allows horizontal browser panning.
    /// </summary>
    PanX,

    /// <summary>
    /// Allows vertical browser panning.
    /// </summary>
    PanY,

    /// <summary>
    /// Allows panning and pinch zooming, but disables additional non-standard gestures.
    /// </summary>
    Manipulation,
}