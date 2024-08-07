namespace Blazorise;

/// <summary>
/// Defines the text decoration.
/// </summary>
public enum TextDecoration
{
    /// <summary>
    /// No decoration. The text renders as it is. This is default.
    /// </summary>
    Default,

    /// <summary>
    /// No line is drawn, and any existing decoration is removed.
    /// </summary>
    None,

    /// <summary>
    /// Draws a line across the text at its baseline.
    /// </summary>
    Underline,

    /// <summary>
    /// Draws a line across the text, directly above its "top" point.
    /// </summary>
    Overline,

    /// <summary>
    /// Draws a line across the text at its "middle" point.
    /// </summary>
    LineThrough,

    /// <summary>
    /// Inherits the decoration from its parent element.
    /// </summary>
    Inherit,
}
