namespace Blazorise;

/// <summary>
/// Groups the logical targets located on either side of a resize handle.
/// </summary>
public sealed record ResizeHandleTargets
{
    /// <summary>
    /// Represents the target at the logical start of the resize axis.
    /// </summary>
    public ResizeHandleTarget Start { get; init; }

    /// <summary>
    /// Represents the target at the logical end of the resize axis.
    /// </summary>
    public ResizeHandleTarget End { get; init; }
}