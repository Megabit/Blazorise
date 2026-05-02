namespace Blazorise.PivotGrid;

/// <summary>
/// Defines how grouped pivot headers are captioned.
/// </summary>
public enum PivotGridGroupCaptionMode
{
    /// <summary>
    /// Shows the full grouped path.
    /// </summary>
    FullPath,

    /// <summary>
    /// Shows only the current grouped value.
    /// </summary>
    Leaf,

    /// <summary>
    /// Shows the current grouped field caption.
    /// </summary>
    Field,

    /// <summary>
    /// Hides the grouped caption.
    /// </summary>
    Hidden,
}