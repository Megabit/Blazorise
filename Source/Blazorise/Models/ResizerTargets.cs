namespace Blazorise;

/// <summary>
/// Groups the logical targets located on either side of a resizer.
/// </summary>
public sealed record ResizerTargets
{
    /// <summary>
    /// Represents the target at the logical start of the resize axis.
    /// </summary>
    public ResizerTarget Start { get; init; }

    /// <summary>
    /// Represents the target at the logical end of the resize axis.
    /// </summary>
    public ResizerTarget End { get; init; }
}