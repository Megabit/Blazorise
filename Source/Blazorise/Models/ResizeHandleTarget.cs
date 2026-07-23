namespace Blazorise;

/// <summary>
/// Describes an element participating in a resize-handle interaction.
/// </summary>
public sealed record ResizeHandleTarget
{
    /// <summary>
    /// Identifies the element whose rendered size is measured.
    /// </summary>
    public string ElementId { get; init; }

    /// <summary>
    /// Identifies the element receiving the resized CSS property. The measured element is used when omitted.
    /// </summary>
    public string ResizeElementId { get; init; }

    /// <summary>
    /// Names the CSS property updated on the resize element.
    /// </summary>
    public string ResizeProperty { get; init; }

    /// <summary>
    /// Prevents the target from shrinking below this CSS length.
    /// </summary>
    public string MinSize { get; init; }

    /// <summary>
    /// Limits the target to this CSS length when specified.
    /// </summary>
    public string MaxSize { get; init; }
}